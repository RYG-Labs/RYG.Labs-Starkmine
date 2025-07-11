import {shortString} from "starknet";
import { contracts } from "@/configs/contracts";
import {formattedContractAddress} from "../../utils/helper";
import { minerContract, provider } from ".";
import { EventKeyEnum } from "@/type/common";

export const getMinerData = async (tokenId: number): Promise<any> => {
    const minerInfo = await minerContract.get_miner_info(tokenId);
    return {
      tokenId: tokenId,
      tier: shortString.decodeShortString(minerInfo.tier),
      hashPower: Number(BigInt(minerInfo.hash_power) / BigInt(1e12)), // 
      level: Number(minerInfo.level),
      efficiency: Number(minerInfo.efficiency),
      lastMaintenance: minerInfo.last_maintenance.toString(),
      coreEngineId: parseInt(minerInfo.core_engine_id, 16),
      isIgnited: Boolean(minerInfo.is_ignited),
    };
};
  
export const getMinersByOwner = async (userAddress: string) => {
    let continuationToken = undefined;
    let allEvents: any[] = [];

    let ownedNFTs = new Set();
  
    try {
      do {
        const events = await provider.getEvents({
          chunk_size: 1000,
          continuation_token: continuationToken,
          from_block: { block_number: 0 },
          to_block: "latest",
          address: contracts.MinerNFT,
          keys: [[
            EventKeyEnum.MinerMinted,
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
            EventKeyEnum.MinerMinted
          ) {
            eventName = "MinerMinted";
          } else if (
            eventKey ===
            EventKeyEnum.Transfer
          ) {
            eventName = "Transfer";
          }
  
          if (eventName === "MinerMinted" || eventName === "Transfer") {
            const userAddressFormatted = formattedContractAddress(userAddress);
            let fromAddress, toAddress, tokenId;
  
            if (eventName === "MinerMinted") {
              fromAddress = "0x0"; 
              toAddress = formattedContractAddress(event.keys[1]);
              tokenId = event.keys[2]; 
            } else if (eventName === "Transfer") {
     
              fromAddress = formattedContractAddress(event.keys[1]);
              toAddress = formattedContractAddress(event.keys[2]);
              tokenId = event.keys[3];
            }
  
            if (
              eventName === "MinerMinted" &&
              toAddress === userAddressFormatted
            ) {
              ownedNFTs.add(tokenId);
            } else if (eventName === "Transfer") {
              if (fromAddress === userAddressFormatted) {
                ownedNFTs.delete(tokenId);
              } else if (toAddress === userAddressFormatted) {
                ownedNFTs.add(tokenId);
              }
            }
          }
        });
      } while (continuationToken);
  
      const minerDetails = await Promise.all(
        Array.from(ownedNFTs).map(async (tokenId: any) => {
          const minerInfo = await getMinerData(parseInt(tokenId, 16));
          return minerInfo;
        })
      );
      console.log("🚀 ~ getPastEvents ~ minerDetails:", minerDetails);
  
      return minerDetails; 
    } catch (error) {
      console.error("Lỗi:", error);
      throw error;
    }
}
  