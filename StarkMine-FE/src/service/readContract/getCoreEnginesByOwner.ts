import { contracts } from "@/configs/contracts";
import { coreEngineContract, provider } from ".";
import { EventKeyEnum } from "@/type/common";
import { formattedContractAddress } from "@/utils/helper";
import {  shortString } from "starknet";
import getRemainingCoreEngineDurability from "./getRemainingCoreEngineDurability";

export const getEngineData = async (tokenId: number) => {
    const coreEngineInfo = await coreEngineContract.get_engine_info(tokenId);
    const durabilityPercent = await getRemainingCoreEngineDurability(tokenId, shortString.decodeShortString(coreEngineInfo.engine_type));
    return {
      tokenId: tokenId,
      attachedMiner: parseInt(coreEngineInfo.attached_miner),
      blocksUsed: parseInt(coreEngineInfo.blocks_used),
      durability: parseInt(coreEngineInfo.durability),
      efficiencyBonus: parseInt(coreEngineInfo.efficiency_bonus),
      engineType: shortString.decodeShortString(coreEngineInfo.engine_type),
      isActive: Boolean(coreEngineInfo.is_active),
      lastUsedBlock: parseInt(coreEngineInfo.last_used_block),
      durabilityPercent: durabilityPercent.data.remainingDurabilityPercent,
    };
};

export const getCoreEnginesByOwner = async (userAddress: string) => {
    let continuationToken = undefined;
    let allEvents: any[] = [];

    let ownedCoreEngines = new Set();
  
    try {
      do {
        const events = await provider.getEvents({
          chunk_size: 1000,
          continuation_token: continuationToken,
          from_block: { block_number: 0 },
          to_block: "latest",
          address: contracts.CoreEngine,
          keys: [[
            EventKeyEnum.EngineMinted,
            EventKeyEnum.Transfer,
            userAddress
          ]],
        });
        allEvents = allEvents.concat(events.events);
        continuationToken = events.continuation_token;
  
   
        events.events.forEach((event) => {
          const eventKey = event.keys[0]; 
          let eventName = "Unknown";

          if (
            eventKey ===
            EventKeyEnum.EngineMinted
          ) {
            eventName = "EngineMinted";
          } else if (
            eventKey ===
            EventKeyEnum.Transfer
          ) {
            eventName = "Transfer";
          }
  
          if (eventName === "EngineMinted" || eventName === "Transfer") {
            const userAddressFormatted = formattedContractAddress(userAddress);
            let fromAddress, toAddress, tokenId;
  
            if (eventName === "EngineMinted") {
              fromAddress = "0x0"; 
              toAddress = formattedContractAddress(event.keys[1]);
              tokenId = event.keys[2]; 
            } else if (eventName === "Transfer") {
     
              fromAddress = formattedContractAddress(event.keys[1]);
              toAddress = formattedContractAddress(event.keys[2]);
              tokenId = event.keys[3];
            }
  
            if (
              eventName === "EngineMinted" &&
              toAddress === userAddressFormatted
            ) {
              ownedCoreEngines.add(tokenId);
            } else if (eventName === "Transfer") {
              if (fromAddress === userAddressFormatted) {
                ownedCoreEngines.delete(tokenId);
              } else if (toAddress === userAddressFormatted) {
                ownedCoreEngines.add(tokenId);
              }
            }
          }
        });
      } while (continuationToken);
  
      const minerDetails = await Promise.all(
        Array.from(ownedCoreEngines).map(async (tokenId: any) => {
          const coreEngineInfo = await getEngineData(parseInt(tokenId, 16));
          return coreEngineInfo
        })
      );
  
      console.log("🚀 ~ getCoreEnginesByOwner ~ minerDetails:", minerDetails)
      return minerDetails; 
    } catch (error) {
      console.error("Error:", error);
      throw error;
    }
}