#[starknet::contract]
mod StationSystem {
    use core::num::traits::Zero;
    use starkmine::mining::station_system::IStationSystem;
    use starkmine::nft::miner_nft::{IMinerNFTDispatcher, IMinerNFTDispatcherTrait};
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address};

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct StationInfo {
        pub level: u8, // Station level (0-4)
        pub multiplier: u128, // Current multiplier (in basis points)
        pub mine_locked: u256, // Amount of MINE tokens locked
        pub lock_timestamp: u64, // When tokens were locked
        pub unlock_timestamp: u64, // When tokens can be unlocked (for downgrades)
        pub pending_downgrade: u8 // Pending downgrade level (0 if none)
    }

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct LevelConfig {
        pub multiplier: u128, // Multiplier for this level (in basis points)
        pub mine_required: u256, // MINE tokens required to reach this level
        pub unlock_period: u64 // Blocks to wait for downgrade
    }

    const SECONDS_PER_DAY: u64 = 86400;
    const UNLOCK_PERIOD_DAYS: u64 = 14;

    #[storage]
    struct Storage {
        // Station data per account
        stations: Map<ContractAddress, StationInfo>,
        level_configs: Map<u8, LevelConfig>,
        // Contract addresses
        mine_token_address: ContractAddress,
        miner_nft_contract: ContractAddress,
        // Global settings
        max_level: u8,
        unlock_period_blocks: u64,
        // Admin
        owner: ContractAddress,
        paused: bool,
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        StationUpgraded: StationUpgraded,
        StationDowngraded: StationDowngraded,
        TokensLocked: TokensLocked,
        TokensUnlocked: TokensUnlocked,
        DowngradeRequested: DowngradeRequested,
        DowngradeCanceled: DowngradeCanceled,
        EmergencyWithdrawal: EmergencyWithdrawal,
    }

    #[derive(Drop, starknet::Event)]
    struct StationUpgraded {
        #[key]
        account: ContractAddress,
        old_level: u8,
        new_level: u8,
        new_multiplier: u128,
        mine_locked: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct StationDowngraded {
        #[key]
        account: ContractAddress,
        old_level: u8,
        new_level: u8,
        new_multiplier: u128,
        mine_unlocked: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct TokensLocked {
        #[key]
        account: ContractAddress,
        amount: u256,
        new_total: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct TokensUnlocked {
        #[key]
        account: ContractAddress,
        amount: u256,
        remaining: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct DowngradeRequested {
        #[key]
        account: ContractAddress,
        current_level: u8,
        target_level: u8,
        unlock_timestamp: u64,
    }

    #[derive(Drop, starknet::Event)]
    struct DowngradeCanceled {
        #[key]
        account: ContractAddress,
        canceled_level: u8,
    }

    #[derive(Drop, starknet::Event)]
    struct EmergencyWithdrawal {
        #[key]
        account: ContractAddress,
        amount: u256,
        penalty: u256,
    }

    #[constructor]
    fn constructor(ref self: ContractState, owner: ContractAddress, mine_token: ContractAddress) {
        self.owner.write(owner);
        self.mine_token_address.write(mine_token);
        self.max_level.write(4);

        // Set unlock period (14 days in blocks, assuming 3 second block time)
        let unlock_blocks = UNLOCK_PERIOD_DAYS * SECONDS_PER_DAY / 3;
        self.unlock_period_blocks.write(unlock_blocks);

        // Initialize level configurations
        self.initialize_levels();
    }

    #[abi(embed_v0)]
    impl StationSystemImpl of IStationSystem<ContractState> {
        fn upgrade_station(ref self: ContractState, target_level: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            assert!(target_level > 0 && target_level <= self.max_level.read(), "Invalid level");

            let mut station = self.stations.read(caller);
            assert!(target_level > station.level, "Can only upgrade to higher level");
            assert!(station.pending_downgrade == 0, "Cannot upgrade during pending downgrade");

            let target_config = self.level_configs.read(target_level);
            let current_config = self.level_configs.read(station.level);

            // Calculate additional MINE needed
            let additional_mine = target_config.mine_required - current_config.mine_required;

            if additional_mine > 0 {
                // Process MINE token lock
                self.lock_mine_tokens(caller, additional_mine);
            }

            let old_level = station.level;
            station.level = target_level;
            station.multiplier = target_config.multiplier;
            station.mine_locked = target_config.mine_required;
            station.lock_timestamp = get_block_number().try_into().unwrap();
            self.stations.write(caller, station);

            // Sync hash power for all user's miners since station multiplier changed
            self.sync_user_miners_hash_power(caller);

            self
                .emit(
                    StationUpgraded {
                        account: caller,
                        old_level,
                        new_level: target_level,
                        new_multiplier: target_config.multiplier,
                        mine_locked: additional_mine,
                    },
                );
        }

        fn request_downgrade(ref self: ContractState, target_level: u8) {
            self.require_not_paused();
            let caller = get_caller_address();

            let mut station = self.stations.read(caller);
            assert!(target_level < station.level, "Can only downgrade to lower level");
            assert!(station.pending_downgrade == 0, "Downgrade already pending");

            let current_block = get_block_number().try_into().unwrap();
            let unlock_timestamp = current_block + self.unlock_period_blocks.read();

            station.pending_downgrade = target_level;
            station.unlock_timestamp = unlock_timestamp;
            self.stations.write(caller, station);

            self
                .emit(
                    DowngradeRequested {
                        account: caller,
                        current_level: station.level,
                        target_level,
                        unlock_timestamp,
                    },
                );
        }

        fn execute_downgrade(ref self: ContractState) {
            self.require_not_paused();
            let caller = get_caller_address();

            let mut station = self.stations.read(caller);
            assert!(station.pending_downgrade > 0, "No pending downgrade");

            let current_block = get_block_number().try_into().unwrap();
            assert!(current_block >= station.unlock_timestamp, "Unlock period not complete");

            let target_level = station.pending_downgrade;
            let target_config = self.level_configs.read(target_level);
            let current_config = self.level_configs.read(station.level);

            // Calculate MINE to unlock
            let mine_to_unlock = current_config.mine_required - target_config.mine_required;

            let old_level = station.level;
            station.level = target_level;
            station.multiplier = target_config.multiplier;
            station.mine_locked = target_config.mine_required;
            station.pending_downgrade = 0;
            station.unlock_timestamp = 0;
            self.stations.write(caller, station);

            // Unlock MINE tokens
            if mine_to_unlock > 0 {
                self.unlock_mine_tokens(caller, mine_to_unlock);
            }

            // Sync hash power for all user's miners since station multiplier changed
            self.sync_user_miners_hash_power(caller);

            self
                .emit(
                    StationDowngraded {
                        account: caller,
                        old_level,
                        new_level: target_level,
                        new_multiplier: target_config.multiplier,
                        mine_unlocked: mine_to_unlock,
                    },
                );
        }

        fn cancel_downgrade(ref self: ContractState) {
            self.require_not_paused();
            let caller = get_caller_address();

            let mut station = self.stations.read(caller);
            assert!(station.pending_downgrade > 0, "No pending downgrade");

            let canceled_level = station.pending_downgrade;
            station.pending_downgrade = 0;
            station.unlock_timestamp = 0;
            self.stations.write(caller, station);

            self.emit(DowngradeCanceled { account: caller, canceled_level });
        }

        fn emergency_withdraw(ref self: ContractState) {
            self.require_not_paused();
            let caller = get_caller_address();

            let mut station = self.stations.read(caller);
            assert!(station.mine_locked > 0, "No locked tokens");

            // Apply 20% penalty for emergency withdrawal
            let penalty = (station.mine_locked * 20) / 100;
            let withdrawal_amount = station.mine_locked - penalty;

            // Reset station to level 0
            let _old_level = station.level;
            station.level = 0;
            station.multiplier = 10000; // 100% (no bonus)
            let _total_locked = station.mine_locked;
            station.mine_locked = 0;
            station.pending_downgrade = 0;
            station.unlock_timestamp = 0;
            self.stations.write(caller, station);

            // Unlock tokens with penalty
            self.unlock_mine_tokens(caller, withdrawal_amount);
            // TODO: Handle penalty (burn or send to treasury)

            // Sync hash power for all user's miners since station was reset to level 0
            self.sync_user_miners_hash_power(caller);

            self.emit(EmergencyWithdrawal { account: caller, amount: withdrawal_amount, penalty });
        }

        fn get_station_info(self: @ContractState, account: ContractAddress) -> StationInfo {
            self.stations.read(account)
        }

        fn get_account_multiplier(self: @ContractState, account: ContractAddress) -> u128 {
            let station = self.stations.read(account);

            // If station is uninitialized (multiplier = 0), return Level 0 multiplier
            if station.multiplier == 0 {
                let level_0_config = self.level_configs.read(0);
                return level_0_config.multiplier;
            }

            station.multiplier
        }

        fn get_level_config(self: @ContractState, level: u8) -> LevelConfig {
            self.level_configs.read(level)
        }

        fn get_time_until_unlock(self: @ContractState, account: ContractAddress) -> u64 {
            let station = self.stations.read(account);
            if station.pending_downgrade == 0 {
                return 0;
            }

            let current_block = get_block_number().try_into().unwrap();
            if current_block >= station.unlock_timestamp {
                return 0;
            }

            station.unlock_timestamp - current_block
        }

        fn can_execute_downgrade(self: @ContractState, account: ContractAddress) -> bool {
            let station = self.stations.read(account);
            if station.pending_downgrade == 0 {
                return false;
            }

            let current_block = get_block_number().try_into().unwrap();
            current_block >= station.unlock_timestamp
        }

        // Admin functions
        fn set_mine_token_address(ref self: ContractState, token: ContractAddress) {
            self.require_owner();
            self.mine_token_address.write(token);
        }

        fn set_miner_nft_contract(ref self: ContractState, contract: ContractAddress) {
            self.require_owner();
            self.miner_nft_contract.write(contract);
        }

        fn update_level_config(ref self: ContractState, level: u8, config: LevelConfig) {
            self.require_owner();
            self.level_configs.write(level, config);
        }

        fn set_unlock_period(ref self: ContractState, blocks: u64) {
            self.require_owner();
            self.unlock_period_blocks.write(blocks);
        }

        fn pause(ref self: ContractState) {
            self.require_owner();
            self.paused.write(true);
        }

        fn unpause(ref self: ContractState) {
            self.require_owner();
            self.paused.write(false);
        }

        fn sync_miner_hash_power(ref self: ContractState, token_id: u256) {
            // Public function to sync a specific miner's hash power after station changes
            // This allows external systems (like frontend) to trigger sync for specific miners
            self.sync_miner_hash_power_internal(token_id);
        }
    }

    // Internal helper functions
    #[generate_trait]
    impl StationSystemInternalImpl of StationSystemInternalTrait {
        fn initialize_levels(ref self: ContractState) {
            // Level 0: 100% multiplier (no bonus), no MINE required
            self
                .level_configs
                .write(
                    0,
                    LevelConfig { multiplier: 10000, // 100%
                    mine_required: 0, unlock_period: 0 },
                );

            // Level 1: 110% multiplier (10% bonus), 2k MINE
            self
                .level_configs
                .write(
                    1,
                    LevelConfig {
                        multiplier: 11000, // 110%
                        mine_required: 2000_000_000_000_000_000_000,
                        unlock_period: self.unlock_period_blocks.read(),
                    },
                );

            // Level 2: 125% multiplier (25% bonus), 5k MINE
            self
                .level_configs
                .write(
                    2,
                    LevelConfig {
                        multiplier: 12500, // 125%
                        mine_required: 5000_000_000_000_000_000_000,
                        unlock_period: self.unlock_period_blocks.read(),
                    },
                );

            // Level 3: 150% multiplier (50% bonus), 12k MINE
            self
                .level_configs
                .write(
                    3,
                    LevelConfig {
                        multiplier: 15000, // 150%
                        mine_required: 12000_000_000_000_000_000_000,
                        unlock_period: self.unlock_period_blocks.read(),
                    },
                );

            // Level 4: 200% multiplier (100% bonus), 30k MINE
            self
                .level_configs
                .write(
                    4,
                    LevelConfig {
                        multiplier: 20000, // 200%
                        mine_required: 30000_000_000_000_000_000_000,
                        unlock_period: self.unlock_period_blocks.read(),
                    },
                );
        }

        fn lock_mine_tokens(ref self: ContractState, from: ContractAddress, amount: u256) {
            if amount > 0 {
                // TODO: Transfer MINE tokens from user to contract
                // This would interact with the MINE token contract

                self
                    .emit(
                        TokensLocked {
                            account: from,
                            amount,
                            new_total: self.stations.read(from).mine_locked + amount,
                        },
                    );
            }
        }

        fn unlock_mine_tokens(ref self: ContractState, to: ContractAddress, amount: u256) {
            if amount > 0 {
                // TODO: Transfer MINE tokens from contract to user
                // This would interact with the MINE token contract

                self
                    .emit(
                        TokensUnlocked {
                            account: to,
                            amount,
                            remaining: self.stations.read(to).mine_locked - amount,
                        },
                    );
            }
        }

        fn require_owner(self: @ContractState) {
            assert!(get_caller_address() == self.owner.read(), "Not owner");
        }

        fn require_not_paused(self: @ContractState) {
            assert!(!self.paused.read(), "Contract is paused");
        }

        fn sync_user_miners_hash_power(ref self: ContractState, account: ContractAddress) {
            // Since we can't track which miners a user owns from this contract,
            // we'll provide a public function for external systems to call
            // This is a placeholder - the actual sync will happen via public function
            let _miner_nft_address = self.miner_nft_contract.read();
            // In practice, the frontend will call sync_miner_hash_power for each owned miner
        }

        fn sync_miner_hash_power_internal(ref self: ContractState, token_id: u256) {
            // Sync a specific miner's hash power after station changes
            let miner_nft_address = self.miner_nft_contract.read();
            if !miner_nft_address.is_zero() {
                let miner_nft = IMinerNFTDispatcher { contract_address: miner_nft_address };
                miner_nft.sync_miner_hash_power(token_id);
            }
        }
    }
}

// Interface for the StationSystem contract
#[starknet::interface]
pub trait IStationSystem<TContractState> {
    fn upgrade_station(ref self: TContractState, target_level: u8);
    fn request_downgrade(ref self: TContractState, target_level: u8);
    fn execute_downgrade(ref self: TContractState);
    fn cancel_downgrade(ref self: TContractState);
    fn emergency_withdraw(ref self: TContractState);
    fn get_station_info(
        self: @TContractState, account: starknet::ContractAddress,
    ) -> StationSystem::StationInfo;
    fn get_account_multiplier(self: @TContractState, account: starknet::ContractAddress) -> u128;
    fn get_level_config(self: @TContractState, level: u8) -> StationSystem::LevelConfig;
    fn get_time_until_unlock(self: @TContractState, account: starknet::ContractAddress) -> u64;
    fn can_execute_downgrade(self: @TContractState, account: starknet::ContractAddress) -> bool;

    // Admin functions
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn set_miner_nft_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn update_level_config(ref self: TContractState, level: u8, config: StationSystem::LevelConfig);
    fn set_unlock_period(ref self: TContractState, blocks: u64);
    fn pause(ref self: TContractState);
    fn unpause(ref self: TContractState);

    fn sync_miner_hash_power(ref self: TContractState, token_id: u256);
}
