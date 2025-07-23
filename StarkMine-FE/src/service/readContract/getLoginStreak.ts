import { contracts } from "@/configs/contracts";
import { provider } from ".";
import {
  ErrorLevelEnum,
  EventKeyEnum,
  MessageEnum,
  REQUIRED_STREAK_TO_CLAIM_REWARD,
  StatusEnum,
  TimeSecondsEnum,
} from "@/type/common";
import { formattedContractAddress } from "@/utils/helper";

const getLoginStreak = async (address: string) => {
  let continuationToken = undefined;
  let allEvents: any[] = [];

  let loginHistory = new Array<number>();

  try {
    do {
      const events = await provider.getEvents({
        chunk_size: 1000,
        continuation_token: continuationToken,
        from_block: { block_number: 0 },
        to_block: "latest",
        address: contracts.RewardDistributor,
        keys: [[EventKeyEnum.LoginRecorded]],
      });
      allEvents = allEvents.concat(events.events);
      console.log("🚀 ~ getLoginStreak ~ allEvents:", allEvents);
      continuationToken = events.continuation_token;

      events.events.forEach((event) => {
        const eventKey = event.keys[0];
        let eventName = "Unknown";

        if (eventKey === EventKeyEnum.LoginRecorded) {
          eventName = "LoginRecorded";
        }

        if (eventName === "LoginRecorded") {
          if (
            formattedContractAddress(event.keys[1]) ===
            formattedContractAddress(address)
          ) {
            loginHistory.push(parseInt(event.data[1], 16));
          }
        }
      });
    } while (continuationToken);

    const streak = await calculateLoginStreak(loginHistory);

    console.log({
      status: StatusEnum.SUCCESS,
      message: MessageEnum.SUCCESS,
      level: ErrorLevelEnum.INFOR,
      data: {
        currentStreak: streak,
        streakToClaimReward: REQUIRED_STREAK_TO_CLAIM_REWARD,
        remainingTimeToRecordLogin:
          loginHistory.length > 0
            ? Math.ceil(
                loginHistory[loginHistory.length - 1] +
                  TimeSecondsEnum.ONE_DAY -
                  new Date().getTime() / 1000
              )
            : 0,
      },
    });

    return {
      status: StatusEnum.SUCCESS,
      message: MessageEnum.SUCCESS,
      level: ErrorLevelEnum.INFOR,
      data: {
        currentStreak: streak,
        streakToClaimReward: REQUIRED_STREAK_TO_CLAIM_REWARD,
        remainingTimeToRecordLogin:
          loginHistory.length > 0
            ? Math.ceil(
                loginHistory[loginHistory.length - 1] +
                  TimeSecondsEnum.ONE_DAY -
                  new Date().getTime() / 1000
              )
            : 0,
      },
    };
  } catch (error) {
    console.error("Error:", error);
    throw error;
  }
};

export default getLoginStreak;

const calculateLoginStreak = async (
  loginHistory: Array<number>
): Promise<number> => {
  // if login history is empty or the last login is less than 2 days ago, return 0
  const date = new Date();
  if (
    loginHistory.length == 0 ||
    date.getTime() / 1000 - loginHistory[loginHistory.length - 1] >
      2 * TimeSecondsEnum.ONE_DAY
  ) {
    return 0;
  }

  // calculate the streak
  let streak = 1;
  let latestLogin = loginHistory[loginHistory.length - 1];
  let i = loginHistory.length - 2;
  while (i >= 0) {
    if (latestLogin - loginHistory[i] > 2 * TimeSecondsEnum.ONE_DAY) {
      break;
    }
    streak++;
    i--;
    latestLogin = loginHistory[i];
  }
  return streak;
};
