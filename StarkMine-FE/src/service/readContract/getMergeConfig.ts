import { mergeContract } from ".";
import { convertWeiToEther } from "@/utils/helper";

export const getMergeConfig = async (fromTier: string, toTier: string): Promise<{ baseSuccessRate: number, failureBonus: number, maxFailureBonus: number, costMine: number, costStrk: number }> => {
    const mergeConfig = await mergeContract.get_merge_config(fromTier, toTier);
    console.log("🚀 ~ getMergeConfig ~ mergeConfig:", {
        baseSuccessRate: Number(BigInt(mergeConfig.base_success_rate)) / 100,
        failureBonus: Number(BigInt(mergeConfig.failure_bonus)) / 100,
        maxFailureBonus: Number(BigInt(mergeConfig.max_failure_bonus)) / 100,
        costMine: Number(convertWeiToEther(mergeConfig.cost_mine)),
        costStrk: Number(convertWeiToEther(mergeConfig.cost_strk)),
    })
    return {
        baseSuccessRate: Number(BigInt(mergeConfig.base_success_rate)) / 100,
        failureBonus: Number(BigInt(mergeConfig.failure_bonus)) / 100,
        maxFailureBonus: Number(BigInt(mergeConfig.max_failure_bonus)) / 100,
        costMine: Number(convertWeiToEther(mergeConfig.cost_mine)),
        costStrk: Number(convertWeiToEther(mergeConfig.cost_strk)),
    }
}