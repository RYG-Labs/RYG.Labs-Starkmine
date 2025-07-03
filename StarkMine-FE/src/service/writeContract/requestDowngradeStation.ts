import { contracts } from "@/configs/contracts";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";

const requestDowngradeStation = async (account: AccountInterface , stationId: number, targetLevel: number): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.StationSystem,
      entrypoint: "request_downgrade",
      calldata: CallData.compile({ station_id: stationId, target_level: targetLevel }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          stationId: stationId,
          targetLevel: targetLevel,
        }
      }
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          stationId: stationId,
          targetLevel: targetLevel,
        }
      }
    };

  } catch (error: any) {
    console.log(error);
    return {
      status: StatusEnum.ERROR,
      message: error.message,
      level: ErrorLevelEnum.WARNING,
      data: {
        stationId: stationId,
        targetLevel: targetLevel,
      },
    }
  }
};

export default requestDowngradeStation;