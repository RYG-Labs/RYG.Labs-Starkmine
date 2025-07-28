import { contracts } from "@/configs/contracts";
import {
  ErrorLevelEnum,
  EventKeyEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";

const mintTicket = async (account: AccountInterface): Promise<MessageBase> => {
  try {
    const tx = await account.execute({
      contractAddress: contracts.TicketNFT,
      entrypoint: "mint_ticket",
      calldata: CallData.compile({ to: account.address }),
    });
    const txR = await provider.waitForTransaction(tx.transaction_hash);
    console.log("🚀 ~ mintTicket ~ txR:", txR);

    if (txR.isSuccess()) {
      for (let i = 0; i < txR.events.length; i++) {
        let key = txR.events[i].keys[0];
        console.log(txR.events[i]);

        if (key == EventKeyEnum.Transfer) {
          return {
            status: StatusEnum.SUCCESS,
            message: MessageEnum.SUCCESS,
            level: ErrorLevelEnum.INFOR,
            data: {
              ticketId: parseInt(txR.events[i].keys[3], 16),
            },
          };
        }
      }
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.MINT_TICKET_FAILED,
        level: ErrorLevelEnum.WARNING,
        data: {},
      };
    } else {
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.EXECUTE_FAILED,
        level: ErrorLevelEnum.WARNING,
        data: {},
      };
    }
  } catch (error) {
    console.log(error);
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {},
    };
  }
};

export default mintTicket;
