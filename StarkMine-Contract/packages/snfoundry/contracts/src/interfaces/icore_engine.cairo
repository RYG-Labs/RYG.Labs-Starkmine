use starknet::ContractAddress;

#[derive(Drop, Serde, starknet::Store, Copy)]
pub struct EngineInfo {
    pub engine_type: felt252, // 'Standard', 'Premium', 'Elite'
    pub efficiency_bonus: u128, // Bonus percentage (in basis points)
    pub durability: u64, // Total blocks of operation
    pub blocks_used: u64, // Blocks already used
    pub last_used_block: u64, // Last block when engine was active
    pub attached_miner: u256, // Token ID of attached miner (0 if unattached)
    pub is_active: bool // Whether currently mining
}

#[derive(Drop, Serde, starknet::Store, Copy)]
pub struct EngineTypeConfig {
    pub efficiency_bonus: u128, // Bonus percentage (in basis points)
    pub durability: u64, // Total operational blocks
    pub mint_cost: u256, // Cost in MINE tokens
    pub repair_cost_base: u256 // Base cost for repairs
}

// Interface for the CoreEngine contract
#[starknet::interface]
pub trait ICoreEngine<TContractState> {
    // ERC-721 standard
    fn name(self: @TContractState) -> felt252;
    fn symbol(self: @TContractState) -> felt252;
    fn balance_of(self: @TContractState, owner: ContractAddress) -> u256;
    fn owner_of(self: @TContractState, token_id: u256) -> ContractAddress;
    fn get_approved(self: @TContractState, token_id: u256) -> ContractAddress;
    fn is_approved_for_all(
        self: @TContractState, owner: ContractAddress, operator: ContractAddress,
    ) -> bool;
    fn approve(ref self: TContractState, to: ContractAddress, token_id: u256);
    fn set_approval_for_all(ref self: TContractState, operator: ContractAddress, approved: bool);
    fn transfer_from(
        ref self: TContractState, from: ContractAddress, to: ContractAddress, token_id: u256,
    );

    // Core Engine specific functions
    fn mint_engine(ref self: TContractState, to: ContractAddress, engine_type: felt252) -> u256;
    fn attach_to_miner(
        ref self: TContractState, engine_id: u256, miner_id: u256, owner: ContractAddress,
    );
    fn detach_from_miner(ref self: TContractState, engine_id: u256, owner: ContractAddress);
    fn repair_engine(ref self: TContractState, engine_id: u256, durability_to_restore: u64);
    fn get_engine_info(self: @TContractState, engine_id: u256) -> EngineInfo;
    fn get_engine_remaining_durability(self: @TContractState, engine_id: u256) -> u64;
    fn is_engine_expired(self: @TContractState, engine_id: u256) -> bool;
    fn get_current_efficiency_bonus(self: @TContractState, engine_id: u256) -> u128;
    fn update_engine_usage(ref self: TContractState, engine_id: u256);
    fn get_engine_type_config(self: @TContractState, engine_type: felt252) -> EngineTypeConfig;

    // Admin functions
    fn set_mine_token_address(ref self: TContractState, token: ContractAddress);
    fn set_miner_nft_contract(ref self: TContractState, contract: ContractAddress);
    fn pause(ref self: TContractState);
    fn unpause(ref self: TContractState);
}
