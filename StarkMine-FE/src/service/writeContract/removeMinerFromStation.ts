import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";

const removeMinerFromStation = async (
  account: AccountInterface,
  stationId: number,
  minerSlot: number
): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.StationSystem,
      entrypoint: "remove_miner_from_station",
      calldata: CallData.compile({
        station_id: stationId,
        miner_slot: minerSlot,
      }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          minerSlot: minerSlot,
          stationId: stationId,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          minerSlot: minerSlot,
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
        minerSlot: minerSlot,
        stationId: stationId,
      },
    };
  }
};

export default removeMinerFromStation;
