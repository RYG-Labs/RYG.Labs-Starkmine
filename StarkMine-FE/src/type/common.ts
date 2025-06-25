import { walletConfig } from "@/configs/network";
import { mainnet, sepolia} from "@starknet-react/chains";

export const MessageEnum = {
    SUCCESS: "Success.",
    ERROR: "Found an error, please try again later.",
    WRONG_NETWORK: `Wrong network, please switch to ${walletConfig.targetNetwork.id == mainnet.id ? "mainnet" : "sepolia"} network!`,
} as const;

export const ErrorLevelEnum = {
    INFOR: 0,
    WARNING: 1,
    ERROR: 2,
}

export interface MessageBase {
    status: "success" | "error";
    message: typeof MessageEnum[keyof typeof MessageEnum];
    data: any;
    level: typeof ErrorLevelEnum[keyof typeof ErrorLevelEnum];
}