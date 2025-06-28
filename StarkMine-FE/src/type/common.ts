import { walletConfig } from "@/configs/network";
import { mainnet, sepolia} from "@starknet-react/chains";

export const MessageEnum = {
    SUCCESS: "Success.",
    ERROR: "Found an error, please try again later.",
    WRONG_NETWORK: `Wrong network, please switch to ${walletConfig.targetNetwork.id == mainnet.id ? "mainnet" : "sepolia"} network!`,
    ADDRESS_NOT_FOUND: "Address not found.",
} as const;

export const ErrorLevelEnum = {
    INFOR: 0,
    WARNING: 1,
    ERROR: 2,
}

export enum EventKeyEnum {
    MinerMinted = "0x270f83cc00ac131de21fbbf4fa173d136a8f7941b5399a881cd17c399164afc",
    Transfer = "0x99cd8bde557814842a3121e8ddfd433a539b8c9f14bf31ebf108d12e6196e9",
}

export interface MessageBase {
    status: "success" | "error";
    message: typeof MessageEnum[keyof typeof MessageEnum];
    data: any;
    level: typeof ErrorLevelEnum[keyof typeof ErrorLevelEnum];
}

