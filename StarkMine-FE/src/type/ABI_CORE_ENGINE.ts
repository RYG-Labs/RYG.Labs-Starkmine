export const ABI_CORE_ENGINE = [
    {
        "type": "impl",
        "name": "CoreEngineImpl",
        "interface_name": "starkmine::interfaces::icore_engine::ICoreEngine"
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
        "name": "starkmine::interfaces::icore_engine::EngineInfo",
        "members": [
            {
                "name": "engine_type",
                "type": "core::felt252"
            },
            {
                "name": "efficiency_bonus",
                "type": "core::integer::u128"
            },
            {
                "name": "durability",
                "type": "core::integer::u64"
            },
            {
                "name": "blocks_used",
                "type": "core::integer::u64"
            },
            {
                "name": "last_used_block",
                "type": "core::integer::u64"
            },
            {
                "name": "attached_miner",
                "type": "core::integer::u256"
            },
            {
                "name": "is_active",
                "type": "core::bool"
            }
        ]
    },
    {
        "type": "struct",
        "name": "starkmine::interfaces::icore_engine::EngineTypeConfig",
        "members": [
            {
                "name": "efficiency_bonus",
                "type": "core::integer::u128"
            },
            {
                "name": "durability",
                "type": "core::integer::u64"
            },
            {
                "name": "mint_cost",
                "type": "core::integer::u256"
            },
            {
                "name": "repair_cost_base",
                "type": "core::integer::u256"
            }
        ]
    },
    {
        "type": "interface",
        "name": "starkmine::interfaces::icore_engine::ICoreEngine",
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
                "name": "mint_engine",
                "inputs": [
                    {
                        "name": "to",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "engine_type",
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
                "name": "attach_to_miner",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "miner_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "owner",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "detach_from_miner",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "owner",
                        "type": "core::starknet::contract_address::ContractAddress"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "repair_engine",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "durability_to_restore",
                        "type": "core::integer::u64"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "defuse_engine",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
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
                "name": "get_engine_info",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::interfaces::icore_engine::EngineInfo"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_engine_remaining_durability",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::integer::u64"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "is_engine_expired",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
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
                "name": "get_current_efficiency_bonus",
                "inputs": [
                    {
                        "name": "engine_id",
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
                "name": "update_engine_usage",
                "inputs": [
                    {
                        "name": "engine_id",
                        "type": "core::integer::u256"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "get_engine_type_config",
                "inputs": [
                    {
                        "name": "engine_type",
                        "type": "core::felt252"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::interfaces::icore_engine::EngineTypeConfig"
                    }
                ],
                "state_mutability": "view"
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
                "name": "set_miner_nft_contract",
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
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::Transfer",
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
        "name": "starkmine::nft::core_engine::CoreEngine::Approval",
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
        "name": "starkmine::nft::core_engine::CoreEngine::ApprovalForAll",
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
        "name": "starkmine::nft::core_engine::CoreEngine::EngineMinted",
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
                "name": "engine_type",
                "type": "core::felt252",
                "kind": "data"
            },
            {
                "name": "efficiency_bonus",
                "type": "core::integer::u128",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::EngineAttached",
        "kind": "struct",
        "members": [
            {
                "name": "engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "miner_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::EngineDetached",
        "kind": "struct",
        "members": [
            {
                "name": "engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "miner_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::EngineRepaired",
        "kind": "struct",
        "members": [
            {
                "name": "engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "cost",
                "type": "core::integer::u256",
                "kind": "data"
            },
            {
                "name": "durability_restored",
                "type": "core::integer::u64",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::EngineExpired",
        "kind": "struct",
        "members": [
            {
                "name": "engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::EngineDefused",
        "kind": "struct",
        "members": [
            {
                "name": "engine_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "owner",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "refund_amount",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::nft::core_engine::CoreEngine::Event",
        "kind": "enum",
        "variants": [
            {
                "name": "Transfer",
                "type": "starkmine::nft::core_engine::CoreEngine::Transfer",
                "kind": "nested"
            },
            {
                "name": "Approval",
                "type": "starkmine::nft::core_engine::CoreEngine::Approval",
                "kind": "nested"
            },
            {
                "name": "ApprovalForAll",
                "type": "starkmine::nft::core_engine::CoreEngine::ApprovalForAll",
                "kind": "nested"
            },
            {
                "name": "EngineMinted",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineMinted",
                "kind": "nested"
            },
            {
                "name": "EngineAttached",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineAttached",
                "kind": "nested"
            },
            {
                "name": "EngineDetached",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineDetached",
                "kind": "nested"
            },
            {
                "name": "EngineRepaired",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineRepaired",
                "kind": "nested"
            },
            {
                "name": "EngineExpired",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineExpired",
                "kind": "nested"
            },
            {
                "name": "EngineDefused",
                "type": "starkmine::nft::core_engine::CoreEngine::EngineDefused",
                "kind": "nested"
            }
        ]
    }
]