export const ABI_MINER_NFT = [
    {
        "type": "impl",
        "name": "MinerNFTImpl",
        "interface_name": "starkmine::nft::miner_nft::IMinerNFT"
    },
    {
        "type": "struct",
        "name": "core::integer::u256",
        "members": [
            {
                "name": "low",
                "type": "core::integer::u128"
            },
            {
                "name": "high",
                "type": "core::integer::u128"
            }
        ]
    },
    {
        "type": "enum",
        "name": "core::bool",
        "variants": [
            {
                "name": "False",
                "type": "()"
            },
            {
                "name": "True",
                "type": "()"
            }
        ]
    },
    {
        "type": "struct",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerInfo",
        "members": [
            {
                "name": "tier",
                "type": "core::felt252"
            },
            {
                "name": "hash_power",
                "type": "core::integer::u128"
            },
            {
                "name": "level",
                "type": "core::integer::u8"
            },
            {
                "name": "efficiency",
                "type": "core::integer::u8"
            },
            {
                "name": "last_maintenance",
                "type": "core::integer::u64"
            },
            {
                "name": "core_engine_id",
                "type": "core::integer::u256"
            },
            {
                "name": "is_ignited",
                "type": "core::bool"
            }
        ]
    },
    {
        "type": "struct",
        "name": "starkmine::nft::miner_nft::MinerNFT::TierConfig",
        "members": [
            {
                "name": "base_hash_power",
                "type": "core::integer::u128"
            },
            {
                "name": "tier_bonus",
                "type": "core::integer::u128"
            },
            {
                "name": "mint_cost_mine",
                "type": "core::integer::u256"
            },
            {
                "name": "mint_cost_strk",
                "type": "core::integer::u256"
            },
            {
                "name": "supply_limit",
                "type": "core::integer::u256"
            },
            {
                "name": "minted_count",
                "type": "core::integer::u256"
            }
        ]
    },
    {
        "type": "interface",
        "name": "starkmine::nft::miner_nft::IMinerNFT",
        "items": [
            {
                "type": "function",
                "name": "name",
                "inputs": [],
                "outputs": [
                    {
                        "type": "core::felt252"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "symbol",
                "inputs": [],
                "outputs": [
                    {
                        "type": "core::felt252"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "balance_of",
                "inputs": [
                    {
                        "name": "owner",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::integer::u256"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "owner_of",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_approved",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "is_approved_for_all",
                "inputs": [
                    {
                        "name": "owner",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "operator",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::bool"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "approve",
                "inputs": [
                    {
                        "name": "to",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_approval_for_all",
                "inputs": [
                    {
                        "name": "operator",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "approved",
                        "type": "core::bool"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "transfer_from",
                "inputs": [
                    {
                        "name": "from",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "to",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "mint_miner",
                "inputs": [
                    {
                        "name": "to",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "tier",
                        "type": "core::felt252"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::integer::u256"
                    }
                ],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "ignite_miner",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "core_engine_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "extinguish_miner",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "upgrade_miner",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "maintain_miner",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "get_miner_info",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::nft::miner_nft::MinerNFT::MinerInfo"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_effective_hash_power",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::integer::u128"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_current_efficiency",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::integer::u8"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_tier_config",
                "inputs": [
                    {
                        "name": "tier",
                        "type": "core::felt252"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::nft::miner_nft::MinerNFT::TierConfig"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "burn",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_mine_token_address",
                "inputs": [
                    {
                        "name": "token",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_core_engine_contract",
                "inputs": [
                    {
                        "name": "contract",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_station_system_contract",
                "inputs": [
                    {
                        "name": "contract",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_merge_system_contract",
                "inputs": [
                    {
                        "name": "contract",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_reward_distributor_contract",
                "inputs": [
                    {
                        "name": "contract",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "sync_miner_hash_power",
                "inputs": [
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "pause",
                "inputs": [],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "unpause",
                "inputs": [],
                "outputs": [],
                "state_mutability": "external"
            }
        ]
    },
    {
        "type": "constructor",
        "name": "constructor",
        "inputs": [
            {
                "name": "name",
                "type": "core::felt252"
            },
            {
                "name": "symbol",
                "type": "core::felt252"
            },
            {
                "name": "owner",
                "type": "core::starknet::contract_address::ContractAddress"
            },
            {
                "name": "mine_token",
                "type": "core::starknet::contract_address::ContractAddress"
            },
            {
                "name": "strk_token",
                "type": "core::starknet::contract_address::ContractAddress"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::Transfer",
        "kind": "struct",
        "members": [
            {
                "name": "from",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "to",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::Approval",
        "kind": "struct",
        "members": [
            {
                "name": "owner",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "approved",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::ApprovalForAll",
        "kind": "struct",
        "members": [
            {
                "name": "owner",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "operator",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "approved",
                "type": "core::bool",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerMinted",
        "kind": "struct",
        "members": [
            {
                "name": "owner",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "tier",
                "type": "core::felt252",
                "kind": "data"
            },
            {
                "name": "hash_power",
                "type": "core::integer::u128",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerIgnited",
        "kind": "struct",
        "members": [
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "core_engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerExtinguished",
        "kind": "struct",
        "members": [
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerUpgraded",
        "kind": "struct",
        "members": [
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "old_level",
                "type": "core::integer::u8",
                "kind": "data"
            },
            {
                "name": "new_level",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::MinerMaintained",
        "kind": "struct",
        "members": [
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "old_efficiency",
                "type": "core::integer::u8",
                "kind": "data"
            },
            {
                "name": "new_efficiency",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::miner_nft::MinerNFT::Event",
        "kind": "enum",
        "variants": [
            {
                "name": "Transfer",
                "type": "starkmine::nft::miner_nft::MinerNFT::Transfer",
                "kind": "nested"
            },
            {
                "name": "Approval",
                "type": "starkmine::nft::miner_nft::MinerNFT::Approval",
                "kind": "nested"
            },
            {
                "name": "ApprovalForAll",
                "type": "starkmine::nft::miner_nft::MinerNFT::ApprovalForAll",
                "kind": "nested"
            },
            {
                "name": "MinerMinted",
                "type": "starkmine::nft::miner_nft::MinerNFT::MinerMinted",
                "kind": "nested"
            },
            {
                "name": "MinerIgnited",
                "type": "starkmine::nft::miner_nft::MinerNFT::MinerIgnited",
                "kind": "nested"
            },
            {
                "name": "MinerExtinguished",
                "type": "starkmine::nft::miner_nft::MinerNFT::MinerExtinguished",
                "kind": "nested"
            },
            {
                "name": "MinerUpgraded",
                "type": "starkmine::nft::miner_nft::MinerNFT::MinerUpgraded",
                "kind": "nested"
            },
            {
                "name": "MinerMaintained",
                "type": "starkmine::nft::miner_nft::MinerNFT::MinerMaintained",
                "kind": "nested"
            }
        ]
    }
]