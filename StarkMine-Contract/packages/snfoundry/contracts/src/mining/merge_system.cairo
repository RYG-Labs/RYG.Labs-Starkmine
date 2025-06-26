// Interface for the MergeSystem contract
#[starknet::interface]
pub trait IMergeSystem<TContractState> {
    fn attempt_merge(
        ref self: TContractState,
        token_id_1: u256,
        token_id_2: u256,
        from_tier: felt252,
        to_tier: felt252,
    ) -> (bool, u256);
    fn get_merge_config(
        self: @TContractState, from_tier: felt252, to_tier: felt252,
    ) -> MergeSystem::MergeConfig;
    fn get_user_merge_history(
        self: @TContractState,
        user: starknet::ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
    ) -> MergeSystem::MergeHistory;
    fn get_current_success_rate(
        self: @TContractState,
        user: starknet::ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
    ) -> u128;
    fn can_attempt_merge(
        self: @TContractState,
        user: starknet::ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
    ) -> bool;
    fn get_global_stats(self: @TContractState) -> (u256, u256);

    // Admin functions
    fn update_merge_config(
        ref self: TContractState,
        from_tier: felt252,
        to_tier: felt252,
        config: MergeSystem::MergeConfig,
    );
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn set_miner_nft_contract(ref self: TContractState, contract: starknet::ContractAddress);
    fn pause(ref self: TContractState);
    fn unpause(ref self: TContractState);
}

