import { contracts } from "@/configs/contracts";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { getMinerData } from "../readContract/getMinersByOwner";
import { getEngineData } from "../readContract/getCoreEnginesByOwner";

const extinguishMiner = async (
  account: AccountInterface,
  minerId: number
): Promise<MessageBase> => {
  try {
    const minerInfo = await getMinerData(minerId);
    const engineInfo = await getEngineData(minerInfo.coreEngineId);

    const tx = await account.execute({
      contractAddress: contracts.MinerNFT,
      entrypoint: "extinguish_miner",
      calldata: CallData.compile({ token_id: uint256.bnToUint256(minerId) }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          minerId: minerId,
          coreEngine: engineInfo,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.EXTINGUISH_MINER_FAILED,
        level: ErrorLevelEnum.WARNING,
        data: {
          minerId: minerId,
          coreEngine: engineInfo,
        },
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        minerId: minerId,
      },
    };
  }
};

export default extinguishMiner;
