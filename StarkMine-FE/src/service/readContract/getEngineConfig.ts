import { convertWeiToEther } from "@/utils/helper";
import { coreEngineContract } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

export const getEngineConfig = async (engineType: string) => {
    const engineConfig = await coreEngineContract.get_engine_type_config(engineType);
   
    return {
        engineType: engineType,
        durability: Number(engineConfig.durability),
        mintCost: Number(convertWeiToEther(engineConfig.mint_cost)),
        repairCostBase: Number(convertWeiToEther(engineConfig.repair_cost_base)),
    }
}

export const getEngineTypeConfigs = async (): Promise<MessageBase> => {
    const engineTypes = ["Basic", "Elite", "Pro", "GIGA"];
    const engineConfigs = await Promise.all(engineTypes.map(engineType => getEngineConfig(engineType)));
    console.log("🚀 ~ getEngineTypeConfigs ~ engineConfigs:", engineConfigs)
    return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: engineConfigs,
    }
}