import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { coreEngineContract } from ".";

const getEngineCurrentEfficiencyBonus = async (engineId: number): Promise<MessageBase> => {
    const remainingEfficiencyBonus = await coreEngineContract.get_current_efficiency_bonus(engineId);
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
            currentEfficiencyBonus: Number(remainingEfficiencyBonus),
        },
    }
}

export default getEngineCurrentEfficiencyBonus;