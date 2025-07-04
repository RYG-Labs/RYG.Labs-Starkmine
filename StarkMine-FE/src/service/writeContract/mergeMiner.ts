import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

const mergeMiner = async (account: AccountInterface, tokenId1: number, tokenId2: number, fromTier: string, toTier: string): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.MergeSystem,
            entrypoint: "attempt_merge",
            calldata: CallData.compile({ token_id_1: uint256.bnToUint256(tokenId1), token_id_2: uint256.bnToUint256(tokenId2), from_tier: fromTier, to_tier: toTier }),
        });
    
        const receipt = await provider.waitForTransaction(tx.transaction_hash);
    
        console.log(receipt);
        
        if (receipt.isSuccess()) {
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: {
                    tokenId1: tokenId1,
                    tokenId2: tokenId2,
                    fromTier: fromTier,
                    toTier: toTier,
                }
            }
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.MERGE_MINER_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: {
                    tokenId1: tokenId1,
                    tokenId2: tokenId2,
                    fromTier: fromTier, 
                    toTier: toTier,
                }
            }
        };
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                tokenId1: tokenId1,
                tokenId2: tokenId2,
                fromTier: fromTier, 
                toTier: toTier,
            },
        }
    }
};  

export default mergeMiner;