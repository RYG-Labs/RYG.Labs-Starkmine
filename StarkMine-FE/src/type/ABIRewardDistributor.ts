const ABIRewardDistributor = [
  {
    type: "impl",
    name: "RewardDistributorImpl",
    interface_name: "starkmine::mining::interface::IRewardDistributor",
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
    type: "struct",
    name: "starkmine::mining::interface::PendingReward",
    members: [
      {
        name: "pending_rewards",
        type: "core::integer::u256",
      },
      {
        name: "last_claimed_at",
        type: "core::integer::u64",
      },
    ],
  },
  {
    type: "interface",
    name: "starkmine::mining::interface::IRewardDistributor",
    items: [
      {
        type: "function",
        name: "total_hash_power",
        inputs: [],
        outputs: [
          {
            type: "core::integer::u128",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "user_hash_power",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
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
        name: "pending_rewards",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [
          {
            type: "starkmine::mining::interface::PendingReward",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "mine_token_address",
        inputs: [],
        outputs: [
          {
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        state_mutability: "view",
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
        name: "set_miner_nft_address",
        inputs: [
          {
            name: "nft_address",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "burn_rate",
        inputs: [],
        outputs: [
          {
            type: "core::integer::u128",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "set_burn_rate",
        inputs: [
          {
            name: "rate",
            type: "core::integer::u128",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "reward_per_block",
        inputs: [],
        outputs: [
          {
            type: "core::integer::u256",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "set_reward_per_block",
        inputs: [
          {
            name: "reward",
            type: "core::integer::u256",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "add_hash_power",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "hash_power",
            type: "core::integer::u128",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "remove_hash_power",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "hash_power",
            type: "core::integer::u128",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "claim_rewards",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [
          {
            type: "core::integer::u256",
          },
        ],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "burn",
        inputs: [
          {
            name: "amount",
            type: "core::integer::u256",
          },
        ],
        outputs: [
          {
            type: "core::integer::u256",
          },
        ],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "update_reward",
        inputs: [],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "get_users_mining",
        inputs: [],
        outputs: [
          {
            type: "core::array::Array::<core::starknet::contract_address::ContractAddress>",
          },
        ],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "get_latest_updated_block",
        inputs: [],
        outputs: [
          {
            type: "core::integer::u64",
          },
        ],
        state_mutability: "view",
      },
    ],
  },
  {
    type: "impl",
    name: "RewardDistributorValidationImpl",
    interface_name:
      "starkmine::mining::interface::IRewardDistributorValidation",
  },
  {
    type: "interface",
    name: "starkmine::mining::interface::IRewardDistributorValidation",
    items: [
      {
        type: "function",
        name: "check_valid_token",
        inputs: [],
        outputs: [],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "check_valid_burn_rate",
        inputs: [
          {
            name: "rate",
            type: "core::integer::u128",
          },
        ],
        outputs: [],
        state_mutability: "view",
      },
      {
        type: "function",
        name: "check_valid_hash_power",
        inputs: [
          {
            name: "user",
            type: "core::starknet::contract_address::ContractAddress",
          },
          {
            name: "hash_power",
            type: "core::integer::u128",
          },
        ],
        outputs: [],
        state_mutability: "view",
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
        name: "burn_rate",
        type: "core::integer::u128",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::HashPowerAdded",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "hash_power",
        type: "core::integer::u128",
        kind: "data",
      },
      {
        name: "total_hash_power",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::HashPowerRemoved",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "hash_power",
        type: "core::integer::u128",
        kind: "data",
      },
      {
        name: "total_hash_power",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::RewardsDistributed",
    kind: "struct",
    members: [
      {
        name: "block_number",
        type: "core::integer::u64",
        kind: "data",
      },
      {
        name: "total_rewards",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "rewards_per_token",
        type: "core::integer::u256",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::RewardsClaimed",
    kind: "struct",
    members: [
      {
        name: "user",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "amount",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "claimed_at",
        type: "core::integer::u64",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::TokensBurned",
    kind: "struct",
    members: [
      {
        name: "amount",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "total_supply_after",
        type: "core::integer::u256",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::MineTokenAddressSet",
    kind: "struct",
    members: [
      {
        name: "old_address",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "new_address",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::MinerNFTAddressSet",
    kind: "struct",
    members: [
      {
        name: "old_address",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "new_address",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::BurnRateSet",
    kind: "struct",
    members: [
      {
        name: "old_rate",
        type: "core::integer::u128",
        kind: "data",
      },
      {
        name: "new_rate",
        type: "core::integer::u128",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::RewardPerBlockSet",
    kind: "struct",
    members: [
      {
        name: "old_reward",
        type: "core::integer::u256",
        kind: "data",
      },
      {
        name: "new_reward",
        type: "core::integer::u256",
        kind: "data",
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
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::ContractAuthorized",
    kind: "struct",
    members: [
      {
        name: "contract_address",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "data",
      },
      {
        name: "authorized",
        type: "core::bool",
        kind: "data",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::reward_distributor::RewardDistributor::Event",
    kind: "enum",
    variants: [
      {
        name: "HashPowerAdded",
        type: "starkmine::mining::reward_distributor::RewardDistributor::HashPowerAdded",
        kind: "nested",
      },
      {
        name: "HashPowerRemoved",
        type: "starkmine::mining::reward_distributor::RewardDistributor::HashPowerRemoved",
        kind: "nested",
      },
      {
        name: "RewardsDistributed",
        type: "starkmine::mining::reward_distributor::RewardDistributor::RewardsDistributed",
        kind: "nested",
      },
      {
        name: "RewardsClaimed",
        type: "starkmine::mining::reward_distributor::RewardDistributor::RewardsClaimed",
        kind: "nested",
      },
      {
        name: "TokensBurned",
        type: "starkmine::mining::reward_distributor::RewardDistributor::TokensBurned",
        kind: "nested",
      },
      {
        name: "MineTokenAddressSet",
        type: "starkmine::mining::reward_distributor::RewardDistributor::MineTokenAddressSet",
        kind: "nested",
      },
      {
        name: "MinerNFTAddressSet",
        type: "starkmine::mining::reward_distributor::RewardDistributor::MinerNFTAddressSet",
        kind: "nested",
      },
      {
        name: "BurnRateSet",
        type: "starkmine::mining::reward_distributor::RewardDistributor::BurnRateSet",
        kind: "nested",
      },
      {
        name: "RewardPerBlockSet",
        type: "starkmine::mining::reward_distributor::RewardDistributor::RewardPerBlockSet",
        kind: "nested",
      },
      {
        name: "ContractAuthorized",
        type: "starkmine::mining::reward_distributor::RewardDistributor::ContractAuthorized",
        kind: "nested",
      },
    ],
  },
];

export default ABIRewardDistributor;
