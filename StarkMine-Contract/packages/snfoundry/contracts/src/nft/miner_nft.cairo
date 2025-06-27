#[starknet::contract]
mod MinerNFT {
    use core::cmp::{max, min};
    use core::num::traits::Zero;
    use starkmine::interfaces::icore_engine::{ICoreEngineDispatcher, ICoreEngineDispatcherTrait};
    use starkmine::mining::interface::{
        IRewardDistributorDispatcher, IRewardDistributorDispatcherTrait,
    };
    use starkmine::mining::station_system::IStationSystemDispatcherTrait;
    use starkmine::nft::miner_nft::IMinerNFT;
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address};


    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct MinerInfo {
        pub tier: felt252, // 'Basic', 'Elite', 'Pro', 'GIGA'
        pub hash_power: u128, // Base hash power in TH/s
        pub level: u8, // Upgrade level (0-5)
        pub efficiency: u8, // Current efficiency percentage (80-100)
        pub last_maintenance: u64, // Block number of last maintenance
        pub core_engine_id: u256, // Attached core engine (0 if none)
        pub is_ignited: bool // Whether miner is currently active
    }

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct TierConfig {
        pub base_hash_power: u128, // Base hash power for this tier
        pub tier_bonus: u128, // Tier bonus percentage (in basis points)
        pub mint_cost_mine: u256, // Cost in MINE tokens
        pub mint_cost_strk: u256, // Cost in STRK tokens
        pub supply_limit: u256, // Maximum supply for this tier
        pub minted_count: u256 // Current minted count
    }

    #[storage]
    struct Storage {
        // ERC-721 Standard
        name: felt252,
        symbol: felt252,
        owners: Map<u256, ContractAddress>,
        balances: Map<ContractAddress, u256>,
        token_approvals: Map<u256, ContractAddress>,
        operator_approvals: Map<(ContractAddress, ContractAddress), bool>,
        // Miner NFT specific
        miners: Map<u256, MinerInfo>,
        tier_configs: Map<felt252, TierConfig>,
        upgrade_costs: Map<u8, u256>, // Cost for each upgrade level
        next_token_id: u256,
        // Contract addresses
        mine_token_address: ContractAddress,
        strk_token_address: ContractAddress,
        core_engine_contract: ContractAddress,
        station_system_contract: ContractAddress,
        merge_system_contract: ContractAddress,
        reward_distributor_contract: ContractAddress,
        // Admin
        owner: ContractAddress,
        paused: bool,
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        Transfer: Transfer,
        Approval: Approval,
        ApprovalForAll: ApprovalForAll,
        MinerMinted: MinerMinted,
        MinerIgnited: MinerIgnited,
        MinerExtinguished: MinerExtinguished,
        MinerUpgraded: MinerUpgraded,
        MinerMaintained: MinerMaintained,
    }

    #[derive(Drop, starknet::Event)]
    struct Transfer {
        #[key]
        from: ContractAddress,
        #[key]
        to: ContractAddress,
        #[key]
        token_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct Approval {
        #[key]
        owner: ContractAddress,
        #[key]
        approved: ContractAddress,
        #[key]
        token_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct ApprovalForAll {
        #[key]
        owner: ContractAddress,
        #[key]
        operator: ContractAddress,
        approved: bool,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerMinted {
        #[key]
        owner: ContractAddress,
        #[key]
        token_id: u256,
        tier: felt252,
        hash_power: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerIgnited {
        #[key]
        token_id: u256,
        #[key]
        core_engine_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerExtinguished {
        #[key]
        token_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerUpgraded {
        #[key]
        token_id: u256,
        old_level: u8,
        new_level: u8,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerMaintained {
        #[key]
        token_id: u256,
        old_efficiency: u8,
        new_efficiency: u8,
    }

    #[constructor]
    fn constructor(
        ref self: ContractState,
        name: felt252,
        symbol: felt252,
        owner: ContractAddress,
        mine_token: ContractAddress,
        strk_token: ContractAddress,
    ) {
        self.name.write(name);
        self.symbol.write(symbol);
        self.owner.write(owner);
        self.mine_token_address.write(mine_token);
        self.strk_token_address.write(strk_token);
        self.next_token_id.write(1);

        // Initialize tier configurations and upgrade costs
        self.initialize_tiers();
        self.initialize_upgrade_costs();
    }

    #[abi(embed_v0)]
    impl MinerNFTImpl of IMinerNFT<ContractState> {
        // ERC-721 Implementation
        fn name(self: @ContractState) -> felt252 {
            self.name.read()
        }

        fn symbol(self: @ContractState) -> felt252 {
            self.symbol.read()
        }

        fn balance_of(self: @ContractState, owner: ContractAddress) -> u256 {
            assert!(!owner.is_zero(), "Query for zero address");
            self.balances.read(owner)
        }

        fn owner_of(self: @ContractState, token_id: u256) -> ContractAddress {
            let owner = self.owners.read(token_id);
            assert!(!owner.is_zero(), "Token does not exist");
            owner
        }

        fn get_approved(self: @ContractState, token_id: u256) -> ContractAddress {
            self.require_minted(token_id);
            self.token_approvals.read(token_id)
        }

        fn is_approved_for_all(
            self: @ContractState, owner: ContractAddress, operator: ContractAddress,
        ) -> bool {
            self.operator_approvals.read((owner, operator))
        }

        fn approve(ref self: ContractState, to: ContractAddress, token_id: u256) {
            let owner = self.owner_of(token_id);
            assert!(to != owner, "Approval to current owner");

            let caller = get_caller_address();
            assert!(
                caller == owner || self.is_approved_for_all(owner, caller),
                "Not owner nor approved for all",
            );

            self.token_approvals.write(token_id, to);
            self.emit(Approval { owner, approved: to, token_id });
        }

        fn set_approval_for_all(
            ref self: ContractState, operator: ContractAddress, approved: bool,
        ) {
            let caller = get_caller_address();
            assert!(caller != operator, "Approve to caller");
            self.operator_approvals.write((caller, operator), approved);
            self.emit(ApprovalForAll { owner: caller, operator, approved });
        }

        fn transfer_from(
            ref self: ContractState, from: ContractAddress, to: ContractAddress, token_id: u256,
        ) {
            self.require_approved_or_owner(get_caller_address(), token_id);

            // Ensure miner is not currently ignited
            let miner = self.miners.read(token_id);
            assert!(!miner.is_ignited, "Cannot transfer ignited miner");

            self.transfer(from, to, token_id);
        }

        // Miner NFT specific functions
        fn mint_miner(ref self: ContractState, to: ContractAddress, tier: felt252) -> u256 {
            self.require_not_paused();
            let tier_config = self.tier_configs.read(tier);
            assert!(tier_config.base_hash_power > 0, "Invalid tier");
            assert!(tier_config.minted_count < tier_config.supply_limit, "Tier supply exhausted");

            // Process payment
            self.process_payment(to, tier, tier_config);

            let token_id = self.next_token_id.read();

            let miner_info = MinerInfo {
                tier,
                hash_power: tier_config.base_hash_power,
                level: 0,
                efficiency: 100, // Start at 100% efficiency
                last_maintenance: get_block_number().try_into().unwrap(),
                core_engine_id: 0,
                is_ignited: false,
            };

            self.mint(to, token_id);
            self.miners.write(token_id, miner_info);
            self.next_token_id.write(token_id + 1);

            // Update tier minted count
            let mut updated_config = tier_config;
            updated_config.minted_count += 1;
            self.tier_configs.write(tier, updated_config);

            self
                .emit(
                    MinerMinted {
                        owner: to, token_id, tier, hash_power: tier_config.base_hash_power,
                    },
                );

            token_id
        }

        fn ignite_miner(ref self: ContractState, token_id: u256, core_engine_id: u256) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_approved_or_owner(caller, token_id);

            let mut miner = self.miners.read(token_id);
            assert!(!miner.is_ignited, "Miner already ignited");
            assert!(miner.core_engine_id == 0, "Core engine already attached");

            // Verify core engine ownership and attach to miner
            let core_engine_contract = self.core_engine_contract.read();
            assert!(!core_engine_contract.is_zero(), "Core engine contract not set");

            // Create dispatcher for CoreEngine contract
            let core_engine_dispatcher = ICoreEngineDispatcher {
                contract_address: core_engine_contract,
            };

            // Verify caller owns the core engine
            let engine_owner = core_engine_dispatcher.owner_of(core_engine_id);
            assert!(engine_owner == caller, "Caller does not own the core engine");

            // Verify engine is not already attached
            let engine_info = core_engine_dispatcher.get_engine_info(core_engine_id);
            assert!(
                engine_info.attached_miner == 0, "Core engine already attached to another miner",
            );

            // Validate that engine type matches miner tier
            assert!(engine_info.engine_type == miner.tier, "Engine type must match miner tier");

            // Attach engine to miner via CoreEngine contract
            core_engine_dispatcher.attach_to_miner(core_engine_id, token_id, caller);

            // Update miner state
            miner.core_engine_id = core_engine_id;
            miner.is_ignited = true;
            self.miners.write(token_id, miner);

            // Add hash power to reward distributor
            self.update_hash_power_on_ignite(token_id, caller);

            self.emit(MinerIgnited { token_id, core_engine_id });
        }

        fn extinguish_miner(ref self: ContractState, token_id: u256) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_approved_or_owner(caller, token_id);

            let mut miner = self.miners.read(token_id);
            assert!(miner.is_ignited, "Miner not ignited");

            // CRITICAL FIX: Store the effective power BEFORE changing state
            let effective_power_to_remove = self.get_effective_hash_power(token_id);

            // Detach core engine from miner
            let core_engine_contract = self.core_engine_contract.read();
            if !core_engine_contract.is_zero() && miner.core_engine_id > 0 {
                // Create dispatcher for CoreEngine contract
                let core_engine_dispatcher = ICoreEngineDispatcher {
                    contract_address: core_engine_contract,
                };

                // Detach engine from miner via CoreEngine contract
                core_engine_dispatcher.detach_from_miner(miner.core_engine_id, caller);
            }

            // Update miner state BEFORE removing hash power
            miner.core_engine_id = 0;
            miner.is_ignited = false;
            self.miners.write(token_id, miner);

            // Now remove the stored effective power from reward distributor
            let distributor_address = self.reward_distributor_contract.read();
            if !distributor_address.is_zero() && effective_power_to_remove > 0 {
                let distributor = IRewardDistributorDispatcher {
                    contract_address: distributor_address,
                };
                distributor.remove_hash_power(caller, effective_power_to_remove);
            }

            self.emit(MinerExtinguished { token_id });
        }

        fn upgrade_miner(ref self: ContractState, token_id: u256) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_approved_or_owner(caller, token_id);

            let mut miner = self.miners.read(token_id);
            assert!(miner.level < 5, "Miner already at max level");

            // If miner is ignited, sync hash power with RewardDistributor
            if miner.is_ignited {
                self.sync_hash_power_before_change(token_id, caller);
            }

            let new_level = miner.level + 1;
            let upgrade_cost = self.get_upgrade_cost(miner.level);

            // Process payment
            self.process_mine_payment(caller, upgrade_cost);

            let old_level = miner.level;
            miner.level = new_level;
            self.miners.write(token_id, miner);

            // If miner is ignited, sync updated hash power with RewardDistributor
            if miner.is_ignited {
                self.sync_hash_power_after_change(token_id, caller);
            }

            self.emit(MinerUpgraded { token_id, old_level, new_level });
        }

        fn maintain_miner(ref self: ContractState, token_id: u256) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_approved_or_owner(caller, token_id);

            let mut miner = self.miners.read(token_id);
            let current_efficiency = self.get_current_efficiency(token_id);

            if current_efficiency >= 100 {
                return; // No maintenance needed
            }

            // If miner is ignited, sync hash power with RewardDistributor
            if miner.is_ignited {
                self.sync_hash_power_before_change(token_id, caller);
            }

            let maintenance_cost = self.get_maintenance_cost(miner.efficiency);

            // Process payment
            self.process_mine_payment(caller, maintenance_cost);

            let old_efficiency = miner.efficiency;
            miner.efficiency = 100; // Restore to full efficiency
            miner.last_maintenance = get_block_number().try_into().unwrap();
            self.miners.write(token_id, miner);

            // If miner is ignited, sync updated hash power with RewardDistributor
            if miner.is_ignited {
                self.sync_hash_power_after_change(token_id, caller);
            }

            self.emit(MinerMaintained { token_id, old_efficiency, new_efficiency: 100 });
        }

        fn get_miner_info(self: @ContractState, token_id: u256) -> MinerInfo {
            self.require_minted(token_id);
            self.miners.read(token_id)
        }

        fn get_effective_hash_power(self: @ContractState, token_id: u256) -> u128 {
            // If miner is not ignited, its effective hash power is zero
            let miner = self.miners.read(token_id);
            if !miner.is_ignited {
                return 0;
            }

            // 1. Base calculation using tier bonus, level multiplier and current efficiency
            let tier_config = self.tier_configs.read(miner.tier);
            let current_efficiency = self.get_current_efficiency(token_id);
            let level_multiplier = self.get_level_multiplier(miner.level);

            // base_with_tier = base_hash_power * (1 + tier_bonus)
            let base_with_tier = (tier_config.base_hash_power * (10000 + tier_config.tier_bonus))
                / 10000;
            // with_level = base_with_tier * (1 + level_multiplier%)
            let with_level = (base_with_tier * level_multiplier) / 100;
            // with_efficiency = with_level * current_efficiency%
            let mut effective_power = (with_level * current_efficiency.into()) / 100;

            // 2. Apply core engine bonus if applicable
            let core_engine_contract = self.core_engine_contract.read();
            if !core_engine_contract.is_zero() && miner.core_engine_id > 0 {
                let core_engine_dispatcher = ICoreEngineDispatcher {
                    contract_address: core_engine_contract,
                };

                // Fetch efficiency bonus from core engine contract
                let engine_bonus = core_engine_dispatcher
                    .get_current_efficiency_bonus(miner.core_engine_id);
                effective_power = (effective_power * (10000 + engine_bonus)) / 10000;
            }

            // 3. Apply station multiplier if StationSystem contract is configured
            let station_contract = self.station_system_contract.read();
            if !station_contract.is_zero() {
                let station_dispatcher =
                    starkmine::mining::station_system::IStationSystemDispatcher {
                    contract_address: station_contract,
                };

                // Determine owner of the miner and which station this miner is assigned to
                let owner: ContractAddress = self.owners.read(token_id);
                let assigned_station = station_dispatcher
                    .get_miner_station_assignment(owner, token_id);

                // If miner is assigned to a station, apply that station's multiplier
                if assigned_station > 0 {
                    let station_multiplier = station_dispatcher
                        .get_account_multiplier(owner, assigned_station);
                    effective_power = (effective_power * station_multiplier) / 10000;
                }
                // If miner is not assigned to any station, use default Level 0 multiplier (10000 =
            // 100%)
            // No need to apply multiplier as it would be 10000/10000 = 1.0
            }

            effective_power
        }

        fn get_current_efficiency(self: @ContractState, token_id: u256) -> u8 {
            let miner = self.miners.read(token_id);
            let current_block = get_block_number().try_into().unwrap();

            // Calculate days since last maintenance (assuming ~30 blocks per minute)
            let blocks_since_maintenance = current_block - miner.last_maintenance;
            let days_since_maintenance = blocks_since_maintenance / (24 * 60 * 30);

            // Efficiency degrades by 1% per day, minimum 80%
            let efficiency_loss = min(days_since_maintenance * 1, 20);
            max(miner.efficiency - efficiency_loss.try_into().unwrap(), 80)
        }

        fn get_tier_config(self: @ContractState, tier: felt252) -> TierConfig {
            self.tier_configs.read(tier)
        }

        fn burn(ref self: ContractState, token_id: u256) {
            self.require_not_paused();

            let owner = self.owner_of(token_id);
            let caller = get_caller_address();

            // Allow burn from: owner, approved operator, or authorized merge system contract
            let merge_system_address = self.merge_system_contract.read();
            if caller != merge_system_address {
                self.require_approved_or_owner(caller, token_id);
            }

            // Clear approvals
            self.token_approvals.write(token_id, Zero::zero());

            // Update balance
            let balance = self.balances.read(owner);
            self.balances.write(owner, balance - 1);

            // Remove owner
            self.owners.write(token_id, Zero::zero());

            // Emit transfer to zero address (burn)
            let zero_address: ContractAddress = Zero::zero();
            self.emit(Transfer { from: owner, to: zero_address, token_id });
        }

        // Admin functions
        fn set_mine_token_address(ref self: ContractState, token: ContractAddress) {
            self.require_owner();
            self.mine_token_address.write(token);
        }

        fn set_core_engine_contract(ref self: ContractState, contract: ContractAddress) {
            self.require_owner();
            self.core_engine_contract.write(contract);
        }

        fn set_station_system_contract(ref self: ContractState, contract: ContractAddress) {
            self.require_owner();
            self.station_system_contract.write(contract);
        }

        fn set_merge_system_contract(ref self: ContractState, contract: ContractAddress) {
            self.require_owner();
            self.merge_system_contract.write(contract);
        }

        fn set_reward_distributor_contract(ref self: ContractState, contract: ContractAddress) {
            self.require_owner();
            self.reward_distributor_contract.write(contract);
        }

        fn sync_miner_hash_power(ref self: ContractState, token_id: u256) {
            // Public function for external contracts (like StationSystem) to sync miner hash power
            let miner = self.miners.read(token_id);
            if miner.is_ignited {
                let owner = self.owners.read(token_id);
                self.sync_hash_power_before_change(token_id, owner);
                self.sync_hash_power_after_change(token_id, owner);
            }
        }

        fn pause(ref self: ContractState) {
            self.require_owner();
            self.paused.write(true);
        }

        fn unpause(ref self: ContractState) {
            self.require_owner();
            self.paused.write(false);
        }
    }

    // Internal helper functions
    #[generate_trait]
    impl MinerNFTInternalImpl of MinerNFTInternalTrait {
        fn initialize_tiers(ref self: ContractState) {
            // Basic Tier: 10 TH/s, 10k supply, 75 STRK
            self
                .tier_configs
                .write(
                    'Basic',
                    TierConfig {
                        base_hash_power: 10_000_000_000_000, // 10 TH/s
                        tier_bonus: 0, // No bonus
                        mint_cost_mine: 0,
                        mint_cost_strk: 75_000_000_000_000_000_000, // 75 STRK
                        supply_limit: 10000,
                        minted_count: 0,
                    },
                );

            // Elite Tier: 30 TH/s, 1k supply, 20k MINE, +20% bonus
            self
                .tier_configs
                .write(
                    'Elite',
                    TierConfig {
                        base_hash_power: 30_000_000_000_000, // 30 TH/s
                        tier_bonus: 2000, // 20% bonus
                        mint_cost_mine: 20000_000_000_000_000_000_000, // 20k MINE
                        mint_cost_strk: 0,
                        supply_limit: 1000,
                        minted_count: 0,
                    },
                );

            // Pro Tier: 70 TH/s, 200 supply, merge only, +35% bonus
            self
                .tier_configs
                .write(
                    'Pro',
                    TierConfig {
                        base_hash_power: 70_000_000_000_000, // 70 TH/s
                        tier_bonus: 3500, // 35% bonus
                        mint_cost_mine: 0, // Merge only
                        mint_cost_strk: 0,
                        supply_limit: 200,
                        minted_count: 0,
                    },
                );

            // GIGA Tier: 160 TH/s, 50 supply, merge only, +50% bonus
            self
                .tier_configs
                .write(
                    'GIGA',
                    TierConfig {
                        base_hash_power: 160_000_000_000_000, // 160 TH/s
                        tier_bonus: 5000, // 50% bonus
                        mint_cost_mine: 0, // Merge only
                        mint_cost_strk: 0,
                        supply_limit: 50,
                        minted_count: 0,
                    },
                );
        }

        fn initialize_upgrade_costs(ref self: ContractState) {
            // Upgrade costs in MINE tokens
            self.upgrade_costs.write(0, 2000_000_000_000_000_000_000); // Level 0->1: 2k MINE
            self.upgrade_costs.write(1, 5000_000_000_000_000_000_000); // Level 1->2: 5k MINE
            self.upgrade_costs.write(2, 12000_000_000_000_000_000_000); // Level 2->3: 12k MINE
            self.upgrade_costs.write(3, 35000_000_000_000_000_000_000); // Level 3->4: 35k MINE
            self.upgrade_costs.write(4, 100000_000_000_000_000_000_000); // Level 4->5: 100k MINE
        }

        fn process_payment(
            ref self: ContractState, to: ContractAddress, tier: felt252, config: TierConfig,
        ) {
            // Process MINE token payment if required
            if config.mint_cost_mine > 0 { // TODO: Transfer MINE tokens from user
            }

            // Process STRK token payment if required
            if config.mint_cost_strk > 0 { // TODO: Transfer STRK tokens from user
            }
        }

        fn process_mine_payment(ref self: ContractState, from: ContractAddress, amount: u256) {
            if amount > 0 { // TODO: Transfer MINE tokens from user
            }
        }

        fn get_upgrade_cost(self: @ContractState, current_level: u8) -> u256 {
            if current_level >= 5 {
                return 0; // Max level reached
            }
            self.upgrade_costs.read(current_level)
        }

        fn get_maintenance_cost(self: @ContractState, current_efficiency: u8) -> u256 {
            // Base cost increases as efficiency decreases
            let efficiency_deficit = 100 - current_efficiency;
            50_000_000_000_000_000_000 * efficiency_deficit.into() // 50 MINE per % efficiency lost
        }

        fn get_level_multiplier(self: @ContractState, level: u8) -> u128 {
            // Each level adds 10% to hash power
            100 + (level.into() * 10)
        }

        fn mint(ref self: ContractState, to: ContractAddress, token_id: u256) {
            assert!(!to.is_zero(), "Mint to zero address");
            assert!(self.owners.read(token_id).is_zero(), "Token already exists");

            let balance = self.balances.read(to);
            self.balances.write(to, balance + 1);
            self.owners.write(token_id, to);

            let zero_address: ContractAddress = Zero::zero();
            self.emit(Transfer { from: zero_address, to, token_id });
        }

        fn transfer(
            ref self: ContractState, from: ContractAddress, to: ContractAddress, token_id: u256,
        ) {
            assert!(self.owner_of(token_id) == from, "Transfer from incorrect owner");
            assert!(!to.is_zero(), "Transfer to zero address");

            // Clear approvals
            self.token_approvals.write(token_id, Zero::zero());

            // Update balances
            let from_balance = self.balances.read(from);
            self.balances.write(from, from_balance - 1);
            let to_balance = self.balances.read(to);
            self.balances.write(to, to_balance + 1);

            // Update owner
            self.owners.write(token_id, to);

            self.emit(Transfer { from, to, token_id });
        }

        fn require_minted(self: @ContractState, token_id: u256) {
            assert!(!self.owners.read(token_id).is_zero(), "Token does not exist");
        }

        fn require_approved_or_owner(
            self: @ContractState, spender: ContractAddress, token_id: u256,
        ) {
            let owner = self.owner_of(token_id);
            assert!(
                spender == owner
                    || self.get_approved(token_id) == spender
                    || self.is_approved_for_all(owner, spender),
                "Not approved or owner",
            );
        }

        fn require_owner(self: @ContractState) {
            assert!(get_caller_address() == self.owner.read(), "Not owner");
        }

        fn require_not_paused(self: @ContractState) {
            assert!(!self.paused.read(), "Contract is paused");
        }

        fn update_hash_power_on_ignite(
            ref self: ContractState, token_id: u256, owner: ContractAddress,
        ) {
            let distributor_address = self.reward_distributor_contract.read();
            if !distributor_address.is_zero() {
                let effective_power = self.get_effective_hash_power(token_id);
                let distributor = IRewardDistributorDispatcher {
                    contract_address: distributor_address,
                };
                distributor.add_hash_power(owner, effective_power);
            }
        }


        fn sync_hash_power_before_change(
            ref self: ContractState, token_id: u256, owner: ContractAddress,
        ) {
            // Remove current hash power from RewardDistributor before making changes
            let distributor_address = self.reward_distributor_contract.read();
            if !distributor_address.is_zero() {
                let current_effective_power = self.get_effective_hash_power(token_id);
                if current_effective_power > 0 {
                    let distributor = IRewardDistributorDispatcher {
                        contract_address: distributor_address,
                    };
                    distributor.remove_hash_power(owner, current_effective_power);
                }
            }
        }

        fn sync_hash_power_after_change(
            ref self: ContractState, token_id: u256, owner: ContractAddress,
        ) {
            // Add updated hash power to RewardDistributor after making changes
            let distributor_address = self.reward_distributor_contract.read();
            if !distributor_address.is_zero() {
                let new_effective_power = self.get_effective_hash_power(token_id);
                if new_effective_power > 0 {
                    let distributor = IRewardDistributorDispatcher {
                        contract_address: distributor_address,
                    };
                    distributor.add_hash_power(owner, new_effective_power);
                }
            }
        }
    }
}

// Interface for the MinerNFT contract
#[starknet::interface]
pub trait IMinerNFT<TContractState> {
    // ERC-721 standard
    fn name(self: @TContractState) -> felt252;
    fn symbol(self: @TContractState) -> felt252;
    fn balance_of(self: @TContractState, owner: starknet::ContractAddress) -> u256;
    fn owner_of(self: @TContractState, token_id: u256) -> starknet::ContractAddress;
    fn get_approved(self: @TContractState, token_id: u256) -> starknet::ContractAddress;
    fn is_approved_for_all(
        self: @TContractState,
        owner: starknet::ContractAddress,
        operator: starknet::ContractAddress,
    ) -> bool;
    fn approve(ref self: TContractState, to: starknet::ContractAddress, token_id: u256);
    fn set_approval_for_all(
        ref self: TContractState, operator: starknet::ContractAddress, approved: bool,
    );
    fn transfer_from(
        ref self: TContractState,
        from: starknet::ContractAddress,
        to: starknet::ContractAddress,
        token_id: u256,
    );

    // Miner NFT specific functions
    fn mint_miner(ref self: TContractState, to: starknet::ContractAddress, tier: felt252) -> u256;
    fn ignite_miner(ref self: TContractState, token_id: u256, core_engine_id: u256);
    fn extinguish_miner(ref self: TContractState, token_id: u256);
    fn upgrade_miner(ref self: TContractState, token_id: u256);
    fn maintain_miner(ref self: TContractState, token_id: u256);
    fn get_miner_info(self: @TContractState, token_id: u256) -> MinerNFT::MinerInfo;
    fn get_effective_hash_power(self: @TContractState, token_id: u256) -> u128;
    fn get_current_efficiency(self: @TContractState, token_id: u256) -> u8;
    fn get_tier_config(self: @TContractState, tier: felt252) -> MinerNFT::TierConfig;

    // Burn function for merge system
    fn burn(ref self: TContractState, token_id: u256);

    // Admin functions
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn set_core_engine_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn set_station_system_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn set_merge_system_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn set_reward_distributor_contract(
        ref self: TContractState, contract: starknet::ContractAddress,
    );
    fn sync_miner_hash_power(ref self: TContractState, token_id: u256);
    fn pause(ref self: TContractState);
    fn unpause(ref self: TContractState);
}
