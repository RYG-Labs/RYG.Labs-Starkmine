import { contracts } from "@/configs/contracts";
import { provider, ticketNftContract } from ".";
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
  let unlockedTickets = new Set<string>();
  let totalSupply = 0;
  let maxSupply = 0;

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
        const eventKey = event.keys[0];
        let eventName = "Unknown";

        if (eventKey === EventKeyEnum.Transfer) {
          eventName = "Transfer";
        }

        if (eventName === "Transfer") {
          // owned tickets
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

          // unlocked tickets
          if (
            fromAddress === userAddressFormatted &&
            toAddress === formattedContractAddress("0x0")
          ) {
            unlockedTickets.add(tokenId);
          }
        }
      });
    } while (continuationToken);

    maxSupply = await ticketNftContract.max_supply();
    totalSupply = await ticketNftContract.total_supply();

    return {
      status: StatusEnum.SUCCESS,
      message: MessageEnum.SUCCESS,
      level: ErrorLevelEnum.INFOR,
      data: {
        ownedTicketIds: Array.from(ownedTickets).map((tokenId) =>
          parseInt(tokenId, 16)
        ),
        unlockedTicketsIds: Array.from(unlockedTickets).map((tokenId) =>
          parseInt(tokenId, 16)
        ),
        maxSupply: Number(BigInt(maxSupply)),
        totalSupply: Number(BigInt(totalSupply)),
      },
    };
  } catch (error) {
    console.error("Lỗi:", error);
    throw error;
  }
};

export default getTicketsByOwner;
