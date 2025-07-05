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
        pub level: u8, // Station level (0-5) - current active level
        pub multiplier: u128, // Current multiplier (in basis points)
        pub mine_locked: u256, // Amount of MINE tokens locked
        pub lock_timestamp: u64, // When tokens were locked
        pub unlock_timestamp: u64, // When tokens can be unlocked (for downgrades)
        pub pending_downgrade: u8, // Original level before downgrade (for cancellation, 0 if none)
        pub miner_count: u8 // Number of miners assigned to this station
    }

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct LevelConfig {
        pub multiplier: u128, // Multiplier for this level (in basis points)
        pub mine_required: u256, // MINE tokens required to reach this level
        pub unlock_period: u64 // Blocks to wait for downgrade
    }

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct MinerAssignment {
        pub token_id: u256, // NFT token ID (0 if slot is empty)
        pub assigned_timestamp: u64 // When the miner was assigned
    }

    const SECONDS_PER_DAY: u64 = 86400;
    const UNLOCK_PERIOD_DAYS: u64 = 14;
    const DEFAULT_STATIONS_COUNT: u8 = 10;
    const MAX_MINERS_PER_STATION: u8 = 6;

    #[storage]
    struct Storage {
        // Station data per account and station ID: (account, station_id) -> StationInfo
        stations: Map<(ContractAddress, u8), StationInfo>,
        // User station count tracking
        user_station_count: Map<ContractAddress, u8>,
        // Miner assignments per station: (account, station_id, miner_slot) -> MinerAssignment
        station_miners: Map<(ContractAddress, u8, u8), MinerAssignment>,
        // Track which station a miner is assigned to: (account, token_id) -> station_id (0 if
        // unassigned)
        miner_station_assignment: Map<(ContractAddress, u256), u8>,
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
        MinerAssigned: MinerAssigned,
        MinerRemoved: MinerRemoved,
        StationsInitialized: StationsInitialized,
    }

    #[derive(Drop, starknet::Event)]
    struct StationUpgraded {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        old_level: u8,
        new_level: u8,
        new_multiplier: u128,
        mine_locked: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct StationDowngraded {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        old_level: u8,
        new_level: u8,
        new_multiplier: u128,
        mine_unlocked: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct TokensLocked {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        amount: u256,
        new_total: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct TokensUnlocked {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        amount: u256,
        remaining: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct DowngradeRequested {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        current_level: u8,
        target_level: u8,
        unlock_timestamp: u64,
    }

    #[derive(Drop, starknet::Event)]
    struct DowngradeCanceled {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        canceled_level: u8,
    }

    #[derive(Drop, starknet::Event)]
    struct EmergencyWithdrawal {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        amount: u256,
        penalty: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerAssigned {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        #[key]
        token_id: u256,
        miner_slot: u8,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerRemoved {
        #[key]
        account: ContractAddress,
        #[key]
        station_id: u8,
        #[key]
        token_id: u256,
        miner_slot: u8,
    }

    #[derive(Drop, starknet::Event)]
    struct StationsInitialized {
        #[key]
        account: ContractAddress,
        station_count: u8,
    }

    #[constructor]
    fn constructor(ref self: ContractState, owner: ContractAddress, mine_token: ContractAddress) {
        self.owner.write(owner);
        self.mine_token_address.write(mine_token);
        self.max_level.write(5);

        // Set unlock period (14 days in blocks, assuming 3 second block time)
        let unlock_blocks = UNLOCK_PERIOD_DAYS * SECONDS_PER_DAY / 3;
        self.unlock_period_blocks.write(unlock_blocks);

        // Initialize level configurations
        self.initialize_levels();
    }

    #[abi(embed_v0)]
    impl StationSystemImpl of IStationSystem<ContractState> {
        fn initialize_user_stations(ref self: ContractState) {
            let caller = get_caller_address();
            let current_count = self.user_station_count.read(caller);

            // Only initialize if user doesn't have stations yet
            assert!(current_count == 0, "Stations already initialized");

            // Create 10 default stations at level 0
            let mut i: u8 = 1;
            while i <= DEFAULT_STATIONS_COUNT {
                let level_0_config = self.level_configs.read(0);
                let station_info = StationInfo {
                    level: 0,
                    multiplier: level_0_config.multiplier,
                    mine_locked: 0,
                    lock_timestamp: 0,
                    unlock_timestamp: 0,
                    pending_downgrade: 0,
                    miner_count: 0,
                };
                self.stations.write((caller, i), station_info);
                i += 1;
            }

            self.user_station_count.write(caller, DEFAULT_STATIONS_COUNT);

            self
                .emit(
                    StationsInitialized { account: caller, station_count: DEFAULT_STATIONS_COUNT },
                );
        }

        fn upgrade_station(ref self: ContractState, station_id: u8, target_level: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);
            assert!(target_level > 0 && target_level <= self.max_level.read(), "Invalid level");

            let mut station = self.stations.read((caller, station_id));
            assert!(target_level > station.level, "Can only upgrade to higher level");
            assert!(station.pending_downgrade == 0, "Cannot upgrade during pending downgrade");

            let target_config = self.level_configs.read(target_level);
            let current_config = self.level_configs.read(station.level);

            // Calculate additional MINE needed
            let additional_mine = target_config.mine_required - current_config.mine_required;

            if additional_mine > 0 {
                // Process MINE token lock
                self.lock_mine_tokens(caller, station_id, additional_mine);
            }

            // CRITICAL FIX: Sync hash power BEFORE changing station level
            // This ensures we remove the correct amount (with old multiplier) from
            // RewardDistributor
            self.sync_station_miners_hash_power(caller, station_id);

            let old_level = station.level;
            station.level = target_level;
            station.multiplier = target_config.multiplier;
            station.mine_locked = target_config.mine_required;
            station.lock_timestamp = get_block_number().try_into().unwrap();
            self.stations.write((caller, station_id), station);

            // Sync hash power again to add the new amount (with new multiplier) to
            // RewardDistributor
            self.sync_station_miners_hash_power(caller, station_id);

            self
                .emit(
                    StationUpgraded {
                        account: caller,
                        station_id,
                        old_level,
                        new_level: target_level,
                        new_multiplier: target_config.multiplier,
                        mine_locked: additional_mine,
                    },
                );
        }

        fn request_downgrade(ref self: ContractState, station_id: u8, target_level: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);

            let mut station = self.stations.read((caller, station_id));
            assert!(target_level < station.level, "Can only downgrade to lower level");
            assert!(station.pending_downgrade == 0, "Downgrade already pending");

            let current_block = get_block_number().try_into().unwrap();
            let unlock_timestamp = current_block + 1;

            // Get target configuration
            let target_config = self.level_configs.read(target_level);

            // Store original level in pending_downgrade for potential cancellation
            let original_level = station.level;
            station.pending_downgrade = original_level;

            // CRITICAL FIX: Sync hash power BEFORE changing station level
            // This ensures we remove the correct amount (with old multiplier) from
            // RewardDistributor
            self.sync_station_miners_hash_power(caller, station_id);

            // IMMEDIATE CHANGES: Apply level and multiplier changes right away
            station.level = target_level;
            station.multiplier = target_config.multiplier;
            station.unlock_timestamp = unlock_timestamp;
            self.stations.write((caller, station_id), station);

            // IMMEDIATE EFFECT: Sync hash power again to add new amount with new (lower) multiplier
            self.sync_station_miners_hash_power(caller, station_id);

            self
                .emit(
                    DowngradeRequested {
                        account: caller,
                        station_id,
                        current_level: original_level,
                        target_level,
                        unlock_timestamp,
                    },
                );
        }

        fn execute_downgrade(ref self: ContractState, station_id: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);

            let mut station = self.stations.read((caller, station_id));
            assert!(station.pending_downgrade > 0, "No pending downgrade");

            let current_block = get_block_number().try_into().unwrap();
            assert!(current_block >= station.unlock_timestamp, "Unlock period not complete");

            // Get original and current level configurations for MINE calculation
            let original_level = station.pending_downgrade; // This stores the original level
            let current_level = station.level; // This is already the target level
            let original_config = self.level_configs.read(original_level);
            let current_config = self.level_configs.read(current_level);

            // Calculate MINE to unlock (difference between original and current requirements)
            let mine_to_unlock = original_config.mine_required - current_config.mine_required;

            // Update station state - only MINE and pending status changes
            station.mine_locked = current_config.mine_required;
            station.pending_downgrade = 0; // Clear pending status
            station.unlock_timestamp = 0; // Clear unlock timestamp
            self.stations.write((caller, station_id), station);

            // Unlock excess MINE tokens
            if mine_to_unlock > 0 {
                self.unlock_mine_tokens(caller, station_id, mine_to_unlock);
            }

            self
                .emit(
                    StationDowngraded {
                        account: caller,
                        station_id,
                        old_level: original_level,
                        new_level: current_level,
                        new_multiplier: station.multiplier,
                        mine_unlocked: mine_to_unlock,
                    },
                );
        }

        fn cancel_downgrade(ref self: ContractState, station_id: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);

            let mut station = self.stations.read((caller, station_id));
            assert!(station.pending_downgrade > 0, "No pending downgrade");

            // Restore original level and multiplier from pending_downgrade
            let original_level = station.pending_downgrade;
            let original_config = self.level_configs.read(original_level);

            let downgraded_level = station.level; // Store for event

            // CRITICAL FIX: Sync hash power BEFORE changing station level
            // This ensures we remove the correct amount (with current lower multiplier) from
            // RewardDistributor
            self.sync_station_miners_hash_power(caller, station_id);

            // RESTORE ORIGINAL STATE: Revert level and multiplier back to original
            station.level = original_level;
            station.multiplier = original_config.multiplier;
            station.pending_downgrade = 0; // Clear pending status
            station.unlock_timestamp = 0; // Clear unlock timestamp
            self.stations.write((caller, station_id), station);

            // IMMEDIATE EFFECT: Sync hash power again with restored (higher) multiplier
            self.sync_station_miners_hash_power(caller, station_id);

            self
                .emit(
                    DowngradeCanceled {
                        account: caller, station_id, canceled_level: downgraded_level,
                    },
                );
        }

        fn emergency_withdraw(ref self: ContractState, station_id: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);

            let mut station = self.stations.read((caller, station_id));
            assert!(station.mine_locked > 0, "No locked tokens");

            // Apply 20% penalty for emergency withdrawal
            let penalty = (station.mine_locked * 20) / 100;
            let withdrawal_amount = station.mine_locked - penalty;

            // CRITICAL FIX: Sync hash power BEFORE changing station level
            // This ensures we remove the correct amount (with current multiplier) from
            // RewardDistributor
            self.sync_station_miners_hash_power(caller, station_id);

            // Reset station to level 0
            let _old_level = station.level;
            station.level = 0;
            station.multiplier = 10000; // 100% (no bonus)
            let _total_locked = station.mine_locked;
            station.mine_locked = 0;
            station.pending_downgrade = 0;
            station.unlock_timestamp = 0;
            self.stations.write((caller, station_id), station);

            // Unlock tokens with penalty
            self.unlock_mine_tokens(caller, station_id, withdrawal_amount);
            // TODO: Handle penalty (burn or send to treasury)

            // Sync hash power again for all miners with level 0 multiplier
            self.sync_station_miners_hash_power(caller, station_id);

            self
                .emit(
                    EmergencyWithdrawal {
                        account: caller, station_id, amount: withdrawal_amount, penalty,
                    },
                );
        }

        fn assign_miner_to_station(ref self: ContractState, station_id: u8, token_id: u256, miner_slot: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);

            // Check if miner is already assigned somewhere
            let current_assignment = self.miner_station_assignment.read((caller, token_id));
            assert!(current_assignment == 0, "Miner already assigned to a station");

            // Check station capacity
            let mut station = self.stations.read((caller, station_id));
            assert!(station.miner_count < MAX_MINERS_PER_STATION, "Station at maximum capacity");

            // Check miner slot is valid
            assert!(miner_slot > 0 && miner_slot <= MAX_MINERS_PER_STATION, "Invalid miner slot");
            let assignment = self.station_miners.read((caller, station_id, miner_slot));
            assert!(assignment.token_id == 0, "Miner slot is already assigned");

            // Assign miner to slot
            let assignment = MinerAssignment {
                token_id, assigned_timestamp: get_block_number().try_into().unwrap(),
            };
            self.station_miners.write((caller, station_id, miner_slot), assignment);
            self.miner_station_assignment.write((caller, token_id), station_id);

            // Update station miner count
            station.miner_count += 1;
            self.stations.write((caller, station_id), station);

            // Sync miner hash power
            self.sync_miner_hash_power_internal(token_id);

            self.emit(MinerAssigned { account: caller, station_id, token_id, miner_slot: miner_slot });
        }

        fn remove_miner_from_station(ref self: ContractState, station_id: u8, miner_slot: u8) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_valid_station(caller, station_id);
            assert!(miner_slot > 0 && miner_slot <= MAX_MINERS_PER_STATION, "Invalid miner slot");

            let assignment = self.station_miners.read((caller, station_id, miner_slot));
            assert!(assignment.token_id != 0, "No miner in this slot");

            let token_id = assignment.token_id;

            // Remove assignment
            let empty_assignment = MinerAssignment { token_id: 0, assigned_timestamp: 0 };
            self.station_miners.write((caller, station_id, miner_slot), empty_assignment);
            self.miner_station_assignment.write((caller, token_id), 0);

            // Update station miner count
            let mut station = self.stations.read((caller, station_id));
            station.miner_count -= 1;
            self.stations.write((caller, station_id), station);

            // Sync miner hash power (now without station bonus)
            self.sync_miner_hash_power_internal(token_id);

            self.emit(MinerRemoved { account: caller, station_id, token_id, miner_slot });
        }

        fn get_station_info(
            self: @ContractState, account: ContractAddress, station_id: u8,
        ) -> StationInfo {
            self.stations.read((account, station_id))
        }

        fn get_account_multiplier(
            self: @ContractState, account: ContractAddress, station_id: u8,
        ) -> u128 {
            let station = self.stations.read((account, station_id));

            // If station is uninitialized (multiplier = 0), return Level 0 multiplier
            if station.multiplier == 0 {
                let level_0_config = self.level_configs.read(0);
                return level_0_config.multiplier;
            }

            station.multiplier
        }

        fn get_miner_station_assignment(
            self: @ContractState, account: ContractAddress, token_id: u256,
        ) -> u8 {
            self.miner_station_assignment.read((account, token_id))
        }

        fn get_station_miners(
            self: @ContractState, account: ContractAddress, station_id: u8,
        ) -> Array<u256> {
            let mut miners = ArrayTrait::new();
            let mut slot: u8 = 1;

            while slot <= MAX_MINERS_PER_STATION {
                let assignment = self.station_miners.read((account, station_id, slot));
                if assignment.token_id != 0 {
                    miners.append(assignment.token_id);
                }
                slot += 1;
            }

            miners
        }

        fn get_user_station_count(self: @ContractState, account: ContractAddress) -> u8 {
            self.user_station_count.read(account)
        }

        fn get_level_config(self: @ContractState, level: u8) -> LevelConfig {
            self.level_configs.read(level)
        }

        fn get_time_until_unlock(
            self: @ContractState, account: ContractAddress, station_id: u8,
        ) -> u64 {
            let station = self.stations.read((account, station_id));
            if station.pending_downgrade == 0 {
                return 0;
            }

            let current_block = get_block_number().try_into().unwrap();
            if current_block >= station.unlock_timestamp {
                return 0;
            }

            station.unlock_timestamp - current_block
        }

        fn can_execute_downgrade(
            self: @ContractState, account: ContractAddress, station_id: u8,
        ) -> bool {
            let station = self.stations.read((account, station_id));
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

            // Level 5: 250% multiplier (150% bonus), 62k MINE
            self
                .level_configs
                .write(
                    5,
                    LevelConfig {
                        multiplier: 25000, // 250%
                        mine_required: 62000_000_000_000_000_000_000,
                        unlock_period: self.unlock_period_blocks.read(),
                    },
                );
        }

        fn lock_mine_tokens(
            ref self: ContractState, from: ContractAddress, station_id: u8, amount: u256,
        ) {
            if amount > 0 {
                // TODO: Transfer MINE tokens from user to contract
                // This would interact with the MINE token contract

                self
                    .emit(
                        TokensLocked {
                            account: from,
                            station_id,
                            amount,
                            new_total: self.stations.read((from, station_id)).mine_locked + amount,
                        },
                    );
            }
        }

        fn unlock_mine_tokens(
            ref self: ContractState, to: ContractAddress, station_id: u8, amount: u256,
        ) {
            if amount > 0 {
                // TODO: Transfer MINE tokens from contract to user
                // This would interact with the MINE token contract

                self
                    .emit(
                        TokensUnlocked {
                            account: to,
                            station_id,
                            amount,
                            remaining: self.stations.read((to, station_id)).mine_locked - amount,
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

        fn require_valid_station(self: @ContractState, account: ContractAddress, station_id: u8) {
            let station_count = self.user_station_count.read(account);
            assert!(station_count > 0, "User stations not initialized");
            assert!(station_id > 0 && station_id <= station_count, "Invalid station ID");
        }

        fn sync_station_miners_hash_power(
            ref self: ContractState, account: ContractAddress, station_id: u8,
        ) {
            // Sync hash power for all miners in a specific station
            let mut slot: u8 = 1;
            while slot <= MAX_MINERS_PER_STATION {
                let assignment = self.station_miners.read((account, station_id, slot));
                if assignment.token_id != 0 {
                    self.sync_miner_hash_power_internal(assignment.token_id);
                }
                slot += 1;
            };
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
    // Station management
    fn initialize_user_stations(ref self: TContractState);
    fn upgrade_station(ref self: TContractState, station_id: u8, target_level: u8);
    fn request_downgrade(ref self: TContractState, station_id: u8, target_level: u8);
    fn execute_downgrade(ref self: TContractState, station_id: u8);
    fn cancel_downgrade(ref self: TContractState, station_id: u8);
    fn emergency_withdraw(ref self: TContractState, station_id: u8);

    // Miner assignment
    fn assign_miner_to_station(ref self: TContractState, station_id: u8, token_id: u256, miner_slot: u8);
    fn remove_miner_from_station(ref self: TContractState, station_id: u8, miner_slot: u8);

    // View functions
    fn get_station_info(
        self: @TContractState, account: starknet::ContractAddress, station_id: u8,
    ) -> StationSystem::StationInfo;
    fn get_account_multiplier(
        self: @TContractState, account: starknet::ContractAddress, station_id: u8,
    ) -> u128;
    fn get_miner_station_assignment(
        self: @TContractState, account: starknet::ContractAddress, token_id: u256,
    ) -> u8;
    fn get_station_miners(
        self: @TContractState, account: starknet::ContractAddress, station_id: u8,
    ) -> Array<u256>;
    fn get_user_station_count(self: @TContractState, account: starknet::ContractAddress) -> u8;
    fn get_level_config(self: @TContractState, level: u8) -> StationSystem::LevelConfig;
    fn get_time_until_unlock(
        self: @TContractState, account: starknet::ContractAddress, station_id: u8,
    ) -> u64;
    fn can_execute_downgrade(
        self: @TContractState, account: starknet::ContractAddress, station_id: u8,
    ) -> bool;

    // Admin functions
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn set_miner_nft_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn update_level_config(ref self: TContractState, level: u8, config: StationSystem::LevelConfig);
    fn set_unlock_period(ref self: TContractState, blocks: u64);
    fn pause(ref self: TContractState);
    fn unpause(ref self: TContractState);

    fn sync_miner_hash_power(ref self: TContractState, token_id: u256);
}
