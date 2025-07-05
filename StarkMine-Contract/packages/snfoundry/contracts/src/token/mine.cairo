#[starknet::contract]
mod MineToken {
    use core::num::traits::Zero;
    use starkmine::token::interface::{IMineToken, IMineTokenValidation};
    use starknet::storage::{
        Map, StorageMapReadAccess, StorageMapWriteAccess, StoragePointerReadAccess,
        StoragePointerWriteAccess,
    };
    use starknet::{ContractAddress, get_block_number, get_caller_address};

    #[storage]
    struct Storage {
        name: felt252,
        symbol: felt252,
        decimals: u8,
        total_supply: u256,
        balances: Map<ContractAddress, u256>,
        allowances: Map<(ContractAddress, ContractAddress), u256>,
        // StarkMine specific
        reward_per_block: u256,
        max_supply: u256,
        last_halving_block: u64,
        halving_interval: u64,
        distributor_address: ContractAddress,
    }

    #[event]
    #[derive(Drop, starknet::Event)]
    enum Event {
        Transfer: Transfer,
        Approval: Approval,
        Halving: Halving,
    }

    #[derive(Drop, starknet::Event)]
    struct Transfer {
        #[key]
        from: ContractAddress,
        #[key]
        to: ContractAddress,
        value: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct Approval {
        #[key]
        owner: ContractAddress,
        #[key]
        spender: ContractAddress,
        value: u256,
    }

    #[derive(Drop, starknet::Event)]
    struct Halving {
        block_number: u64,
        new_reward_per_block: u256,
    }

    #[constructor]
    fn constructor(
        ref self: ContractState,
        name: felt252,
        symbol: felt252,
        decimals: u8,
        initial_supply: u256,
        recipient: ContractAddress,
        max_supply: u256,
        reward_per_block: u256,
        halving_interval: u64,
    ) {
        self.name.write(name);
        self.symbol.write(symbol);
        self.decimals.write(decimals);
        self.reward_per_block.write(reward_per_block);
        self.max_supply.write(max_supply);
        self.halving_interval.write(halving_interval);
        self.last_halving_block.write(get_block_number().try_into().unwrap());

        if (initial_supply.is_non_zero()) {
            self.total_supply.write(initial_supply);
            self.balances.write(recipient, initial_supply);

            let zero_address: ContractAddress = Zero::zero();
            self.emit(Transfer { from: zero_address, to: recipient, value: initial_supply });
        }
    }

    #[abi(embed_v0)]
    impl MineTokenImpl of IMineToken<ContractState> {
        fn name(self: @ContractState) -> felt252 {
            self.name.read()
        }

        fn symbol(self: @ContractState) -> felt252 {
            self.symbol.read()
        }

        fn decimals(self: @ContractState) -> u8 {
            self.decimals.read()
        }

        fn total_supply(self: @ContractState) -> u256 {
            self.total_supply.read()
        }

        fn balance_of(self: @ContractState, account: ContractAddress) -> u256 {
            self.balances.read(account)
        }

        fn allowance(
            self: @ContractState, owner: ContractAddress, spender: ContractAddress,
        ) -> u256 {
            self.allowances.read((owner, spender))
        }

        fn transfer(ref self: ContractState, recipient: ContractAddress, amount: u256) -> bool {
            let sender = get_caller_address();
            self.transfer_helper(sender, recipient, amount);
            true
        }

        fn transfer_from(
            ref self: ContractState,
            sender: ContractAddress,
            recipient: ContractAddress,
            amount: u256,
        ) -> bool {
            let caller = get_caller_address();
            let current_allowance = self.allowances.read((sender, caller));
            assert!(current_allowance >= amount, "ERC20: insufficient allowance");

            self.allowances.write((sender, caller), current_allowance - amount);
            self.transfer_helper(sender, recipient, amount);
            true
        }

        fn approve(ref self: ContractState, spender: ContractAddress, amount: u256) -> bool {
            let caller = get_caller_address();
            self.allowances.write((caller, spender), amount);
            self.emit(Approval { owner: caller, spender, value: amount });
            true
        }

