import { contracts } from "@/configs/contracts";
import { walletConfig } from "@/configs/network";
import ABICoreEngine from "@/type/ABICoreEngine";
import ABIMerge from "@/type/ABIMerge";
import ABIMiner from "@/type/ABIMiner";
import ABIMineToken from "@/type/ABIMineToken";
import ABIRewardDistributor from "@/type/ABIRewardDistributor";
import ABIStationSystem from "@/type/ABIStationSystem";
import { mainnet } from "@starknet-react/chains";
import { Contract, RpcProvider } from "starknet";

const nodeUrl =
  walletConfig.targetNetwork.id == mainnet.id
    ? process.env.NEXT_PUBLIC_MAINNET_STARKNET_NODE_URL
    : process.env.NEXT_PUBLIC_TESTNET_STARKNET_NODE_URL;

export const provider = new RpcProvider({ nodeUrl: `${nodeUrl}` });

export const getAbi = async (contractAddress: string) => {
  const { abi } = await provider.getClassAt(contractAddress);

  console.log("🚀 ~ getAbi ~ abi:",contractAddress,  abi);
  return abi;
};

export const engineDurabilityConfig = new Map<string, number>(
  [
    ["Basic", 86400],
    ["Elite", 129600],
    ["Pro", 172800],
    ["GIGA", 259200],
  ]
)

export const mineContract = new Contract(ABIMineToken, contracts.MineToken, provider);
export const minerContract = new Contract(ABIMiner, contracts.MinerNFT, provider);
export const coreEngineContract = new Contract(ABICoreEngine, contracts.CoreEngine, provider);
export const stationContract = new Contract(ABIStationSystem, contracts.StationSystem, provider);
export const mergeContract = new Contract(ABIMerge, contracts.MergeSystem, provider);
export const rewardDistributorContract = new Contract(ABIRewardDistributor, contracts.RewardDistributor, provider);