import { contracts } from "@/configs/contracts";
import { ErrorLevelEnum, EventKeyEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";

const mintCoreEngine = async (account: AccountInterface, engineType: string): Promise<MessageBase> => {
  console.log(engineType);

  try {
    const tx = await account.execute({
      contractAddress: contracts.CoreEngine,
      entrypoint: "mint_engine",
      calldata: CallData.compile({ to: account.address, engine_type: engineType }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    if (receipt.isSuccess()) {
      let coreEngineId = undefined;

      receipt.events.forEach((event) => {
        const eventKey = event.keys[0];
        let eventName = "Unknown";

        if (
          eventKey ===
          EventKeyEnum.EngineMinted
        ) {
          eventName = "EngineMinted";
        }

        if (eventName === "EngineMinted") {
          coreEngineId = parseInt(event.keys[2], 16);
        }
      });

      if (!coreEngineId) {
        return {
          status: StatusEnum.ERROR,
          message: MessageEnum.ERROR,
          level: ErrorLevelEnum.WARNING,
          data: {
            engineType: engineType,
          },
        }
      }


      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          engineType: engineType,
          coreEngineId: coreEngineId,
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