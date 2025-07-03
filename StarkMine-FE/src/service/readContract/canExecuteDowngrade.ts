import { contracts } from "@/configs/contracts";
import { ABI_STATION_SYSTEM } from "@/type/ABI_STATION_SYSTEM";
import { provider } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { Contract } from "starknet";

const stationSystemContract = new Contract(
    ABI_STATION_SYSTEM,
    contracts.StationSystem,
    provider
);

const canExecuteDowngrade = async (address: string,stationId: number): Promise<MessageBase> => {
    try {
        const canExecute = await stationSystemContract.can_execute_downgrade(address, stationId);
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