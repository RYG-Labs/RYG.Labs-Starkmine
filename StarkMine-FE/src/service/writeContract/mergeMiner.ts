import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, EventKeyEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

const mergeMiner = async (account: AccountInterface, tokenId1: number, tokenId2: number, fromTier: string, toTier: string): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.MergeSystem,
            entrypoint: "attempt_merge",
            calldata: CallData.compile({ token_id_1: uint256.bnToUint256(tokenId1), token_id_2: uint256.bnToUint256(tokenId2), from_tier: fromTier, to_tier: toTier }),
        });
    
        const receipt = await provider.waitForTransaction(tx.transaction_hash);
    
        console.log(receipt);

        let data: any = {
            tokenId1: tokenId1,
            tokenId2: tokenId2,
            fromTier: fromTier,
            toTier: toTier,
            isMergeSuccessful: false,
            newTokenId: 0,
            baseSuccessRate: toTier === "Pro" ? 50 : 25,
            successRateAfterFailed: 0,
        }
        if (receipt.isSuccess()) {
        

            receipt.events.forEach((event) => {
                if (event.keys[0] === EventKeyEnum.MergeSuccessful) {
                    data.isMergeSuccessful = true;
                    data.newTokenId = parseInt(event.data[2], 16);
                    return;
                }

                if(event.keys[0] === EventKeyEnum.MergeFailed) {
                    data.isMergeSuccessful = false;
                    data.successRateAfterFailed = parseInt(event.data[2], 16) / 100;
                    return;
                }
            });
            console.log(data);
        
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: data
            }
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.MERGE_MINER_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: data
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