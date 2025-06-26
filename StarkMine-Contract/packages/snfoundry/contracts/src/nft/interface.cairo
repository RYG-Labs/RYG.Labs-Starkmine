#[starknet::interface]
pub trait INFTMiner<TContractState> {
    // ERC-721 standard functions
    fn name(self: @TContractState) -> felt252;
    fn symbol(self: @TContractState) -> felt252;
    fn token_uri(self: @TContractState, token_id: u256) -> felt252;
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
    fn safe_transfer_from(
        ref self: TContractState,
        from: starknet::ContractAddress,
        to: starknet::ContractAddress,
        token_id: u256,
        data: Span<felt252>,
    );

    // StarkMine specific functions
    fn total_supply(self: @TContractState) -> u256;
    fn hash_power(self: @TContractState, token_id: u256) -> u128;
    fn room_id(self: @TContractState, token_id: u256) -> u256;
    fn get_room_manager(self: @TContractState) -> starknet::ContractAddress;
    fn set_room_manager(ref self: TContractState, manager: starknet::ContractAddress);
    fn get_reward_distributor(self: @TContractState) -> starknet::ContractAddress;
    fn set_reward_distributor(ref self: TContractState, distributor: starknet::ContractAddress);
    fn mint(
        ref self: TContractState, to: starknet::ContractAddress, room_id: u256, hash_power: u128,
    ) -> u256;
    fn assign_to_room(ref self: TContractState, token_id: u256, room_id: u256);
    fn upgrade(ref self: TContractState, token_id: u256, new_hash_power: u128) -> bool;
    fn burn(ref self: TContractState, token_id: u256);
    fn get_upgrade_cost(
        self: @TContractState, current_hash_power: u128, new_hash_power: u128,
    ) -> u256;
}

#[starknet::interface]
pub trait INFTMinerValidation<TContractState> {
    fn check_valid_room_manager(self: @TContractState, caller: starknet::ContractAddress);
    fn check_valid_owner(self: @TContractState, token_id: u256, caller: starknet::ContractAddress);
}

#[starknet::interface]
pub trait IMiningRoomManager<TContractState> {
    fn get_room_price(self: @TContractState) -> u256;
    fn set_room_price(ref self: TContractState, price: u256);
    fn get_room_count(self: @TContractState) -> u256;
    fn get_room_owner(self: @TContractState, room_id: u256) -> starknet::ContractAddress;
    fn get_nft_miner_address(self: @TContractState) -> starknet::ContractAddress;
    fn set_nft_miner_address(ref self: TContractState, address: starknet::ContractAddress);
    fn buy_room(ref self: TContractState) -> u256;
    fn get_room_miner_count(self: @TContractState, room_id: u256) -> u32;
    fn get_room_capacity(self: @TContractState) -> u32;
    fn is_room_full(self: @TContractState, room_id: u256) -> bool;
    fn add_miner_to_room(ref self: TContractState, room_id: u256);
    fn remove_miner_from_room(ref self: TContractState, room_id: u256);
}

#[starknet::interface]
pub trait IMiningRoomManagerValidation<TContractState> {
    fn check_valid_payment(self: @TContractState, payment: u256);
    fn check_valid_room_id(self: @TContractState, room_id: u256);
}
