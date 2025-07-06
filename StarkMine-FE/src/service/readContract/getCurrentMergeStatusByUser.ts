import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { mergeContract, minerContract } from ".";
import { getMergeConfig } from "./getMergeConfig";
import { convertWeiToEther } from "@/utils/helper";
const getTierConfig = async (tier: string) => {
    const tierConfig = await minerContract.get_tier_config(tier);
    return {
        baseHashPower: Number(BigInt(tierConfig.base_hash_power) / BigInt(1e12)),
        mintCostMine: Number(convertWeiToEther(tierConfig.mint_cost_mine)),
        mintCostStrk: Number(convertWeiToEther(tierConfig.mint_cost_strk)),
        mintedCount: Number(tierConfig.minted_count),
        supplyLimit: Number(tierConfig.supply_limit),
        tierBonus: Number(BigInt(tierConfig.tier_bonus)) / 100,
    };

}
const getCurrentMergeStatusByUser = async (address: string, fromTier: string, toTier: string): Promise<MessageBase> => {
    const currentSuccessRate = await mergeContract.get_current_success_rate(address, fromTier, toTier);

    const mergeConfig = await getMergeConfig(fromTier, toTier);

    const tierConfig = await getTierConfig(toTier);

    console.log("🚀 ~ getCurrentSuccessRate ~ currentSuccessRate:", {
        fromTier: fromTier,
        toTier: toTier,
        baseSuccessRate: mergeConfig.baseSuccessRate,
        successRateBonus: Number(BigInt(currentSuccessRate)) / 100 - mergeConfig.baseSuccessRate,
        costStrk: mergeConfig.costStrk,
        costMine: mergeConfig.costMine,
        tierConfig: tierConfig,
    })
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            fromTier: fromTier,
            toTier: toTier,
            baseSuccessRate: mergeConfig.baseSuccessRate,
            successRateBonus: Number(BigInt(currentSuccessRate)) / 100 - mergeConfig.baseSuccessRate,
            costStrk: mergeConfig.costStrk,
            costMine: mergeConfig.costMine,
            tierConfig: tierConfig

        }
    }
}

export default getCurrentMergeStatusByUser;