const ABITicketSystem = [
  {
    type: "impl",
    name: "TicketSystemImpl",
    interface_name: "starkmine::mining::ticket_system::ITicketSystem",
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
    type: "interface",
    name: "starkmine::mining::ticket_system::ITicketSystem",
    items: [
      {
        type: "function",
        name: "open_ticket",
        inputs: [
          {
            name: "ticket_id",
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
        name: "set_miner_nft_address",
        inputs: [
          {
            name: "address",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "set_ticket_nft_address",
        inputs: [
          {
            name: "address",
            type: "core::starknet::contract_address::ContractAddress",
          },
        ],
        outputs: [],
        state_mutability: "external",
      },
      {
        type: "function",
        name: "set_owner",
        inputs: [
          {
            name: "owner",
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
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::ticket_system::TicketSystem::TicketOpened",
    kind: "struct",
    members: [
      {
        name: "ticket_id",
        type: "core::integer::u256",
        kind: "key",
      },
      {
        name: "miner_id",
        type: "core::integer::u256",
        kind: "key",
      },
      {
        name: "owner",
        type: "core::starknet::contract_address::ContractAddress",
        kind: "key",
      },
    ],
  },
  {
    type: "event",
    name: "starkmine::mining::ticket_system::TicketSystem::Event",
    kind: "enum",
    variants: [
      {
        name: "TicketOpened",
        type: "starkmine::mining::ticket_system::TicketSystem::TicketOpened",
        kind: "nested",
      },
    ],
  },
];

export default ABITicketSystem;
