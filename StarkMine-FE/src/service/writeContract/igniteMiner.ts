import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";


export const igniteMiner = async (account: AccountInterface, minerId: number, coreEngineId: number): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.MinerNFT,
            entrypoint: "ignite_miner",
            calldata: CallData.compile({ token_id: uint256.bnToUint256(minerId), core_engine_id: uint256.bnToUint256(coreEngineId) }),
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
                    coreEngineId: coreEngineId,
                }

            }
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.ERROR,
                level: ErrorLevelEnum.WARNING,
                data: {
                    minerId: minerId,
                    coreEngineId: coreEngineId,
                }
            }
        }
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                minerId: minerId,
                coreEngineId: coreEngineId,
            },
        }
    }
};