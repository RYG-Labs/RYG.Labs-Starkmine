import { contracts } from "@/configs/contracts";
import { walletConfig } from "@/configs/network";
import { mainnet } from "@starknet-react/chains";
import { Contract, RpcProvider } from "starknet";

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

export const mineContract = new Contract(await getAbi(contracts.MineToken), contracts.MineToken, provider);
export const minerContract = new Contract(await getAbi(contracts.MinerNFT), contracts.MinerNFT, provider);
export const coreEngineContract = new Contract(await getAbi(contracts.CoreEngine), contracts.CoreEngine, provider);
export const stationContract = new Contract(await getAbi(contracts.StationSystem), contracts.StationSystem, provider);
export const mergeContract = new Contract(await getAbi(contracts.MergeSystem), contracts.MergeSystem, provider);
export const rewardDistributorContract = new Contract(await getAbi(contracts.RewardDistributor), contracts.RewardDistributor, provider);