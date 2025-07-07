const ABIStationSystem = [
    {
        "type": "impl",
        "name": "StationSystemImpl",
        "interface_name": "starkmine::mining::station_system::IStationSystem"
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
        "type": "struct",
        "name": "starkmine::mining::station_system::StationSystem::StationInfo",
        "members": [
            {
                "name": "level",
                "type": "core::integer::u8"
            },
            {
                "name": "multiplier",
                "type": "core::integer::u128"
            },
            {
                "name": "mine_locked",
                "type": "core::integer::u256"
            },
            {
                "name": "lock_timestamp",
                "type": "core::integer::u64"
            },
            {
                "name": "unlock_timestamp",
                "type": "core::integer::u64"
            },
            {
                "name": "pending_downgrade",
                "type": "core::integer::u8"
            },
            {
                "name": "miner_count",
                "type": "core::integer::u8"
            }
        ]
    },
    {
        "type": "struct",
        "name": "starkmine::mining::station_system::StationSystem::MinerInfo",
        "members": [
            {
                "name": "token_id",
                "type": "core::integer::u256"
            },
            {
                "name": "slot",
                "type": "core::integer::u8"
            }
        ]
    },
    {
        "type": "struct",
        "name": "starkmine::mining::station_system::StationSystem::LevelConfig",
        "members": [
            {
                "name": "multiplier",
                "type": "core::integer::u128"
            },
            {
                "name": "mine_required",
                "type": "core::integer::u256"
            },
            {
                "name": "unlock_period",
                "type": "core::integer::u64"
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
        "type": "interface",
        "name": "starkmine::mining::station_system::IStationSystem",
        "items": [
            {
                "type": "function",
                "name": "initialize_user_stations",
                "inputs": [],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "upgrade_station",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    },
                    {
                        "name": "target_level",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "request_downgrade",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    },
                    {
                        "name": "target_level",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "execute_downgrade",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "cancel_downgrade",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "emergency_withdraw",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "assign_miner_to_station",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    },
                    {
                        "name": "token_id",
                        "type": "core::integer::u256"
                    },
                    {
                        "name": "miner_slot",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "remove_miner_from_station",
                "inputs": [
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    },
                    {
                        "name": "miner_slot",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "get_station_info",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::mining::station_system::StationSystem::StationInfo"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_account_multiplier",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
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
                "name": "get_miner_station_assignment",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
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
                "name": "get_station_miners",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [
                    {
                        "type": "core::array::Array::<starkmine::mining::station_system::StationSystem::MinerInfo>"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_user_station_count",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
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
                "name": "get_level_config",
                "inputs": [
                    {
                        "name": "level",
                        "type": "core::integer::u8"
                    }
                ],
                "outputs": [
                    {
                        "type": "starkmine::mining::station_system::StationSystem::LevelConfig"
                    }
                ],
                "state_mutability": "view"
            },
            {
                "type": "function",
                "name": "get_time_until_unlock",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
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
                "name": "can_execute_downgrade",
                "inputs": [
                    {
                        "name": "account",
                        "type": "core::starknet::contract_address::ContractAddress"
                    },
                    {
                        "name": "station_id",
                        "type": "core::integer::u8"
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
                "name": "update_level_config",
                "inputs": [
                    {
                        "name": "level",
                        "type": "core::integer::u8"
                    },
                    {
                        "name": "config",
                        "type": "starkmine::mining::station_system::StationSystem::LevelConfig"
                    }
                ],
                "outputs": [],
                "state_mutability": "external"
            },
            {
                "type": "function",
                "name": "set_unlock_period",
                "inputs": [
                    {
                        "name": "blocks",
                        "type": "core::integer::u64"
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
            }
        ]
    },
    {
        "type": "constructor",
        "name": "constructor",
        "inputs": [
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
        "name": "starkmine::mining::station_system::StationSystem::StationUpgraded",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
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
            },
            {
                "name": "new_multiplier",
                "type": "core::integer::u128",
                "kind": "data"
            },
            {
                "name": "mine_locked",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::StationDowngraded",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
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
            },
            {
                "name": "new_multiplier",
                "type": "core::integer::u128",
                "kind": "data"
            },
            {
                "name": "mine_unlocked",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::TokensLocked",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "amount",
                "type": "core::integer::u256",
                "kind": "data"
            },
            {
                "name": "new_total",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::TokensUnlocked",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "amount",
                "type": "core::integer::u256",
                "kind": "data"
            },
            {
                "name": "remaining",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::DowngradeRequested",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "current_level",
                "type": "core::integer::u8",
                "kind": "data"
            },
            {
                "name": "target_level",
                "type": "core::integer::u8",
                "kind": "data"
            },
            {
                "name": "unlock_timestamp",
                "type": "core::integer::u64",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::DowngradeCanceled",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "canceled_level",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::EmergencyWithdrawal",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "amount",
                "type": "core::integer::u256",
                "kind": "data"
            },
            {
                "name": "penalty",
                "type": "core::integer::u256",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::MinerAssigned",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "miner_slot",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::MinerRemoved",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_id",
                "type": "core::integer::u8",
                "kind": "key"
            },
            {
                "name": "token_id",
                "type": "core::integer::u256",
                "kind": "key"
            },
            {
                "name": "miner_slot",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::StationsInitialized",
        "kind": "struct",
        "members": [
            {
                "name": "account",
                "type": "core::starknet::contract_address::ContractAddress",
                "kind": "key"
            },
            {
                "name": "station_count",
                "type": "core::integer::u8",
                "kind": "data"
            }
        ]
    },
    {
        "type": "event",
        "name": "starkmine::mining::station_system::StationSystem::Event",
        "kind": "enum",
        "variants": [
            {
                "name": "StationUpgraded",
                "type": "starkmine::mining::station_system::StationSystem::StationUpgraded",
                "kind": "nested"
            },
            {
                "name": "StationDowngraded",
                "type": "starkmine::mining::station_system::StationSystem::StationDowngraded",
                "kind": "nested"
            },
            {
                "name": "TokensLocked",
                "type": "starkmine::mining::station_system::StationSystem::TokensLocked",
                "kind": "nested"
            },
            {
                "name": "TokensUnlocked",
                "type": "starkmine::mining::station_system::StationSystem::TokensUnlocked",
                "kind": "nested"
            },
            {
                "name": "DowngradeRequested",
                "type": "starkmine::mining::station_system::StationSystem::DowngradeRequested",
                "kind": "nested"
            },
            {
                "name": "DowngradeCanceled",
                "type": "starkmine::mining::station_system::StationSystem::DowngradeCanceled",
                "kind": "nested"
            },
            {
                "name": "EmergencyWithdrawal",
                "type": "starkmine::mining::station_system::StationSystem::EmergencyWithdrawal",
                "kind": "nested"
            },
            {
                "name": "MinerAssigned",
                "type": "starkmine::mining::station_system::StationSystem::MinerAssigned",
                "kind": "nested"
            },
            {
                "name": "MinerRemoved",
                "type": "starkmine::mining::station_system::StationSystem::MinerRemoved",
                "kind": "nested"
            },
            {
                "name": "StationsInitialized",
                "type": "starkmine::mining::station_system::StationSystem::StationsInitialized",
                "kind": "nested"
            }
        ]
    }
]

export default ABIStationSystem;