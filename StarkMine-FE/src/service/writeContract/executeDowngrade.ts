import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";
import canExecuteDowngrade from "../readContract/canExecuteDowngrade";

const executeDowngrade = async (
  account: AccountInterface,
  stationId: number
) => {
  try {
    const canExecute = await canExecuteDowngrade(account.address, stationId);

    if (!canExecute.data.canExecute) {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.CANNOT_EXECUTE_DOWNGRADE_RIGHT_NOW,
        level: ErrorLevelEnum.WARNING,
        data: {
          stationId: stationId,
        },
      };
    }

    const tx = await account.execute({
      contractAddress: contracts.StationSystem,
      entrypoint: "execute_downgrade",
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

export default executeDowngrade;
