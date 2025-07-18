import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { getEngineData } from "../readContract/getCoreEnginesByOwner";

export const igniteMiner = async (
  account: AccountInterface,
  minerId: number,
  coreEngineId: number
): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.MinerNFT,
      entrypoint: "ignite_miner",
      calldata: CallData.compile({
        token_id: uint256.bnToUint256(minerId),
        core_engine_id: uint256.bnToUint256(coreEngineId),
      }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      const coreEngineInfo = await getEngineData(coreEngineId);
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          minerId: minerId,
          coreEngine: coreEngineInfo,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {},
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {},
    };
  }
};