        fn reward_per_block(self: @ContractState) -> u256 {
            self.reward_per_block.read()
        }

        fn max_supply(self: @ContractState) -> u256 {
            self.max_supply.read()
        }

        fn last_halving_block(self: @ContractState) -> u64 {
            self.last_halving_block.read()
        }

        fn halving_interval(self: @ContractState) -> u64 {
            self.halving_interval.read()
        }

        fn get_distributor_address(self: @ContractState) -> ContractAddress {
            self.distributor_address.read()
        }

        fn set_distributor_address(ref self: ContractState, distributor: ContractAddress) {
            let caller = get_caller_address();
            // Only the current distributor or zero address distributor can set a new one
            let current_distributor = self.distributor_address.read();
            assert!(
                current_distributor.is_zero() || caller == current_distributor,
                "Only current distributor can change distributor",
            );
            self.distributor_address.write(distributor);
        }

        fn mint(ref self: ContractState, to: ContractAddress, amount: u256) -> bool {
            let caller = get_caller_address();
            self.check_valid_distributor(caller);
            self.check_minting_limit(amount);

            let current_supply = self.total_supply.read();
            let new_supply = current_supply + amount;

            self.total_supply.write(new_supply);
            let recipient_balance = self.balances.read(to);
            self.balances.write(to, recipient_balance + amount);

            let zero_address: ContractAddress = Zero::zero();
            self.emit(Transfer { from: zero_address, to, value: amount });
            true
        }

        fn burn(ref self: ContractState, amount: u256) -> bool {
            let caller = get_caller_address();
            self.check_valid_distributor(caller);

            // Check that there are enough tokens to burn
            let current_supply = self.total_supply.read();
            assert!(current_supply >= amount, "Insufficient tokens to burn");

            // Update total supply
            self.total_supply.write(current_supply - amount);

            // Burn tokens from the caller (distributor)
            let caller_balance = self.balances.read(caller);
            assert!(caller_balance >= amount, "Distributor has insufficient balance");
            self.balances.write(caller, caller_balance - amount);

            // Emit transfer event to zero address (burn)
            let zero_address: ContractAddress = Zero::zero();
            self.emit(Transfer { from: caller, to: zero_address, value: amount });

            true
        }

        fn check_for_halving(ref self: ContractState) {
            let current_block = get_block_number().try_into().unwrap();
            let last_halving = self.last_halving_block.read();
            let interval = self.halving_interval.read();

            if (current_block >= last_halving + interval) {
                // Time for halving
                let current_reward = self.reward_per_block.read();
                let new_reward = current_reward / 2.into();

                self.reward_per_block.write(new_reward);
                self.last_halving_block.write(current_block);

                self
                    .emit(
                        Halving { block_number: current_block, new_reward_per_block: new_reward },
                    );
            }
        }
    }

    #[abi(embed_v0)]
    impl MineTokenValidationImpl of IMineTokenValidation<ContractState> {
        fn check_valid_distributor(self: @ContractState, caller: ContractAddress) {
            let distributor = self.distributor_address.read();
            assert!(caller == distributor, "Caller is not the distributor");
        }

        fn check_minting_limit(self: @ContractState, amount: u256) {
            let current_supply = self.total_supply.read();
            let max_supply = self.max_supply.read();

            assert!(current_supply + amount <= max_supply, "Would exceed max supply");
        }
    }

    // Helper methods
    #[generate_trait]
    impl MineTokenHelpers of MineTokenHelpersTrait {
        fn transfer_helper(
            ref self: ContractState,
            sender: ContractAddress,
            recipient: ContractAddress,
            amount: u256,
        ) {
            assert!(sender.is_non_zero(), "ERC20: transfer from 0");
            assert!(recipient.is_non_zero(), "ERC20: transfer to 0");

            let sender_balance = self.balances.read(sender);
            assert!(sender_balance >= amount, "ERC20: insufficient balance");

            self.balances.write(sender, sender_balance - amount);
            let recipient_balance = self.balances.read(recipient);
            self.balances.write(recipient, recipient_balance + amount);

            self.emit(Transfer { from: sender, to: recipient, value: amount });
        }
    }
}
