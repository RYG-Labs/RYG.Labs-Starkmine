import { ETransactionVersion, Contract, CallData, uint256 } from 'starknet';
import * as dotenv from 'dotenv';
import { account, BLOCK_INTERVAL, CORE_ENGINE_CONTRACT_ADDRESS, getAbi, MAX_RETRIES, MINER_ADDRESS, POLL_INTERVAL_MS, provider, REWARD_CONTRACT_ADDRESS } from './helpers';
import { EventKeyEnum } from './types';

// Load environment variables
dotenv.config();

let previousBlockNumber: number | null = null;
let currentBlockNumber: number;
let lastUpdateRewardBlock: number;
let lastUpdateListCoreEnginesExpired: number;


// support function
async function getListEnginesExpired(): Promise<number[]> {
    const coreEngineContract = new Contract(await getAbi(CORE_ENGINE_CONTRACT_ADDRESS), CORE_ENGINE_CONTRACT_ADDRESS, provider);
    const listEnginesExpired = await coreEngineContract.get_engines_expired();
    const listEnginesExpiredFormatted = listEnginesExpired.map((engineId: bigint) => Number(engineId));
    return listEnginesExpiredFormatted;
}

// main function
async function ExtinguishMinersWithExpiredCoreEngines() {
      lastUpdateListCoreEnginesExpired = currentBlockNumber;    

      // get list core engines expired
      const listEnginesExpired = await getListEnginesExpired();
      if(listEnginesExpired.length === 0) return;

      // get miners with expired core engines
      let continuationToken = undefined;
      let allEvents: any[] = [];
      let minerNeedToExtinguish = new Set<number>();
     
    
      try {
        do {
          const events = await provider.getEvents({
            chunk_size: 1000,
            continuation_token: continuationToken,
            from_block: { block_number: 0 },
            to_block: "latest",
            address: MINER_ADDRESS,
            keys: [[
              EventKeyEnum.MinerIgnited,
              EventKeyEnum.MinerExtinguished,
              "0x1"
            ]],
          });
          allEvents = allEvents.concat(events.events);
          continuationToken = events.continuation_token;
    
     
          events.events.forEach((event: any) => {
            const eventKey = event.keys[0]; 
            let eventName = "Unknown";
  
            if (
              eventKey ===
              EventKeyEnum.MinerExtinguished
            ) {
              eventName = "MinerExtinguished";
            } else if (
              eventKey ===
              EventKeyEnum.MinerIgnited
            ) {
              eventName = "MinerIgnited";
            }
            
            if(eventName === "MinerIgnited") {
              const minerId = parseInt(event.keys[1], 16);
              const coreEngineId = parseInt(event.keys[3], 16);
              if(listEnginesExpired.includes(coreEngineId)) {
                minerNeedToExtinguish.add(minerId);
              }
              
            }
            if(eventName === "MinerExtinguished") {
              const minerId = parseInt(event.keys[1], 16);
              minerNeedToExtinguish.delete(minerId);
            }
          });
        } while (continuationToken);
  
        if(minerNeedToExtinguish.size === 0) {
          console.log("No miner to extinguish at block " + currentBlockNumber);
          return
        };
      
        // extinguish miners
        const calls: {contractAddress: string, entrypoint: string, calldata: any}[] = [];
        Array.from(minerNeedToExtinguish).forEach((minerId: number) => {
          calls.push({
            contractAddress: MINER_ADDRESS,
            entrypoint: "extinguish_miner",
            calldata: CallData.compile({ token_id: uint256.bnToUint256(minerId) }),
          })
        });
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
              const feeEstimate = await account.estimateFee(calls, {
                version: ETransactionVersion.V3,
                resourceBounds: resourceBounds,
              });
  
        const result = await account.execute(calls,
          {
            version: "0x3",
            resourceBounds: feeEstimate.resourceBounds,
          }
        
      );
        const txR = await provider.waitForTransaction(result.transaction_hash);
        if (txR.isSuccess()) {
          console.log("extinguish miner success " + minerNeedToExtinguish + " ", result.transaction_hash);
        } else {
          console.error("extinguish miner failed");
        }
      } catch (error) {
        console.error("Error:", error);
        throw error;
      }
}
async function checkAndUpdateReward() {
  try {
    // Check if at least 5 blocks have passed
    if (currentBlockNumber >= lastUpdateRewardBlock + BLOCK_INTERVAL) {
      const call = {
        contractAddress: REWARD_CONTRACT_ADDRESS,
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


      // Prepare and send transaction with retries
      let retryCount = 0;
      while (retryCount < MAX_RETRIES) {
        try {
          const result = await account.execute({
            contractAddress: REWARD_CONTRACT_ADDRESS,
            entrypoint: 'update_reward',
            calldata: [],
            
          },{
            version: "0x3",
            resourceBounds: feeEstimate.resourceBounds,
          }
        );
          const txR = await provider.waitForTransaction(result.transaction_hash);
          if (txR.isSuccess()) {
            console.log("update reward success at block " + currentBlockNumber);
          }
          lastUpdateRewardBlock = currentBlockNumber;
          break;
        } catch (error) {
          retryCount++;
          console.error(`[${new Date().toISOString()}] Retry ${retryCount}/${MAX_RETRIES}:`, error);
          if (retryCount === MAX_RETRIES) throw error;
          await new Promise(resolve => setTimeout(resolve, 5000)); // Wait 5 seconds before retry
        }
      }
    } else {
      console.log(`[${new Date().toISOString()}] Not enough blocks passed. Last update: ${lastUpdateRewardBlock}, Current: ${currentBlockNumber}`);
    }
  } catch (error) {
    console.error(`[${new Date().toISOString()}] Error in checkAndUpdateReward:`, error);
  }
}

// start function
async function init() {
  const rewardContract = new Contract(await getAbi(REWARD_CONTRACT_ADDRESS), REWARD_CONTRACT_ADDRESS, provider);
  lastUpdateRewardBlock = Number(await rewardContract.get_latest_updated_block());
  lastUpdateListCoreEnginesExpired = Number(await provider.getBlockNumber()) - 1;

  // Cập nhật block number và gọi các hàm khi block thay đổi
  setInterval(async () => {
    try {
      const block = await provider.getBlockNumber();
      currentBlockNumber = Number(block);

      // Kiểm tra nếu block number thay đổi
      if (previousBlockNumber !== currentBlockNumber && previousBlockNumber !== null) {
        console.log(`[${new Date().toISOString()}] Block number changed: ${previousBlockNumber} -> ${currentBlockNumber}`);
        
        // Gọi hai hàm khi block thay đổi
        await Promise.all([
          ExtinguishMinersWithExpiredCoreEngines(),
          checkAndUpdateReward()
        ]);
      }
      
      // Cập nhật previousBlockNumber
      previousBlockNumber = currentBlockNumber;
    } catch (error) {
      console.error(`[${new Date().toISOString()}] Error in block polling:`, error);
    }
  }, POLL_INTERVAL_MS);
}  

async function main() {
  await init();
}

main().catch(error => { console.error(error) });