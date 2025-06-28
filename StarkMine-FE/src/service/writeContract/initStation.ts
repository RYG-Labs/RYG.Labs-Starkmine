import { contracts } from "@/configs/contracts";
import { MessageBase } from "@/type/common";
import { AccountInterface } from "starknet";
import { provider } from "../readContract";

export const initStation = async (account: AccountInterface): Promise<boolean> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.StationSystem,
            entrypoint: "initialize_user_stations",
            calldata: [],
        });

       const receipt = await provider.waitForTransaction(tx.transaction_hash);

        console.log(receipt);
        
        if (receipt.isSuccess()) {
            return true;
        } else {
            return false;
        }
    } catch (error) {
        return false;
    }
};  