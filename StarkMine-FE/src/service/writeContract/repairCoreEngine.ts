import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

const repairCoreEngine = async (account: AccountInterface, engineId: number, durabilityToRestore: number): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.CoreEngine,
            entrypoint: "repair_engine",
            calldata: CallData.compile({ engine_id: uint256.bnToUint256(engineId), durability_to_restore: durabilityToRestore }),
        });
    
        const receipt = await provider.waitForTransaction(tx.transaction_hash);
    
        console.log(receipt);
        
        if (receipt.isSuccess()) {
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: {
                    engineId: engineId,
                    durabilityToRestore: durabilityToRestore,
                }
            } 
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.REPAIR_CORE_ENGINE_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: {
                    engineId: engineId,
                    durabilityToRestore: durabilityToRestore,
                }
            }
        };
    } catch (error: any) {
        console.log(error.message);
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                engineId: engineId,
                durabilityToRestore: durabilityToRestore,
            },
        }
    }
};  

export default repairCoreEngine;