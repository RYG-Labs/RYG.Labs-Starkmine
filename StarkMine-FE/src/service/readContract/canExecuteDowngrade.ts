
import { stationContract } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";

const canExecuteDowngrade = async (address: string,stationId: number): Promise<MessageBase> => {
    try {
        const canExecute = await stationContract.can_execute_downgrade(address, stationId);
        return {
            status: StatusEnum.SUCCESS,
            message: MessageEnum.SUCCESS,
            level: ErrorLevelEnum.INFOR,
            data: {
                canExecute: canExecute,
            }
        }
    } catch (error) {
        return {
            status: StatusEnum.ERROR,
            message: MessageEnum.ERROR,
            level: ErrorLevelEnum.WARNING,
            data: {
                canExecute: false,
            },
        }
    }
};  

export default canExecuteDowngrade;