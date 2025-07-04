
import {
  convertWeiToEther,
} from "../../utils/helper";
import { mineContract } from ".";

export const balanceOf = async (address: string) => {
  const balance = await mineContract.balance_of(address);
  console.log("🚀 ~ balanceOf ~ balance:", convertWeiToEther(balance))
  return convertWeiToEther(balance);
};