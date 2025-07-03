import { contracts } from "@/configs/contracts";
import { ABI_MINER_NFT } from "@/type/ABI_MINER_NFT";
import { Contract } from "starknet";
import { provider } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { convertWeiToEther } from "@/utils/helper";

const minerContract = new Contract(ABI_MINER_NFT, contracts.MinerNFT, provider)

const getTierConfig = async (tier: string) => {
    const tierConfig = await  minerContract.get_tier_config(tier);
    return {
        baseHashPower: Number(BigInt(tierConfig.base_hash_power) / BigInt(1e12)) ,
        mintCostMine: Number(convertWeiToEther(tierConfig.mint_cost_mine)),
        mintCostStrk: Number(convertWeiToEther(tierConfig.mint_cost_strk)),
        mintedCount: Number(tierConfig.minted_count),
        supplyLimit: Number(tierConfig.supply_limit),
        tierBonus: Number(BigInt(tierConfig.tier_bonus)),
    };

}

const getTiersConfig = async (): Promise<MessageBase> => {
  try {
    const listTiers = ["Basic", "Elite", "Pro", "GIGA"];
    const tiersConfig = await Promise.all(listTiers.map((tier) => getTierConfig(tier)));
    console.log("🚀 ~ getTiersConfig ~ tiersConfig:", tiersConfig)
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: tiersConfig,
    }
  } catch (error) {
    console.log(error);
    
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.ERROR,
      level: ErrorLevelEnum.WARNING,
      data: [],
    }
  }
};

export default getTiersConfig;