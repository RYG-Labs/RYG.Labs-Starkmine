import { contracts } from "@/configs/contracts";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";

const mintCoreEngine = async (account: AccountInterface, engineType: string): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.CoreEngine,
      entrypoint: "mint_engine",
      calldata: CallData.compile({ to: account.address, engine_type: engineType }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          engineType: engineType,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          engineType: engineType,
        },
      };
    }
  } catch (error: any) {
    console.log(error);
    return {
        status: StatusEnum.ERROR,
        message: error.message,
        level: ErrorLevelEnum.WARNING,
        data: {
          engineType: engineType,
        },
    }
  }
};

export default mintCoreEngine;