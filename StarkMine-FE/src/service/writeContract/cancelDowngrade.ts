import { contracts } from "@/configs/contracts";
import { CallData } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";

const cancelDowngrade = async (account: any, stationId: number) => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.StationSystem,
      entrypoint: "cancel_downgrade",
      calldata: CallData.compile({ station_id: stationId }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          stationId: stationId,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          stationId: stationId,
        },
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        stationId: stationId,
      },
    };
  }
};

export default cancelDowngrade;
