#[starknet::interface]
pub trait IRewardDistributor<TContractState> {
    fn total_hash_power(self: @TContractState) -> u128;
    fn user_hash_power(self: @TContractState, user: starknet::ContractAddress) -> u128;
    fn pending_rewards(self: @TContractState, user: starknet::ContractAddress) -> u256;
    fn mine_token_address(self: @TContractState) -> starknet::ContractAddress;
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn set_miner_nft_address(ref self: TContractState, nft_address: starknet::ContractAddress);
    fn last_update_block(self: @TContractState) -> u64;
    fn burn_rate(self: @TContractState) -> u128;
    fn set_burn_rate(ref self: TContractState, rate: u128);
    fn reward_per_block(self: @TContractState) -> u256;
    fn set_reward_per_block(ref self: TContractState, reward: u256);
    fn add_hash_power(ref self: TContractState, user: starknet::ContractAddress, hash_power: u128);
    fn remove_hash_power(
        ref self: TContractState, user: starknet::ContractAddress, hash_power: u128,
    );
    fn distribute(ref self: TContractState);
    fn claim_rewards(ref self: TContractState, user: starknet::ContractAddress) -> u256;
    fn burn(ref self: TContractState, amount: u256) -> u256;
    fn authorize_contract(
        ref self: TContractState, contract_address: starknet::ContractAddress, authorized: bool,
    );
}

#[starknet::interface]
pub trait IRewardDistributorValidation<TContractState> {
    fn check_valid_token(self: @TContractState);
    fn check_valid_burn_rate(self: @TContractState, rate: u128);
    fn check_valid_hash_power(
        self: @TContractState, user: starknet::ContractAddress, hash_power: u128,
    );
}

#[starknet::interface]
pub trait ILevelManager<TContractState> {
    fn get_user_level(self: @TContractState, user: starknet::ContractAddress) -> u32;
    fn get_level_cost_eth(self: @TContractState, level: u32) -> u256;
    fn get_level_cost_mine(self: @TContractState, level: u32) -> u256;
    fn get_level_hash_boost(self: @TContractState, level: u32) -> u128;
    fn set_level_cost_eth(ref self: TContractState, level: u32, cost: u256);
    fn set_level_cost_mine(ref self: TContractState, level: u32, cost: u256);
    fn set_level_hash_boost(ref self: TContractState, level: u32, boost: u128);
    fn set_mine_token_address(ref self: TContractState, token: starknet::ContractAddress);
    fn level_up_with_eth(ref self: TContractState, user: starknet::ContractAddress);
    fn level_up_with_mine(ref self: TContractState, user: starknet::ContractAddress);
    fn get_user_effective_hash_power(
        self: @TContractState, user: starknet::ContractAddress, base_hash_power: u128,
    ) -> u128;
}

#[starknet::interface]
pub trait ILevelManagerValidation<TContractState> {
    fn check_valid_level(self: @TContractState, level: u32);
    fn check_valid_payment(self: @TContractState, payment: u256, required: u256);
    fn check_valid_token(self: @TContractState);
}
