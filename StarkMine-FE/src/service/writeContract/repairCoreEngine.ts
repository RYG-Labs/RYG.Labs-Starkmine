import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { engineDurabilityConfig, provider } from "../readContract";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { getEngineData } from "../readContract/getCoreEnginesByOwner";

const repairCoreEngine = async (account: AccountInterface, engineId: number, durabilityPercentToRestore: number): Promise<MessageBase> => {
    // get coreEngine info
    const engineDetail = await getEngineData(engineId);
    const maxDurability = engineDurabilityConfig.get(engineDetail.engineType);
    if (!maxDurability) return { status: StatusEnum.ERROR, message: MessageEnum.ENGINE_TYPE_NOT_FOUND, level: ErrorLevelEnum.WARNING, data: {
        engineId: engineId,
        durabilityToRestore: durabilityPercentToRestore,
    } }

    // calculate blocks to restore
    const blocksByOnePercent = maxDurability / 100;
    const blocksToRestore = blocksByOnePercent * durabilityPercentToRestore;

    try {
        const tx = await account.execute({
            contractAddress: contracts.CoreEngine,
            entrypoint: "repair_engine",
            calldata: CallData.compile({ engine_id: uint256.bnToUint256(engineId), durability_to_restore: blocksToRestore }),
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
                    durabilityToRestore: blocksToRestore,
                }
            } 
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.REPAIR_CORE_ENGINE_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: {
                    engineId: engineId,
                    durabilityToRestore: blocksToRestore,
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
                durabilityToRestore: blocksToRestore,
            },
        }
    }
};  

export default repairCoreEngine;