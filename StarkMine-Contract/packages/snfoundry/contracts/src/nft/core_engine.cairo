// The core engine NFT implementation using the centralized interface

#[starknet::contract]
mod CoreEngine {
    use core::cmp::min;
    use core::num::traits::Zero;
    use starkmine::interfaces::icore_engine::{EngineInfo, EngineTypeConfig, ICoreEngine};
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address};

    #[storage]
    struct Storage {
        // ERC-721 Standard
        name: felt252,
        symbol: felt252,
        owners: Map<u256, ContractAddress>,
        balances: Map<ContractAddress, u256>,
        token_approvals: Map<u256, ContractAddress>,
        operator_approvals: Map<(ContractAddress, ContractAddress), bool>,
        // Core Engine specific
        engines: Map<u256, EngineInfo>,
        engine_type_configs: Map<felt252, EngineTypeConfig>,
        next_token_id: u256,
        // Contract addresses
        mine_token_address: ContractAddress,
        miner_nft_contract: ContractAddress,
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
        EngineMinted: EngineMinted,
        EngineAttached: EngineAttached,
        EngineDetached: EngineDetached,
        EngineRepaired: EngineRepaired,
        EngineExpired: EngineExpired,
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
    struct EngineMinted {
        #[key]
        owner: ContractAddress,
        #[key]
        token_id: u256,
        engine_type: felt252,
        efficiency_bonus: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct EngineAttached {
        #[key]
        engine_id: u256,
        #[key]
        miner_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct EngineDetached {
        #[key]
        engine_id: u256,
        #[key]
        miner_id: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct EngineRepaired {
        #[key]
        engine_id: u256,
        cost: u256,
        durability_restored: u64,
    }

    #[derive(Drop, starknet::Event)]
    struct EngineExpired {
        #[key]
        engine_id: u256,
    }

    #[constructor]
    fn constructor(
        ref self: ContractState,
        name: felt252,
        symbol: felt252,
        owner: ContractAddress,
        mine_token: ContractAddress,
    ) {
        self.name.write(name);
        self.symbol.write(symbol);
        self.owner.write(owner);
        self.mine_token_address.write(mine_token);
        self.next_token_id.write(1);

        // Initialize engine type configurations
        self.initialize_engine_types();
    }

    #[abi(embed_v0)]
    impl CoreEngineImpl of ICoreEngine<ContractState> {
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

            // Ensure engine is not currently attached to a miner
            let engine = self.engines.read(token_id);
            assert!(engine.attached_miner == 0, "Cannot transfer attached engine");

            self.transfer(from, to, token_id);
        }

        // Core Engine specific functions
        fn mint_engine(ref self: ContractState, to: ContractAddress, engine_type: felt252) -> u256 {
            self.require_not_paused();
            let config = self.engine_type_configs.read(engine_type);
            assert!(config.efficiency_bonus > 0, "Invalid engine type");

            // Process payment
            self.process_payment(to, config.mint_cost);

            let token_id = self.next_token_id.read();

            let engine_info = EngineInfo {
                engine_type,
                efficiency_bonus: config.efficiency_bonus,
                durability: config.durability,
                blocks_used: 0,
                last_used_block: 0,
                attached_miner: 0,
                is_active: false,
            };

            self.mint(to, token_id);
            self.engines.write(token_id, engine_info);
            self.next_token_id.write(token_id + 1);

            self
                .emit(
                    EngineMinted {
                        owner: to, token_id, engine_type, efficiency_bonus: config.efficiency_bonus,
                    },
                );

            token_id
        }

        fn attach_to_miner(
            ref self: ContractState, engine_id: u256, miner_id: u256, owner: ContractAddress,
        ) {
            self.require_not_paused();
            let caller = get_caller_address();

            // Only allow calls from the MinerNFT contract
            assert!(
                caller == self.miner_nft_contract.read(), "Only miner contract can attach engines",
            );

            // Check that the provided owner actually owns the engine
            self.require_approved_or_owner(owner, engine_id);

            let mut engine = self.engines.read(engine_id);
            assert!(engine.attached_miner == 0, "Engine already attached");
            assert!(!self.is_engine_expired(engine_id), "Engine expired");

            // TODO: Verify miner ownership and validate miner contract
            // This would interact with the MinerNFT contract

            engine.attached_miner = miner_id;
            engine.is_active = true;
            engine.last_used_block = get_block_number().try_into().unwrap();
            self.engines.write(engine_id, engine);

            self.emit(EngineAttached { engine_id, miner_id });
        }

        fn detach_from_miner(ref self: ContractState, engine_id: u256, owner: ContractAddress) {
            self.require_not_paused();
            let caller = get_caller_address();

            // Only allow calls from the MinerNFT contract
            assert!(
                caller == self.miner_nft_contract.read(), "Only miner contract can detach engines",
            );

            // Check that the provided owner actually owns the engine
            self.require_approved_or_owner(owner, engine_id);

            let mut engine = self.engines.read(engine_id);
            assert!(engine.attached_miner > 0, "Engine not attached");

            let miner_id = engine.attached_miner;

            // Update usage blocks
            if engine.is_active {
                let current_block = get_block_number().try_into().unwrap();
                let blocks_active = current_block - engine.last_used_block;
                engine.blocks_used += blocks_active;
            }

            engine.attached_miner = 0;
            engine.is_active = false;
            self.engines.write(engine_id, engine);

            self.emit(EngineDetached { engine_id, miner_id });
        }

        fn repair_engine(ref self: ContractState, engine_id: u256, durability_to_restore: u64) {
            self.require_not_paused();
            let caller = get_caller_address();
            self.require_approved_or_owner(caller, engine_id);

            let mut engine = self.engines.read(engine_id);
            let config = self.engine_type_configs.read(engine.engine_type);

            let max_restorable = min(engine.blocks_used, durability_to_restore);
            let repair_cost = self.calculate_repair_cost(config.repair_cost_base, max_restorable);

            // Process payment
            self.process_payment(caller, repair_cost);

            engine.blocks_used -= max_restorable;
            self.engines.write(engine_id, engine);

            self
                .emit(
                    EngineRepaired {
                        engine_id, cost: repair_cost, durability_restored: max_restorable,
                    },
                );
        }

        fn get_engine_info(self: @ContractState, engine_id: u256) -> EngineInfo {
            self.require_minted(engine_id);
            self.engines.read(engine_id)
        }

        fn get_engine_remaining_durability(self: @ContractState, engine_id: u256) -> u64 {
            let engine = self.engines.read(engine_id);
            if engine.durability > engine.blocks_used {
                engine.durability - engine.blocks_used
            } else {
                0
            }
        }

        fn is_engine_expired(self: @ContractState, engine_id: u256) -> bool {
            self.get_engine_remaining_durability(engine_id) == 0
        }

        fn get_current_efficiency_bonus(self: @ContractState, engine_id: u256) -> u128 {
            let engine = self.engines.read(engine_id);
            if self.is_engine_expired(engine_id) {
                return 0;
            }

            // Efficiency degrades as engine wears out
            let remaining_durability = self.get_engine_remaining_durability(engine_id);
            let durability_percentage = (remaining_durability * 100) / engine.durability;

            // Efficiency drops to 50% when durability is at 0%
            let efficiency_multiplier = 50 + (durability_percentage / 2);
            (engine.efficiency_bonus * efficiency_multiplier.into()) / 100
        }

        fn update_engine_usage(ref self: ContractState, engine_id: u256) {
            let caller = get_caller_address();
            assert!(
                caller == self.miner_nft_contract.read(), "Only miner contract can update usage",
            );

            let mut engine = self.engines.read(engine_id);
            if engine.is_active {
                let current_block = get_block_number().try_into().unwrap();
                let blocks_since_last_update = current_block - engine.last_used_block;

                engine.blocks_used += blocks_since_last_update;
                engine.last_used_block = current_block;

                if engine.blocks_used >= engine.durability {
                    engine.is_active = false;
                    self.emit(EngineExpired { engine_id });
                }

                self.engines.write(engine_id, engine);
            }
        }

        fn get_engine_type_config(self: @ContractState, engine_type: felt252) -> EngineTypeConfig {
            self.engine_type_configs.read(engine_type)
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
    impl CoreEngineInternalImpl of CoreEngineInternalTrait {
        fn initialize_engine_types(ref self: ContractState) {
            // Standard Engine: 20% efficiency bonus, 30 days durability
            let blocks_per_month = 30
                * 24
                * 60
                * 20; // 30 days * 24 hours * 60 minutes * 20 blocks/minute
            self
                .engine_type_configs
                .write(
                    'Standard',
                    EngineTypeConfig {
                        efficiency_bonus: 2000, // 20%
                        durability: blocks_per_month,
                        mint_cost: 500_000_000_000_000_000_000, // 500 MINE
                        repair_cost_base: 50_000_000_000_000_000_000 // 50 MINE base
                    },
                );

            // Premium Engine: 35% efficiency bonus, 45 days durability
            self
                .engine_type_configs
                .write(
                    'Premium',
                    EngineTypeConfig {
                        efficiency_bonus: 3500, // 35%
                        durability: (blocks_per_month * 45) / 30,
                        mint_cost: 1500_000_000_000_000_000_000, // 1500 MINE
                        repair_cost_base: 100_000_000_000_000_000_000 // 100 MINE base
                    },
                );

            // Elite Engine: 50% efficiency bonus, 60 days durability
            self
                .engine_type_configs
                .write(
                    'Elite',
                    EngineTypeConfig {
                        efficiency_bonus: 5000, // 50%
                        durability: blocks_per_month * 2,
                        mint_cost: 3000_000_000_000_000_000_000, // 3000 MINE
                        repair_cost_base: 200_000_000_000_000_000_000 // 200 MINE base
                    },
                );
        }

        fn process_payment(ref self: ContractState, from: ContractAddress, amount: u256) {
            if amount > 0 { // TODO: Transfer MINE tokens from user
            // This would interact with the MINE token contract
            }
        }

        fn calculate_repair_cost(
            self: @ContractState, base_cost: u256, blocks_to_repair: u64,
        ) -> u256 {
            // Cost increases with the amount of durability to restore
            let blocks_cost = base_cost * blocks_to_repair.into() / 100;
            base_cost + blocks_cost
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
    }
}