#[starknet::contract]
mod MergeSystem {
    use core::cmp::min;
    use starkmine::mining::merge_system::IMergeSystem;
    use starkmine::nft::miner_nft::{IMinerNFTDispatcher, IMinerNFTDispatcherTrait};
    use starkmine::token::interface::{IMineTokenDispatcher, IMineTokenDispatcherTrait};
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address, get_contract_address};

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct MergeConfig {
        pub base_success_rate: u128, // Base success rate (in basis points)
        pub failure_bonus: u128, // Bonus per failure (in basis points)  
        pub max_failure_bonus: u128, // Maximum failure bonus
        pub cost_mine: u256, // Cost in MINE tokens
        pub cost_strk: u256 // Cost in STRK tokens
    }

    #[derive(Drop, Serde, starknet::Store, Copy)]
    pub struct MergeHistory {
        pub attempts: u32, // Total attempts made
        pub failures: u32, // Number of failures
        pub current_bonus: u128 // Current failure bonus
    }

    #[storage]
    struct Storage {
        // Merge configurations for different tier combinations
        merge_configs: Map<(felt252, felt252), MergeConfig>, // (from_tier, to_tier) -> config
        // User merge history per tier combination
        user_merge_history: Map<(ContractAddress, felt252, felt252), MergeHistory>,
        // Global merge statistics
        total_merge_attempts: u256,
        successful_merges: u256,
        // Contract addresses
        mine_token_address: ContractAddress,
        strk_token_address: ContractAddress,
        miner_nft_contract: ContractAddress,
        // Randomness source (simple implementation)
        nonce: u256,
        // Admin
        owner: ContractAddress,
        paused: bool,
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        MergeAttempted: MergeAttempted,
        MergeSuccessful: MergeSuccessful,
        MergeFailed: MergeFailed,
        ConfigUpdated: ConfigUpdated,
    }

    #[derive(Drop, starknet::Event)]
    struct MergeAttempted {
        #[key]
        user: ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
        source_token_1: u256,
        source_token_2: u256,
        success_rate: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct MergeSuccessful {
        #[key]
        user: ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
        new_token_id: u256,
        final_success_rate: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct MergeFailed {
        #[key]
        user: ContractAddress,
        from_tier: felt252,
        to_tier: felt252,
        new_failure_bonus: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct ConfigUpdated {
        from_tier: felt252,
        to_tier: felt252,
        new_base_rate: u128,
    }

    #[constructor]
    fn constructor(
        ref self: ContractState,
        owner: ContractAddress,
        mine_token: ContractAddress,
        strk_token: ContractAddress,
        miner_nft: ContractAddress,
    ) {
        self.owner.write(owner);
        self.mine_token_address.write(mine_token);
        self.strk_token_address.write(strk_token);
        self.miner_nft_contract.write(miner_nft);
        self.nonce.write(1);

        // Initialize merge configurations
        self.initialize_merge_configs();
    }

    #[abi(embed_v0)]
    impl MergeSystemImpl of IMergeSystem<ContractState> {
        fn attempt_merge(
            ref self: ContractState,
            token_id_1: u256,
            token_id_2: u256,
            from_tier: felt252,
            to_tier: felt252,
        ) -> (bool, u256) {
            self.require_not_paused();
            let caller = get_caller_address();

            // Validate merge configuration exists
            let config = self.merge_configs.read((from_tier, to_tier));
            assert!(config.base_success_rate > 0, "Invalid merge combination");

            // Get user history
            let user_history = self.user_merge_history.read((caller, from_tier, to_tier));

            // Validate ownership and tiers
            self.validate_merge_requirements(caller, token_id_1, token_id_2, from_tier);

            // Process payment
            self.process_payments(caller, config);

            // Calculate success rate with failure bonus
            let success_rate = min(
                config.base_success_rate + user_history.current_bonus,
                config.base_success_rate + config.max_failure_bonus,
            );

            // Generate random number for success/failure determination
            let random_value = self.generate_random();
            let is_success = random_value % 10000 < success_rate;

            // Update global statistics
            self.total_merge_attempts.write(self.total_merge_attempts.read() + 1);

            // Emit attempt event
            self
                .emit(
                    MergeAttempted {
                        user: caller,
                        from_tier,
                        to_tier,
                        source_token_1: token_id_1,
                        source_token_2: token_id_2,
                        success_rate,
                    },
                );

            // Burn both source tokens regardless of success/failure
            // TODO: Implement actual burning when NFT interface is available
            // For now, this is a placeholder that should:
            // 1. Call miner_nft_contract.burn(token_id_1)
            // 2. Call miner_nft_contract.burn(token_id_2)
            self.burn_source_tokens(token_id_1, token_id_2);

            if is_success {
                // Success: Mint new token of target tier
                let new_token_id = self.mint_merged_token(caller, to_tier);

                // Reset failure bonus on success
                let mut updated_history = user_history;
                updated_history.attempts += 1;
                updated_history.current_bonus = 0;
                self.user_merge_history.write((caller, from_tier, to_tier), updated_history);

                // Update global success count
                self.successful_merges.write(self.successful_merges.read() + 1);

                self
                    .emit(
                        MergeSuccessful {
                            user: caller,
                            from_tier,
                            to_tier,
                            new_token_id,
                            final_success_rate: success_rate,
                        },
                    );

                (true, new_token_id)
            } else {
                // Failure: Increase failure bonus for next attempt
                let new_bonus = min(
                    user_history.current_bonus + config.failure_bonus, config.max_failure_bonus,
                );

                let mut updated_history = user_history;
                updated_history.attempts += 1;
                updated_history.failures += 1;
                updated_history.current_bonus = new_bonus;
                self.user_merge_history.write((caller, from_tier, to_tier), updated_history);

                self
                    .emit(
                        MergeFailed {
                            user: caller, from_tier, to_tier, new_failure_bonus: new_bonus,
                        },
                    );

                (false, 0)
            }
        }

        fn get_merge_config(
            self: @ContractState, from_tier: felt252, to_tier: felt252,
        ) -> MergeConfig {
            self.merge_configs.read((from_tier, to_tier))
        }

        fn get_user_merge_history(
            self: @ContractState, user: ContractAddress, from_tier: felt252, to_tier: felt252,
        ) -> MergeHistory {
            self.user_merge_history.read((user, from_tier, to_tier))
        }

        fn get_current_success_rate(
            self: @ContractState, user: ContractAddress, from_tier: felt252, to_tier: felt252,
        ) -> u128 {
            let config = self.merge_configs.read((from_tier, to_tier));
            let history = self.user_merge_history.read((user, from_tier, to_tier));

            min(
                config.base_success_rate + history.current_bonus,
                config.base_success_rate + config.max_failure_bonus,
            )
        }

        fn can_attempt_merge(
            self: @ContractState, user: ContractAddress, from_tier: felt252, to_tier: felt252,
        ) -> bool {
            let config = self.merge_configs.read((from_tier, to_tier));
            config.base_success_rate > 0 // Simply check if valid combination exists
        }

        fn get_global_stats(self: @ContractState) -> (u256, u256) {
            (self.total_merge_attempts.read(), self.successful_merges.read())
        }

        // Admin functions
        fn update_merge_config(
            ref self: ContractState, from_tier: felt252, to_tier: felt252, config: MergeConfig,
        ) {
            self.require_owner();
            self.merge_configs.write((from_tier, to_tier), config);

            self
                .emit(
                    ConfigUpdated { from_tier, to_tier, new_base_rate: config.base_success_rate },
                );
        }

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
    impl MergeSystemInternalImpl of MergeSystemInternalTrait {
        fn initialize_merge_configs(ref self: ContractState) {
            // Elite -> Pro: 50% base success rate, +1% per failure (max +20%)
            self
                .merge_configs
                .write(
                    ('Elite', 'Pro'),
                    MergeConfig {
                        base_success_rate: 5000, // 50%
                        failure_bonus: 100, // +1% per failure
                        max_failure_bonus: 2000, // Max +20%
                        cost_mine: 1000_000_000_000_000_000_000, // 1k MINE
                        cost_strk: 125_000_000_000_000_000_000 // 125 STRK
                    },
                );

            // Pro -> GIGA: 25% base success rate, +1% per failure (max +50%)
            self
                .merge_configs
                .write(
                    ('Pro', 'GIGA'),
                    MergeConfig {
                        base_success_rate: 2500, // 25%
                        failure_bonus: 100, // +1% per failure
                        max_failure_bonus: 5000, // Max +50%
                        cost_mine: 5000_000_000_000_000_000_000, // 5k MINE
                        cost_strk: 125_000_000_000_000_000_000 // 125 STRK
                    },
                );
        }

        fn process_payments(ref self: ContractState, from: ContractAddress, config: MergeConfig) {
            // Get token contract addresses
            let mine_token_address = self.mine_token_address.read();
            let contract_address = get_contract_address();

            // TODO: Process MINE token payment
            if config.cost_mine > 0 {// let mine_token = IMineTokenDispatcher { contract_address: mine_token_address };
            // let success = mine_token.transfer_from(from, contract_address, config.cost_mine);
            // assert!(success, "MINE token transfer failed");
            }

            // Process STRK token payment - Note: Using MINE token interface for now
            // In production, you'd use a separate STRK token interface
            if config
                .cost_strk > 0 { // TODO: Implement STRK token transfer when STRK token contract is available
            // For now, we'll skip STRK payment or treat it as MINE tokens
            // let strk_token = IERC20Dispatcher { contract_address: strk_token_address };
            // let success = strk_token.transfer_from(from, contract_address, config.cost_strk);
            // assert!(success, "STRK token transfer failed");
            }
        }

        fn mint_merged_token(ref self: ContractState, to: ContractAddress, tier: felt252) -> u256 {
            // Get the NFT contract dispatcher
            let nft_contract = IMinerNFTDispatcher {
                contract_address: self.miner_nft_contract.read(),
            };

            // Mint new token of target tier
            let new_token_id = nft_contract.mint_miner(to, tier);
            new_token_id
        }

        fn burn_source_tokens(ref self: ContractState, token_id_1: u256, token_id_2: u256) {
            // Get the NFT contract dispatcher
            let nft_contract = IMinerNFTDispatcher {
                contract_address: self.miner_nft_contract.read(),
            };

            // Burn both source tokens
            nft_contract.burn(token_id_1);
            nft_contract.burn(token_id_2);
        }

        fn generate_random(ref self: ContractState) -> u128 {
            // Simple pseudo-random number generation
            // In production, should use a more secure randomness source
            let current_nonce = self.nonce.read();
            let block_number = get_block_number().into();
            let caller = get_caller_address();

            let caller_felt: felt252 = caller.into();
            let combined = current_nonce + block_number + caller_felt.into();
            self.nonce.write(current_nonce + 1);

            // Return last 4 digits as random value for percentage calculation
            (combined % 10000).try_into().unwrap()
        }

        fn require_owner(self: @ContractState) {
            assert!(get_caller_address() == self.owner.read(), "Not owner");
        }

        fn require_not_paused(self: @ContractState) {
            assert!(!self.paused.read(), "Contract is paused");
        }

        fn validate_merge_requirements(
            ref self: ContractState,
            caller: ContractAddress,
            token_id_1: u256,
            token_id_2: u256,
            from_tier: felt252,
        ) {
            // Basic validation to prevent errors
            assert!(token_id_1 != token_id_2, "Cannot merge same token");
            assert!(token_id_1 > 0, "Invalid token ID 1");
            assert!(token_id_2 > 0, "Invalid token ID 2");

            // Get NFT contract dispatcher
            let nft = IMinerNFTDispatcher { contract_address: self.miner_nft_contract.read() };

            // Validate ownership
            assert!(nft.owner_of(token_id_1) == caller, "Not owner of token 1");
            assert!(nft.owner_of(token_id_2) == caller, "Not owner of token 2");

            // Validate tiers and states
            let info1 = nft.get_miner_info(token_id_1);
            let info2 = nft.get_miner_info(token_id_2);
            assert!(info1.tier == from_tier, "Token 1 wrong tier");
            assert!(info2.tier == from_tier, "Token 2 wrong tier");
            assert!(!info1.is_ignited, "Token 1 is active");
            assert!(!info2.is_ignited, "Token 2 is active");
        }
    }
}
