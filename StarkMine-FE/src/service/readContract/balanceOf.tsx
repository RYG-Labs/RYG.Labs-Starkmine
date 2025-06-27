import { walletConfig } from "@/configs/network";
import { Contract, RpcProvider } from "starknet";
import { mainnet } from "@starknet-react/chains";
import { contracts } from "@/configs/contracts";
import { ABI_MINE_TOKEN } from "@/type/ABI_MINE_TOKEN";
import { convertWeiToEther } from "../../utils/helper";

const nodeUrl =
  walletConfig.targetNetwork.id == mainnet.id
    ? process.env.NEXT_PUBLIC_MAINNET_STARKNET_NODE_URL
    : process.env.NEXT_PUBLIC_TESTNET_STARKNET_NODE_URL;

export const provider = new RpcProvider({ nodeUrl: `${nodeUrl}` });

export const balanceOf = async (address: string) => {
  const MineContract = new Contract(
    ABI_MINE_TOKEN,
    contracts.MineToken,
    provider
  );
  const balance = await MineContract.balance_of(address);
  return convertWeiToEther(balance);
};
