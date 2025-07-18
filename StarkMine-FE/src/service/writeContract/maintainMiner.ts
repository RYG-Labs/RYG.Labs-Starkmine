import { contracts } from "@/configs/contracts";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";

const maintainMiner = async (
  account: AccountInterface,
  minerId: number
): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.MinerNFT,
      entrypoint: "maintain_miner",
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
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          minerId: minerId,
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

export default maintainMiner;
