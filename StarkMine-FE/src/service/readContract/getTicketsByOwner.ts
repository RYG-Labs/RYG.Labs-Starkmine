import { contracts } from "@/configs/contracts";
import { provider } from ".";
import {
  ErrorLevelEnum,
  EventKeyEnum,
  MessageBase,
  MessageEnum,
  StatusEnum,
} from "@/type/common";
import { formattedContractAddress } from "@/utils/helper";

const getTicketsByOwner = async (userAddress: string): Promise<MessageBase> => {
  let continuationToken = undefined;
  let allEvents: any[] = [];

  let ownedTickets = new Set<string>();

  try {
    do {
      const events = await provider.getEvents({
        chunk_size: 1000,
        continuation_token: continuationToken,
        from_block: { block_number: 0 },
        to_block: "latest",
        address: contracts.TicketNFT,
        keys: [[EventKeyEnum.Transfer, userAddress]],
      });
      allEvents = allEvents.concat(events.events);
      continuationToken = events.continuation_token;

      events.events.forEach((event) => {
        console.log("🚀 ~ getTicketsByOwner ~ event:", event);
        const eventKey = event.keys[0];
        let eventName = "Unknown";

        if (eventKey === EventKeyEnum.Transfer) {
          eventName = "Transfer";
        }

        if (eventName === "Transfer") {
          const userAddressFormatted = formattedContractAddress(userAddress);
          let fromAddress, toAddress, tokenId;

          fromAddress = formattedContractAddress(event.keys[1]);
          toAddress = formattedContractAddress(event.keys[2]);
          tokenId = event.keys[3];

          if (fromAddress === userAddressFormatted) {
            ownedTickets.delete(tokenId);
          } else if (toAddress === userAddressFormatted) {
            ownedTickets.add(tokenId);
          }
        }
      });
    } while (continuationToken);
    console.log(
      Array.from(ownedTickets).map((tokenId) => parseInt(tokenId, 16))
    );

    return {
      status: StatusEnum.SUCCESS,
      message: MessageEnum.SUCCESS,
      level: ErrorLevelEnum.INFOR,
      data: {
        ticketIds: Array.from(ownedTickets).map((tokenId) =>
          parseInt(tokenId, 16)
        ),
      },
    };
  } catch (error) {
    console.error("Lỗi:", error);
    throw error;
  }
};

export default getTicketsByOwner;
