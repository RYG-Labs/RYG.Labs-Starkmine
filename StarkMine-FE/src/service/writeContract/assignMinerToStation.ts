import { contracts } from "@/configs/contracts";
import { AccountInterface, CallData, uint256 } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

const assignMinerToStation = async (account: AccountInterface, stationId: number, minerId: number, index: number): Promise<MessageBase> => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.StationSystem,
            entrypoint: "assign_miner_to_station",
            calldata: CallData.compile({ station_id: stationId, token_id: uint256.bnToUint256(minerId) }),
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
                    stationId: stationId,
                    index: index,
                }
            } 
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.ASSIGN_MINER_TO_STATION_FAILED,
                level: ErrorLevelEnum.WARNING,
                data: {
                    minerId: minerId,
                    stationId: stationId,
                    index: index,
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
                stationId: stationId,
                index: index,
            },
        }
    }
};  

export default assignMinerToStation;