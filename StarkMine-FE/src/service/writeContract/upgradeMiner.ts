import { contracts } from "@/configs/contracts";
import { CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";

const upgradeMiner = async (account: any, minerId: number) => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.MinerNFT,
            entrypoint: "upgrade_miner",
            calldata: CallData.compile({ token_id: uint256.bnToUint256(minerId) }),
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
                message: MessageEnum.ERROR,
                level: ErrorLevelEnum.WARNING,
                data: {
                    minerId: minerId,
                }
            }
        };
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                minerId: minerId,
            },
        }
    }
};  

export default upgradeMiner;