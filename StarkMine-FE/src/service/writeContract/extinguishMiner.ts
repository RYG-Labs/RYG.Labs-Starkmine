import { contracts } from "@/configs/contracts";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { AccountInterface, CallData } from "starknet";
import { provider } from "../readContract";

const extinguishMiner = async (account: AccountInterface, minerId: number): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.MinerNFT,
            entrypoint: "extinguish_miner",
            calldata: CallData.compile({ token_id: minerId }),
        });
    
        const receipt = await provider.waitForTransaction(tx.transaction_hash);
    
        console.log(receipt);
        
        if (receipt.isSuccess()) {
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: {
                    minerId: minerId,
                }
            } 
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.EXTINGUISH_MINER_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: {
                    minerId: minerId,
                }
            }
        };
    } catch (error) {
        return {
            status: StatusEnum.ERROR,
            message: MessageEnum.ERROR,
            level: ErrorLevelEnum.WARNING,
            data: {
                minerId: minerId,
            },
        }
    }
};

export default extinguishMiner;