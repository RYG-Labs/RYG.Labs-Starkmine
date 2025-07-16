import { RpcProvider, Account, config, logger, ETransactionVersion } from 'starknet';
import * as dotenv from 'dotenv';
import { ABIRewardDistributor } from './abis/AbiRewardDistributor';

// Load environment variables
dotenv.config();

// Configuration
const TARGET_NETWORK = process.env.TARGET_NETWORK || 'sepolia';
const CONTRACT_ADDRESS = process.env.REWARD_DISTRIBUTOR_ADDRESS || 'YOUR_CONTRACT_ADDRESS';
const PRIVATE_KEY = process.env.PRIVATE_KEY || 'YOUR_PRIVATE_KEY';
const ACCOUNT_ADDRESS = process.env.ACCOUNT_ADDRESS || 'YOUR_ACCOUNT_ADDRESS';
const RPC_URL = TARGET_NETWORK === "sepolia" 
  ? process.env.NEXT_PUBLIC_TESTNET_STARKNET_NODE_URL 
  : process.env.NEXT_PUBLIC_MAINNET_STARKNET_NODE_URL;
const BLOCK_INTERVAL = 5; // Call update_reward every 5 blocks
const POLL_INTERVAL_MS = 30_000; // Poll every 30 seconds
const MAX_RETRIES = 3; // Maximum retry attempts for failed transactions

// Initialize provider and account
const provider = new RpcProvider({
    nodeUrl: RPC_URL,
    specVersion: '0.7.1',
  });
  config.set('legacyMode', true);
  logger.setLogLevel('ERROR');

const account = new Account(
   provider,
  ACCOUNT_ADDRESS,
  PRIVATE_KEY,
  undefined,
  ETransactionVersion.V2
);


// const rewardContract = new Contract(ABIRewardDistributor, CONTRACT_ADDRESS, provider);
// rewardContract.connect(account);

// Store the last block number when update_reward was called
let lastUpdateBlock: number | null = null;

// Function to check block number and call update_reward
async function checkAndUpdateReward() {
  try {
    // Get current block number
    const block = await provider.getBlockNumber();
    const currentBlockNumber = Number(block);
    console.log(`[${new Date().toISOString()}] Current block number: ${currentBlockNumber}`);

    // If this is the first run, initialize lastUpdateBlock
    if (lastUpdateBlock === null) {
      lastUpdateBlock = currentBlockNumber;
      console.log(`[${new Date().toISOString()}] Initialized last update block: ${lastUpdateBlock}`);
    }

    // Check if at least 5 blocks have passed
    if (currentBlockNumber >= lastUpdateBlock + BLOCK_INTERVAL) {
      console.log(`[${new Date().toISOString()}] Calling update_reward at block ${currentBlockNumber}...`);

      const call = {
        contractAddress: CONTRACT_ADDRESS,
        entrypoint: 'update_reward',
        calldata: [],
      };

      // Ước tính phí
      const resourceBounds = {
        l1_gas: {
            max_amount: "0x100000000000", // 65,536, đủ lớn để bao quát
            max_price_per_unit: "0x15000000000", // ~90,000,000,000 wei
          },
          l2_gas: {
            max_amount: "0x800000000000", // 524,288, đủ cho yêu cầu tối thiểu
            max_price_per_unit: "0x100000000",
          },
          l1_data_gas: {
            max_amount: "0x1000000000000",
            max_price_per_unit: "0x100000000",
          },
      };
      const feeEstimate = await account.estimateFee(call, {
        version: ETransactionVersion.V3,
        resourceBounds: resourceBounds,
      });
      console.log("🚀 ~ checkAndUpdateReward ~ feeEstimate:", feeEstimate)


      // Prepare and send transaction with retries
      let retryCount = 0;
      while (retryCount < MAX_RETRIES) {
        try {
          const result = await account.execute({
            contractAddress: CONTRACT_ADDRESS,
            entrypoint: 'update_reward',
            calldata: [],
            
          },{
            version: "0x3",
            resourceBounds: feeEstimate.resourceBounds,
          }
        );
          const txR = await provider.waitForTransaction(result.transaction_hash);
          if (txR.isSuccess()) {
            console.log("ok");
          }
          lastUpdateBlock = currentBlockNumber;
          break;
        } catch (error) {
          retryCount++;
          console.error(`[${new Date().toISOString()}] Retry ${retryCount}/${MAX_RETRIES}:`, error);
          if (retryCount === MAX_RETRIES) throw error;
          await new Promise(resolve => setTimeout(resolve, 5000)); // Wait 5 seconds before retry
        }
      }
    } else {
      console.log(`[${new Date().toISOString()}] Not enough blocks passed. Last update: ${lastUpdateBlock}, Current: ${currentBlockNumber}`);
    }
  } catch (error) {
    console.error(`[${new Date().toISOString()}] Error in checkAndUpdateReward:`, error);
  }
}

// Main loop to poll block number
async function startKeeper() {
  console.log(`[${new Date().toISOString()}] Starting keeper...`);
  while (true) {
    await checkAndUpdateReward();
    await new Promise(resolve => setTimeout(resolve, POLL_INTERVAL_MS));
  }
}

// Start the keeper
startKeeper().catch(error => {
  console.error(`[${new Date().toISOString()}] Keeper failed:`, error);
  process.exit(1);
});