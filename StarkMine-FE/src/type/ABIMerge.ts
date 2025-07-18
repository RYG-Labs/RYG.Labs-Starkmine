const ABIMerge = [
  {
    type: "impl",
    name: "MergeSystemImpl",
    interface_name: "starkmine::mining::merge_system::IMergeSystem",
  },
  {
    type: "struct",
    name: "core::integer::u256",
    members: [
      {
        name: "low",
        type: "core::integer::u128",
      },
      {
        name: "high",
        type: "core::integer::u128",
      },
    ],
  },
  {
    type: "enum",
    name: "core::bool",
    variants: [
      {
        name: "False",
        type: "()",
      },
      {
        name: "True",
        type: "()",
      },
    ],
  },
  {
    type: "struct",
    name: "starkmine::mining::merge_system::MergeSystem::MergeConfig",
    members: [
      {
        name: "base_success_rate",
        type: "core::integer::u128",
      },
      {
        name: "failure_bonus",
        type: "core::integer::u128",
      },
      {
        name: "max_failure_bonus",
        type: "core::integer::u128",
      },
      {
        name: "cost_mine",
        type: "core::integer::u256",
      },
      {
        name: "cost_strk",
        type: "core::integer::u256",
      },
    ],
  },
  {
    type: "struct",
    name: "starkmine::mining::merge_system::MergeSystem::MergeHistory",
    members: [
      {
        name: "attempts",
        type: "core::integer::u32",
      },
      {
        name: "failures",
        type: "core::integer::u32",
      },
      {
        name: "current_bonus",
        type: "core::integer::u128",
      },
    ],
  },
  {
    type: "interface",
    name: "starkmine::mining::merge_system::IMergeSystem",
    items: [
      {
        type: "function",
        name: "attempt_merge",
        inputs: [
          {
            name: "token_id_1",
            type: "core::integer::u256",
          },
          {
            name: "token_id_2",
            type: "core::integer::u256",
          },
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
        ],
        outputs: [
          {
            type: "(core::bool, core::integer::u256)",
          },
        ],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "get_merge_config",
        inputs: [
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
        ],
        outputs: [
          {
            type: "starkmine::mining::merge_system::MergeSystem::MergeConfig",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "get_user_merge_history",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
        ],
        outputs: [
          {
            type: "starkmine::mining::merge_system::MergeSystem::MergeHistory",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "get_current_success_rate",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
        ],
        outputs: [
          {
            type: "core::integer::u128",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "can_attempt_merge",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
        ],
        outputs: [
          {
            type: "core::bool",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "get_global_stats",
        inputs: [],
        outputs: [
          {
            type: "(core::integer::u256, core::integer::u256)",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "update_merge_config",
        inputs: [
          {
            name: "from_tier",
            type: "core::felt252",
          },
          {
            name: "to_tier",
            type: "core::felt252",
          },
          {
            name: "config",
            type: "starkmine::mining::merge_system::MergeSystem::MergeConfig",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "set_mine_token_address",
        inputs: [
          {
            name: "token",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "set_miner_nft_contract",
        inputs: [
          {
            name: "contract",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "pause",
        inputs: [],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "unpause",
        inputs: [],
        outputs: [],
        state_mutability: "external",
      },
    ],
  },
  {
    type: "constructor",
    name: "constructor",
    inputs: [
      {
        name: "owner",
        type: "core::starknet::contract_address::ContractAddress",
      },
      {
        name: "mine_token",
        type: "core::starknet::contract_address::ContractAddress",
      },
      {
        name: "strk_token",
        type: "core::starknet::contract_address::ContractAddress",
      },
      {
        name: "miner_nft",
        type: "core::starknet::contract_address::ContractAddress",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::merge_system::MergeSystem::MergeAttempted",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "key",
      },
      {
        name: "from_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "to_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "source_token_1",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "source_token_2",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "success_rate",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::merge_system::MergeSystem::MergeSuccessful",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "key",
      },
      {
        name: "from_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "to_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "new_token_id",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "final_success_rate",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::merge_system::MergeSystem::MergeFailed",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "key",
      },
      {
        name: "from_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "to_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "new_failure_bonus",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::merge_system::MergeSystem::ConfigUpdated",
    kind: "struct",
    members: [
      {
        name: "from_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "to_tier",
        type: "core::felt252",
        kind: "data",
      },
      {
        name: "new_base_rate",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::merge_system::MergeSystem::Event",
    kind: "enum",
    variants: [
      {
        name: "MergeAttempted",
        type: "starkmine::mining::merge_system::MergeSystem::MergeAttempted",
        kind: "nested",
      },
      {
        name: "MergeSuccessful",
        type: "starkmine::mining::merge_system::MergeSystem::MergeSuccessful",
        kind: "nested",
      },
      {
        name: "MergeFailed",
        type: "starkmine::mining::merge_system::MergeSystem::MergeFailed",
        kind: "nested",
      },
      {
        name: "ConfigUpdated",
        type: "starkmine::mining::merge_system::MergeSystem::ConfigUpdated",
        kind: "nested",
      },
    ],
  },
];

export default ABIMerge;
