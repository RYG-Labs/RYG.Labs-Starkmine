import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";

const claimPendingReward = async (
  account: AccountInterface
): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.RewardDistributor,
      entrypoint: "claim_rewards",
      calldata: CallData.compile({ address: account.address }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          address: account.address,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          address: account.address,
        },
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        address: account.address,
      },
    };
  }
};

export default claimPendingReward;
