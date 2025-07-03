
import {
  Contract,
} from "starknet";
import { contracts } from "@/configs/contracts";
import { ABI_MINE_TOKEN } from "@/type/ABI_MINE_TOKEN";
import {
  convertWeiToEther,
} from "../../utils/helper";
import { provider } from ".";

export const balanceOf = async (address: string) => {
  const MineContract = new Contract(
    ABI_MINE_TOKEN,
    contracts.MineToken,
    provider
  );
  const balance = await MineContract.balance_of(address);
  console.log("🚀 ~ balanceOf ~ balance:", convertWeiToEther(balance))
  return convertWeiToEther(balance);
};