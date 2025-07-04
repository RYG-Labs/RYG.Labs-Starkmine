import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { getAbi, provider } from ".";
import { contracts } from "@/configs/contracts";
import { convertWeiToEther } from "@/utils/helper";
import { Contract } from "starknet";

const StationSystemContract = new Contract(await getAbi(contracts.StationSystem), contracts.StationSystem, provider);

const getTimeUntilUnlock = async (address: string, stationId: number): Promise<MessageBase> => {
    try {
        const timeUntilUnlock = await StationSystemContract.get_time_until_unlock(address, stationId);
        console.log("🚀 ~ getTimeUntilUnlock ~ timeUntilUnlock:", timeUntilUnlock)
    
        return {
            status: StatusEnum.SUCCESS,
            message: MessageEnum.SUCCESS,
            level: ErrorLevelEnum.INFOR,
            data: {
                timeUntilUnlock: Number(convertWeiToEther(timeUntilUnlock)),
            }
        }
    } catch (error: any) {
        return {
            status: StatusEnum.ERROR,
            message: error.message,
            level: ErrorLevelEnum.WARNING,
            data: {
                timeUntilUnlock: 0,
            },
        }
    }
}   

export default getTimeUntilUnlock;