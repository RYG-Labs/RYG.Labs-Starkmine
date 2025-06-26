#[starknet::contract]
mod RewardDistributor {
    use core::num::traits::Zero;
    use starkmine::mining::interface::{IRewardDistributor, IRewardDistributorValidation};
    use starkmine::token::interface::{IMineTokenDispatcher, IMineTokenDispatcherTrait};
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address, get_contract_address};

    // Constants for precision handling
    const PRECISION_SCALE: u256 = 1000000000000000000; // 1e18
    const MAX_BURN_RATE: u128 = 10000; // 100% in basis points
    const DEFAULT_REWARD_PER_BLOCK: u256 = 1000000000000000000000; // 1000 MINE per block

    #[storage]
    struct Storage {
        // Core state
        mine_token_address: ContractAddress,
        miner_nft_address: ContractAddress,
        total_hash_power: u128,
        user_hash_power: Map<ContractAddress, u128>,
        user_rewards_per_token_paid: Map<ContractAddress, u256>,
        user_pending_rewards: Map<ContractAddress, u256>,
        // Distribution parameters
        reward_per_block: u256,
        last_update_block: u64,
        rewards_per_token_stored: u256,
        burn_rate: u128, // Basis points (10000 = 100%)
        // Access control
        owner: ContractAddress,
        authorized_contracts: Map<ContractAddress, bool>,
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        HashPowerAdded: HashPowerAdded,
        HashPowerRemoved: HashPowerRemoved,
        RewardsDistributed: RewardsDistributed,
        RewardsClaimed: RewardsClaimed,
        TokensBurned: TokensBurned,
        MineTokenAddressSet: MineTokenAddressSet,
        MinerNFTAddressSet: MinerNFTAddressSet,
        BurnRateSet: BurnRateSet,
        RewardPerBlockSet: RewardPerBlockSet,
        ContractAuthorized: ContractAuthorized,
    }

    #[derive(Drop, starknet::Event)]
    struct HashPowerAdded {
        user: ContractAddress,
        hash_power: u128,
        total_hash_power: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct HashPowerRemoved {
        user: ContractAddress,
        hash_power: u128,
        total_hash_power: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct RewardsDistributed {
        block_number: u64,
        total_rewards: u256,
        rewards_per_token: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct RewardsClaimed {
        user: ContractAddress,
        amount: u256,
        burned_amount: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct TokensBurned {
        amount: u256,
        total_supply_after: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct MineTokenAddressSet {
        old_address: ContractAddress,
        new_address: ContractAddress,
    }

    #[derive(Drop, starknet::Event)]
    struct MinerNFTAddressSet {
        old_address: ContractAddress,
        new_address: ContractAddress,
    }

    #[derive(Drop, starknet::Event)]
    struct BurnRateSet {
        old_rate: u128,
        new_rate: u128,
    }

    #[derive(Drop, starknet::Event)]
    struct RewardPerBlockSet {
        old_reward: u256,
        new_reward: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct ContractAuthorized {
        contract_address: ContractAddress,
        authorized: bool,
    }

    #[constructor]
    fn constructor(ref self: ContractState, owner: ContractAddress, burn_rate: u128) {
        assert!(burn_rate <= MAX_BURN_RATE, "Invalid burn rate");
        self.owner.write(owner);
        self.burn_rate.write(burn_rate);
        self.last_update_block.write(get_block_number().try_into().unwrap());
        self.rewards_per_token_stored.write(0);
        self.reward_per_block.write(DEFAULT_REWARD_PER_BLOCK);
    }

    #[abi(embed_v0)]
    impl RewardDistributorImpl of IRewardDistributor<ContractState> {
        fn total_hash_power(self: @ContractState) -> u128 {
            self.total_hash_power.read()
        }

        fn user_hash_power(self: @ContractState, user: ContractAddress) -> u128 {
            self.user_hash_power.read(user)
        }

        fn pending_rewards(self: @ContractState, user: ContractAddress) -> u256 {
            let current_rewards_per_token = self.calculate_rewards_per_token();
            let user_hash = self.user_hash_power.read(user);
            let rewards_per_token_paid = self.user_rewards_per_token_paid.read(user);
            let pending = self.user_pending_rewards.read(user);

            // Calculate new rewards since last update
            let new_rewards = (user_hash.into()
                * (current_rewards_per_token - rewards_per_token_paid))
                / PRECISION_SCALE;

            pending + new_rewards
        }

        fn mine_token_address(self: @ContractState) -> ContractAddress {
            self.mine_token_address.read()
        }

        fn set_mine_token_address(ref self: ContractState, token: ContractAddress) {
            self.require_owner();
            let old_address = self.mine_token_address.read();
            self.mine_token_address.write(token);
            self.emit(MineTokenAddressSet { old_address, new_address: token });
        }

        fn last_update_block(self: @ContractState) -> u64 {
            self.last_update_block.read()
        }

        fn burn_rate(self: @ContractState) -> u128 {
            self.burn_rate.read()
        }

        fn set_burn_rate(ref self: ContractState, rate: u128) {
            self.require_owner();
            assert!(rate <= MAX_BURN_RATE, "Burn rate cannot exceed 100%");
            let old_rate = self.burn_rate.read();
            self.burn_rate.write(rate);
            self.emit(BurnRateSet { old_rate, new_rate: rate });
        }

        fn reward_per_block(self: @ContractState) -> u256 {
            // Get reward per block from MineToken contract if available, otherwise use stored value
            let mine_token_addr = self.mine_token_address.read();
            if !mine_token_addr.is_zero() {
                let mine_token = IMineTokenDispatcher { contract_address: mine_token_addr };
                mine_token.reward_per_block()
            } else {
                self.reward_per_block.read()
            }
        }

        fn set_reward_per_block(ref self: ContractState, reward: u256) {
            self.require_owner();
            let old_reward = self.reward_per_block.read();
            self.reward_per_block.write(reward);
            self.emit(RewardPerBlockSet { old_reward, new_reward: reward });
        }

        fn add_hash_power(ref self: ContractState, user: ContractAddress, hash_power: u128) {
            // Only callable by authorized contracts (MinerNFT)
            self.require_authorized_caller();
            self.update_rewards(user);

            let current_power = self.user_hash_power.read(user);
            let new_power = current_power + hash_power;

            self.user_hash_power.write(user, new_power);

            let total = self.total_hash_power.read() + hash_power;
            self.total_hash_power.write(total);

            self.emit(HashPowerAdded { user, hash_power, total_hash_power: total });
        }

        fn remove_hash_power(ref self: ContractState, user: ContractAddress, hash_power: u128) {
            // Only callable by authorized contracts (MinerNFT)
            self.require_authorized_caller();
            self.update_rewards(user);

            let current_power = self.user_hash_power.read(user);
            assert!(current_power >= hash_power, "Insufficient hash power");

            let new_power = current_power - hash_power;
            self.user_hash_power.write(user, new_power);

            let total = self.total_hash_power.read() - hash_power;
            self.total_hash_power.write(total);

            self.emit(HashPowerRemoved { user, hash_power, total_hash_power: total });
        }

        fn distribute(ref self: ContractState) {
            let current_block = get_block_number().try_into().unwrap();
            let last_block = self.last_update_block.read();

            if current_block <= last_block {
                return; // No new blocks to distribute
            }

            let total_hash = self.total_hash_power.read();
            if total_hash == 0 {
                self.last_update_block.write(current_block);
                return; // No hash power to distribute to
            }

            // Calculate total rewards for the period
            let blocks_elapsed = current_block - last_block;
            let reward_per_block = self.reward_per_block();
            let total_rewards = reward_per_block * blocks_elapsed.into();

            // Update rewards per token
            let new_rewards_per_token = self
                .calculate_rewards_per_token_with_new_rewards(total_rewards);
            self.rewards_per_token_stored.write(new_rewards_per_token);
            self.last_update_block.write(current_block);

            self
                .emit(
                    RewardsDistributed {
                        block_number: current_block,
                        total_rewards,
                        rewards_per_token: new_rewards_per_token,
                    },
                );
        }

        fn claim_rewards(ref self: ContractState, user: ContractAddress) -> u256 {
            self.update_rewards(user);

            let pending = self.user_pending_rewards.read(user);
            if pending == 0 {
                return 0;
            }

            // Reset pending rewards
            self.user_pending_rewards.write(user, 0);

            // Calculate burn amount
            let burn_rate = self.burn_rate.read();
            let burn_amount = (pending * burn_rate.into()) / MAX_BURN_RATE.into();
            let user_amount = pending - burn_amount;

            // Mint tokens to user and burn amount
            let mine_token = IMineTokenDispatcher {
                contract_address: self.mine_token_address.read(),
            };

            if user_amount > 0 {
                mine_token.mint(user, user_amount);
            }

            if burn_amount > 0 {
                // Mint to contract and then burn
                let contract_address = get_contract_address();
                mine_token.mint(contract_address, burn_amount);
                mine_token.burn(burn_amount);

                self
                    .emit(
                        TokensBurned {
                            amount: burn_amount, total_supply_after: mine_token.total_supply(),
                        },
                    );
            }

            self.emit(RewardsClaimed { user, amount: user_amount, burned_amount: burn_amount });

            user_amount
        }

        fn burn(ref self: ContractState, amount: u256) -> u256 {
            // Additional burn function for manual burns
            self.require_owner();
            let mine_token = IMineTokenDispatcher {
                contract_address: self.mine_token_address.read(),
            };
            mine_token.burn(amount);

            self.emit(TokensBurned { amount, total_supply_after: mine_token.total_supply() });
            amount
        }

        fn set_miner_nft_address(ref self: ContractState, nft_address: ContractAddress) {
            self.require_owner();
            let old_address = self.miner_nft_address.read();
            self.miner_nft_address.write(nft_address);
            self.emit(MinerNFTAddressSet { old_address, new_address: nft_address });
        }

        // FIXED: Expose the authorize_contract function publicly
        fn authorize_contract(
            ref self: ContractState, contract_address: ContractAddress, authorized: bool,
        ) {
            self.require_owner();
            self.authorized_contracts.write(contract_address, authorized);
            self.emit(ContractAuthorized { contract_address, authorized });
        }
    }

    #[abi(embed_v0)]
    impl RewardDistributorValidationImpl of IRewardDistributorValidation<ContractState> {
        fn check_valid_token(self: @ContractState) {
            assert!(!self.mine_token_address.read().is_zero(), "Token not set");
        }

        fn check_valid_burn_rate(self: @ContractState, rate: u128) {
            assert!(rate <= MAX_BURN_RATE, "Invalid burn rate");
        }

        fn check_valid_hash_power(self: @ContractState, user: ContractAddress, hash_power: u128) {
            let current = self.user_hash_power.read(user);
            assert!(current >= hash_power, "Insufficient hash power");
        }
    }

    // Internal helper functions
    #[generate_trait]
    impl RewardDistributorInternalImpl of RewardDistributorInternalTrait {
        fn require_owner(self: @ContractState) {
            assert!(get_caller_address() == self.owner.read(), "Not owner");
        }

        fn require_authorized_caller(self: @ContractState) {
            let caller = get_caller_address();
            let miner_nft = self.miner_nft_address.read();
            let is_authorized = self.authorized_contracts.read(caller);
            // Allow owner, authorized contracts, or miner NFT contract
            assert!(
                caller == self.owner.read() || caller == miner_nft || is_authorized,
                "Not authorized",
            );
        }

        fn update_rewards(ref self: ContractState, user: ContractAddress) {
            let current_rewards_per_token = self.calculate_rewards_per_token();
            self.rewards_per_token_stored.write(current_rewards_per_token);

            let user_hash = self.user_hash_power.read(user);
            if user_hash > 0 {
                let rewards_per_token_paid = self.user_rewards_per_token_paid.read(user);
                let pending = self.user_pending_rewards.read(user);

                let new_rewards = (user_hash.into()
                    * (current_rewards_per_token - rewards_per_token_paid))
                    / PRECISION_SCALE;
                self.user_pending_rewards.write(user, pending + new_rewards);
            }

            self.user_rewards_per_token_paid.write(user, current_rewards_per_token);
        }

        fn calculate_rewards_per_token(self: @ContractState) -> u256 {
            let total_hash = self.total_hash_power.read();
            if total_hash == 0 {
                return self.rewards_per_token_stored.read();
            }

            let current_block = get_block_number().try_into().unwrap();
            let last_block = self.last_update_block.read();
            let blocks_elapsed = current_block - last_block;

            let reward_per_block = self.reward_per_block();
            let total_new_rewards = reward_per_block * blocks_elapsed.into();

            self.calculate_rewards_per_token_with_new_rewards(total_new_rewards)
        }

        fn calculate_rewards_per_token_with_new_rewards(
            self: @ContractState, new_rewards: u256,
        ) -> u256 {
            let total_hash = self.total_hash_power.read();
            if total_hash == 0 {
                return self.rewards_per_token_stored.read();
            }

            let current_stored = self.rewards_per_token_stored.read();
            // Scale by PRECISION_SCALE for precision
            let additional_per_token = (new_rewards * PRECISION_SCALE) / total_hash.into();

            current_stored + additional_per_token
        }
    }
}
