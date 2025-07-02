import { contracts } from "@/configs/contracts";
import { CallData } from "starknet";
import { provider } from "../readContract";
import { ErrorLevelEnum, MessageEnum, StatusEnum } from "@/type/common";

const upgradeStation = async (account: any, stationId: number, targetLevel: number) => {
    try {
        const tx = await account.execute({
            contractAddress: contracts.StationSystem,
            entrypoint: "upgrade_station",
            calldata: CallData.compile({ station_id: stationId, target_level: targetLevel }),
        });

        const receipt = await provider.waitForTransaction(tx.transaction_hash);

        console.log(receipt);
        
        if (receipt.isSuccess()) {
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: {
                    stationId: stationId,
                    targetLevel: targetLevel,
                }
            }
        } else {
            return {
                status: StatusEnum.ERROR,
                message: MessageEnum.ERROR,
                level: ErrorLevelEnum.WARNING,
                data: {
                    stationId: stationId,
                    targetLevel: targetLevel,
                }
            }
        };
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                stationId: stationId,
                targetLevel: targetLevel,
            },
        }
    }
};          

export default upgradeStation;