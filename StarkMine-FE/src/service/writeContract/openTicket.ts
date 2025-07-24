import {
  ErrorLevelEnum,
  EventKeyEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { provider } from "../readContract";
import { AccountInterface, CallData, uint256 } from "starknet";
import { contracts } from "@/configs/contracts";

const openTicket = async (
  account: AccountInterface,
  ticketId: number
): Promise<MessageBase> => {
  const tx = await account.execute({
    contractAddress: contracts.TicketSystem,
    entrypoint: "open_ticket",
    calldata: CallData.compile({ ticket_id: uint256.bnToUint256(ticketId) }),
  });

  const txR = await provider.waitForTransaction(tx.transaction_hash);

  if (txR.isSuccess()) {
    for (let i = 0; i < txR.events.length; i++) {
      if (txR.events[i].keys[0] == EventKeyEnum.MinerMinted) {
        console.log("🚀 ~ openTicket ~ txR.events[i]:", txR.events[i]);
        console.log("from:" + "0x0");
        console.log("to:" + txR.events[i].keys[1]);
        console.log("tokenId:" + txR.events[i].keys[2]);
        console.log(
          "spaceship:" +
            Buffer.from(
              txR.events[i].data[0].replace("0x", ""),
              "hex"
            ).toString("utf8")
        );

        return {
          status: StatusEnum.SUCCESS,
          message: MessageEnum.SUCCESS,
          level: ErrorLevelEnum.INFOR,
          data: {
            ticketId: ticketId,
            tier: Buffer.from(
              txR.events[i].data[0].replace("0x", ""),
              "hex"
            ).toString("utf8"),
            tokenId: parseInt(txR.events[i].keys[2], 16),
          },
        };
      }
    }
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.OPEN_TICKET_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        ticketId: ticketId,
      },
    };
  } else {
    return {
      status: StatusEnum.ERROR,
      message: MessageEnum.EXECUTE_FAILED,
      level: ErrorLevelEnum.WARNING,
      data: {
        ticketId: ticketId,
      },
    };
  }
};

export default openTicket;
