import { contracts } from "@/configs/contracts";
import {
  ErrorLevelEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { AccountInterface } from "starknet";
import { provider } from "../readContract";

const recordLogin = async (account: AccountInterface): Promise<MessageBase> => {
  try {
    const result = await account.execute({
      contractAddress: contracts.RewardDistributor,
      entrypoint: "record_login",
      calldata: [],
    });

    const txR = await provider.waitForTransaction(result.transaction_hash);

    if (txR.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {},
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.RECORD_LOGIN_FAILED,
        level: ErrorLevelEnum.WARNING,
        data: {},
      };
    }
  } catch (error) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {},
    };
  }
};

export default recordLogin;
