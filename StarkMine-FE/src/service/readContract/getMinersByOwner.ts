import {
    Contract,
    shortString,
  } from "starknet";
  import { contracts } from "@/configs/contracts";
  import {
    formattedContractAddress,
  } from "../../utils/helper";
  import { ABI_MINER_NFT } from "@/type/ABI_MINER_NFT";
  import { provider } from ".";

const getMinerData = async (tokenId: string) => {
    const MinerNFTContract = new Contract(
      ABI_MINER_NFT,
      contracts.MinerNFT,
      provider
    );
    const minerInfo = await MinerNFTContract.get_miner_info(tokenId);
    return minerInfo;
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
            "0x270f83cc00ac131de21fbbf4fa173d136a8f7941b5399a881cd17c399164afc", // MinerMinted
             "0x99cd8bde557814842a3121e8ddfd433a539b8c9f14bf31ebf108d12e6196e9", // Transfer
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
            "0x270f83cc00ac131de21fbbf4fa173d136a8f7941b5399a881cd17c399164afc"
          ) {
            eventName = "MinerMinted";
          } else if (
            eventKey ===
            "0x99cd8bde557814842a3121e8ddfd433a539b8c9f14bf31ebf108d12e6196e9"
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
          const minerInfo = await getMinerData(tokenId);
          return {
            token_id: shortString.decodeShortString(tokenId),
            tier: shortString.decodeShortString(minerInfo.tier),
            hash_power: minerInfo.hash_power.toString(),
            level: Number(minerInfo.level),
            efficiency: Number(minerInfo.efficiency),
            last_maintenance: minerInfo.last_maintenance.toString(),
            core_engine_id: shortString.decodeShortString(minerInfo.core_engine_id),
            is_ignited: Boolean(minerInfo.is_ignited),
          };
        })
      );
      console.log("🚀 ~ getPastEvents ~ minerDetails:", minerDetails);
  
      return minerDetails; 
    } catch (error) {
      console.error("Lỗi:", error);
      throw error;
    }
}
  