import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";

const repairCoreEngine = async (
  account: AccountInterface,
  engineId: number,
  durabilityPercentToRestore: number
): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.CoreEngine,
      entrypoint: "repair_engine",
      calldata: CallData.compile({
        engine_id: uint256.bnToUint256(engineId),
        durability_to_restore: durabilityPercentToRestore,
      }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          engineId: engineId,
          durabilityToRestore: durabilityPercentToRestore,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.REPAIR_CORE_ENGINE_FAILED,
        level: ErrorLevelEnum.WARNING,
        data: {
          engineId: engineId,
          durabilityToRestore: durabilityPercentToRestore,
        },
      };
    }
  } catch (error: any) {
    console.log(MessageEnum.EXECUTE_FAILED);
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        engineId: engineId,
        durabilityToRestore: durabilityPercentToRestore,
      },
    };
  }
};

export default repairCoreEngine;
