#[starknet::interface]
pub trait IMineToken<TContractState> {
    // ERC-20 standard functions
    fn name(self: @TContractState) -> felt252;
    fn symbol(self: @TContractState) -> felt252;
    fn decimals(self: @TContractState) -> u8;
    fn total_supply(self: @TContractState) -> u256;
    fn balance_of(self: @TContractState, account: starknet::ContractAddress) -> u256;
    fn allowance(
        self: @TContractState, owner: starknet::ContractAddress, spender: starknet::ContractAddress,
    ) -> u256;
    fn transfer(
        ref self: TContractState, recipient: starknet::ContractAddress, amount: u256,
    ) -> bool;
    fn transfer_from(
        ref self: TContractState,
        sender: starknet::ContractAddress,
        recipient: starknet::ContractAddress,
        amount: u256,
    ) -> bool;
    fn approve(ref self: TContractState, spender: starknet::ContractAddress, amount: u256) -> bool;

    // StarkMine specific functions
    fn reward_per_block(self: @TContractState) -> u256;
    fn max_supply(self: @TContractState) -> u256;
    fn last_halving_block(self: @TContractState) -> u64;
    fn halving_interval(self: @TContractState) -> u64;
    fn remaining_blocks_for_halving(self: @TContractState) -> u64;
    fn get_distributor_address(self: @TContractState) -> starknet::ContractAddress;
    fn set_distributor_address(ref self: TContractState, distributor: starknet::ContractAddress);
    fn mint(ref self: TContractState, to: starknet::ContractAddress, amount: u256) -> bool;
    fn burn(ref self: TContractState, amount: u256) -> bool;
    fn check_for_halving(ref self: TContractState);
}

#[starknet::interface]
pub trait IMineTokenValidation<TContractState> {
    fn check_valid_distributor(self: @TContractState, caller: starknet::ContractAddress);
    fn check_minting_limit(self: @TContractState, amount: u256);
}
