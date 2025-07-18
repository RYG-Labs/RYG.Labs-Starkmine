import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";

const mintMiner = async (account: AccountInterface, tier: string) => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.MinerNFT,
      entrypoint: "mint_miner",
      calldata: CallData.compile({ to: account.address, tier: tier }),
    });

    const receipt = await provider.waitForTransaction(tx.transaction_hash);

    console.log(receipt);

    if (receipt.isSuccess()) {
      return {
        status: StatusEnum.SUCCESS,
        message: MessageEnum.SUCCESS,
        level: ErrorLevelEnum.INFOR,
        data: {
          tier: tier,
        },
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: {
          tier: tier,
        },
      };
    }
  } catch (error: any) {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        tier: tier,
      },
    };
  }
};

export default mintMiner;
