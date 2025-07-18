import { contracts } from "@/configs/contracts";
import { CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";

const defuseEngine = async (account: any, engineId: number) => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.CoreEngine,
      entrypoint: "defuse_engine",
      calldata: CallData.compile({ engine_id: uint256.bnToUint256(engineId) }),
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
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          engineId: engineId,
        },
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        engineId: engineId,
      },
    };
  }
};

export default defuseEngine;
