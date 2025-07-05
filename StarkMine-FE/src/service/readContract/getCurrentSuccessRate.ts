import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { mergeContract } from ".";
import { getMergeConfig } from "./getMergeConfig";

const getCurrentSuccessRate = async (address: string, fromTier: string, toTier: string): Promise<MessageBase> => {
    const currentSuccessRate = await mergeContract.get_current_success_rate(address, fromTier, toTier);
   
    const mergeConfig = await getMergeConfig(fromTier, toTier);

    console.log("🚀 ~ getCurrentSuccessRate ~ currentSuccessRate:", {
        fromTier: fromTier,
        toTier: toTier,
        baseSuccessRate: mergeConfig.baseSuccessRate,
        successRateBonus: Number(BigInt(currentSuccessRate)) / 100 - mergeConfig.baseSuccessRate,
        costStrk: mergeConfig.costStrk,
        costMine: mergeConfig.costMine,
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
        }
    }
}

export default getCurrentSuccessRate;