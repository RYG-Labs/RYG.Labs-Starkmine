
import { stationContract } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StationInfo, StatusEnum } from "@/type/common";
import { initStation } from "../writeContract/initStation";
import { AccountInterface } from "starknet";
import { convertWeiToEther } from "@/utils/helper";

const getMinerIdsAssignedToStation = async (userAddress: string, stationId: number) => {
    const minerAssigned = await stationContract.get_station_miners(userAddress, stationId);
    const minerAssignedFormatted = minerAssigned.map((miner : { token_id: BigInt, slot: BigInt }) => { 
      return { tokenId: Number(miner.token_id), slot: Number(miner.slot) }
    })
    return minerAssignedFormatted
}

const formatStationData = (stationInfo: any): StationInfo => {
    return {
        id: Number(stationInfo.id),
        level: Number(stationInfo.level),
        multiplier: Number(stationInfo.multiplier),
        mineLocked: Number(convertWeiToEther(stationInfo.mine_locked)),
        lockTimestamp: Number(stationInfo.lock_timestamp),
        unlockTimestamp: Number(stationInfo.unlock_timestamp),
        pendingDowngrade: Number(stationInfo.pending_downgrade),
        minerCount: Number(stationInfo.miner_count),
        minerAssigned: stationInfo.minerAssigned,
    }
}

const getStationData = async (userAddress: string, stationId: number): Promise<StationInfo> => {
    const stationInfo = await stationContract.get_station_info(userAddress, stationId);
    const minerAssigned = await getMinerIdsAssignedToStation(userAddress, stationId);
    return formatStationData({
        id: stationId,
        minerAssigned: minerAssigned,
        ...stationInfo
    });
};

export const getAllStations = async (userAddress: string, stationCount: number): Promise<any[]> => {
    const stationsData = []

    for (let i = 1; i <= stationCount; i++) {
        const stationInfo = await getStationData(userAddress, i);
        stationsData.push(stationInfo);
    }

    return stationsData;
};

export const getStationsByOwner = async (account: AccountInterface, userAddress: string): Promise<MessageBase> => {
    try {
      const stationCount = parseInt(await stationContract.get_user_station_count(userAddress));
      if (!stationCount) {
        const initResult = await initStation(account);
        if (!initResult) {
          return {
            status: StatusEnum.ERROR,
            message: MessageEnum.STATION_INIT_FAILED,
            level: ErrorLevelEnum.ERROR,
            data: [],
          } as MessageBase;
        } else {
            let stationCountAfterInit = parseInt(await stationContract.get_user_station_count(userAddress));
            const attemptCount = 20;
            let retriedAttemptCount = 0;
            while (stationCountAfterInit <= 0 && retriedAttemptCount < attemptCount) {
               // call station count again
               stationCountAfterInit = parseInt(await stationContract.get_user_station_count(userAddress));
               retriedAttemptCount++;
            }

            if (retriedAttemptCount >= attemptCount || stationCountAfterInit <= 0) {
                return {
                    status: StatusEnum.ERROR,
                    message: MessageEnum.STATION_INIT_FAILED,
                    level: ErrorLevelEnum.ERROR,
                    data: [],
                }
            }

            const allStations = await getAllStations(userAddress, stationCountAfterInit);
            console.log("🚀 ~ getStationsByOwner ~ allStations:", allStations)
            
            return {
                status: StatusEnum.SUCCESS,
                message: MessageEnum.SUCCESS,
                level: ErrorLevelEnum.INFOR,
                data: allStations,
            } as MessageBase;
        }
      } else {
        const allStations = await getAllStations(userAddress, stationCount);
        console.log("🚀 ~ getStationsByOwner ~ allStations:", allStations)
        return {
            status: StatusEnum.SUCCESS,
            message: MessageEnum.SUCCESS,
            level: ErrorLevelEnum.INFOR,
            data: allStations,
        } as MessageBase;
      }
      
    } catch (error) {
      console.log(error);
      
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.ERROR,
        data: [],
      }
    }
}