import { walletConfig } from "@/configs/network";
import { mainnet, sepolia } from "@starknet-react/chains";

export enum StatusEnum {
  SUCCESS = "success",
  ERROR = "error",
}

export const MessageEnum = {
  SUCCESS: "Success.",
  ERROR: "Found an error, please try again later.",
  WRONG_NETWORK: `Wrong network, please switch to ${
    walletConfig.targetNetwork.id == mainnet.id ? "mainnet" : "sepolia"
  } network!`,
  ADDRESS_NOT_FOUND: "Address not found.",
  STATION_INIT_FAILED: "Station initialization failed.",
  ASSIGN_MINER_TO_STATION_FAILED: "Assign miner to station failed.",
  EXTINGUISH_MINER_FAILED: "Extinguish miner failed.",
  MERGE_MINER_FAILED: "Merge miner failed.",
  REPAIR_CORE_ENGINE_FAILED: "Repair core engine failed.",
  CANNOT_EXECUTE_DOWNGRADE_RIGHT_NOW:
    "Cannot execute downgrade right now. Please wait a moment.",
  ENGINE_TYPE_NOT_FOUND: "Engine type not found.",
  EXECUTE_FAILED: "Execute failed.",
  RECORD_LOGIN_FAILED: "Record login failed.",
  OPEN_TICKET_FAILED: "Open ticket failed.",
  MINT_TICKET_FAILED: "Mint ticket failed.",
} as const;

export const ErrorLevelEnum = {
  INFOR: 0,
  WARNING: 1,
  ERROR: 2,
};

export enum EventKeyEnum {
  MinerMinted = "0x270f83cc00ac131de21fbbf4fa173d136a8f7941b5399a881cd17c399164afc",
  Transfer = "0x99cd8bde557814842a3121e8ddfd433a539b8c9f14bf31ebf108d12e6196e9",
  EngineMinted = "0x16622eaa1ea392485702e569a492f2766d8198c203bcf49e65995f3377ade45",
  StationsInitialized = "0x1d1299fed5c4794b4b4e6e9d1dfcb0497e57ec9c982e6e095b59fda38596cab",
  EngineMined = "0x16622eaa1ea392485702e569a492f2766d8198c203bcf49e65995f3377ade45",
  MergeSuccessful = "0x10c55b7f0b5c91d0ed5421e006e3328fa4b78f32b137e225162d69274528028",
  MergeFailed = "0x7dffcf74ad1b984e275cae190e486c0163c9a2669f10f531688bafb9717812",
  LoginRecorded = "0x111a79b0aa2fee2970c538703c9e67be34e91e60c04816fde74367ba592d3c5",
  TicketOpened = "Oxad9ea978670140bf0fc350424d62092a17ea7bae089850421e4c77026b20d6",
}

export interface MessageBase {
  status: StatusEnum.SUCCESS | StatusEnum.ERROR;
  message: (typeof MessageEnum)[keyof typeof MessageEnum];
  data: any;
  level: (typeof ErrorLevelEnum)[keyof typeof ErrorLevelEnum];
}

export const SECOND_PER_BLOCK = 30;
export const REQUIRED_STREAK_TO_CLAIM_REWARD = 5;

export interface StationInfo {
  id: number;
  level: number;
  multiplier: number;
  mineLocked: number;
  lockTimestamp: number;
  unlockTimestamp: number;
  pendingDowngrade: number;
  minerCount: number;
  minersAssigned: any;
}

export enum TimeSecondsEnum {
  ONE_DAY = 86400,
}
