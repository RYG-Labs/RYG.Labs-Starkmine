import { walletConfig } from "@/configs/network";
import { mainnet } from "@starknet-react/chains";
import { RpcProvider } from "starknet";

const nodeUrl =
  walletConfig.targetNetwork.id == mainnet.id
    ? process.env.NEXT_PUBLIC_MAINNET_STARKNET_NODE_URL
    : process.env.NEXT_PUBLIC_TESTNET_STARKNET_NODE_URL;

export const provider = new RpcProvider({ nodeUrl: `${nodeUrl}` });

export const getAbi = async (contractAddress: string) => {
  const { abi } = await provider.getClassAt(contractAddress);

  console.log("🚀 ~ getAbi ~ abi:", abi);
  return abi;
};