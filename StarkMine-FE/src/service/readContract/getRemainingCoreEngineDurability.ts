import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { coreEngineContract } from ".";
import { getEngineData } from "./getCoreEnginesByOwner";

const getRemainingCoreEngineDurability = async (coreEngineId: number, engineType: string): Promise<MessageBase> => {
  const coreEngineInfo = await getEngineData(coreEngineId);

  const remainingCoreEngineDurability = await coreEngineContract.get_engine_remaining_durability(coreEngineId)

  const remainingDurabilityPercent = Number(remainingCoreEngineDurability) / Number(coreEngineInfo.durability) * 100;
  console.log("🚀 ~ getRemainingCoreEngineDurability ~ remainingDurabilityPercent:", remainingDurabilityPercent)

  return {
    status: StatusEnum.SUCCESS,
    message: MessageEnum.SUCCESS,
    level: ErrorLevelEnum.INFOR,
    data: {
      remainingDurabilityBlocks: Number(remainingCoreEngineDurability),
      remainingDurabilityPercent: remainingDurabilityPercent
    }
  }
};

export default getRemainingCoreEngineDurability;