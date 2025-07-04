import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import {  stationContract } from ".";
import { convertWeiToEther } from "@/utils/helper";

const getTimeUntilUnlock = async (address: string, stationId: number): Promise<MessageBase> => {
    try {
        const timeUntilUnlock = await stationContract.get_time_until_unlock(address, stationId);
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