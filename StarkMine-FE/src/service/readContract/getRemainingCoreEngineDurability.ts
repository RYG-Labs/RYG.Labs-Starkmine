import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { coreEngineContract, engineDurabilityConfig } from ".";

const getRemainingCoreEngineDurability = async (coreEngineId: number, engineType: string): Promise<MessageBase> => {
  const maxDurability = engineDurabilityConfig.get(engineType);
  if (!maxDurability) return { status: StatusEnum.ERROR, message: MessageEnum.ENGINE_TYPE_NOT_FOUND, level: ErrorLevelEnum.WARNING, data: {
    remainingDurabilityBlocks: 0,
    remainingDurabilityPercent: 0
  } }
  console.log("🚀 ~ getRemainingCoreEngineDurability ~ maxDurability:", maxDurability)
  const remainingCoreEngineDurability = await coreEngineContract.get_engine_remaining_durability(coreEngineId)
  console.log("🚀 ~ getRemaningCoreEngineDurability ~ remainingCoreEngineDurability:", remainingCoreEngineDurability)

  const remainingDurabilityPercent = Number(remainingCoreEngineDurability) / Number(maxDurability) * 100;
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