import {
  Account,
  config,
  ETransactionVersion,
  logger,
  RpcProvider,
} from "starknet";
import * as dotenv from "dotenv";
// Load environment variables
dotenv.config();

const PRIVATE_KEY = process.env.PRIVATE_KEY || "YOUR_PRIVATE_KEY";
const ACCOUNT_ADDRESS = process.env.ACCOUNT_ADDRESS || "YOUR_ACCOUNT_ADDRESS";
// Configuration
export const REWARD_CONTRACT_ADDRESS =
  process.env.REWARD_DISTRIBUTOR_ADDRESS || "YOUR_CONTRACT_ADDRESS";
export const CORE_ENGINE_CONTRACT_ADDRESS =
  process.env.CORE_ENGINE_ADDRESS || "YOUR_CONTRACT_ADDRESS";
export const MINER_ADDRESS =
  process.env.MINER_ADDRESS || "YOUR_CONTRACT_ADDRESS";
export const BLOCK_INTERVAL = 10; // Call update_reward every 5 blocks
export const POLL_INTERVAL_MS = 10_000; // Poll every 30 seconds
export const MAX_RETRIES = 3; // Maximum retry attempts for failed transactions

export const RPC_URL =
  process.env.TARGET_NETWORK === "sepolia"
    ? process.env.NEXT_PUBLIC_TESTNET_STARKNET_NODE_URL
    : process.env.NEXT_PUBLIC_MAINNET_STARKNET_NODE_URL;

export const provider = new RpcProvider({
  nodeUrl: RPC_URL,
  specVersion: "0.8.1",
});
config.set("legacyMode", true);
logger.setLogLevel("ERROR");

export const account = new Account(
  provider,
  ACCOUNT_ADDRESS,
  PRIVATE_KEY,
  undefined,
  ETransactionVersion.V2
);

export const getAbi = async (contractAddress: string) => {
  const { abi } = await provider.getClassAt(contractAddress);
  return abi;
};
