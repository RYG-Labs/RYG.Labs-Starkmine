import { contracts } from "@/configs/contracts";
import { provider } from ".";
import { ErrorLevelEnum, MessageBase, MessageEnum, StatusEnum } from "@/type/common";
import { initStation } from "../writeContract/initStation";
import { AccountInterface, Contract } from "starknet";
import { ABI_STATION_SYSTEM } from "@/type/ABI_STATION_SYSTEM";
import { convertWeiToEther } from "@/utils/helper";

const StationSystemContract = new Contract(
    ABI_STATION_SYSTEM,
    contracts.StationSystem,
    provider
);

const getMinerIdsAssignedToStation = async (userAddress: string, stationId: number) => {
    const minerIds = await StationSystemContract.get_station_miners(userAddress, stationId);
    const minerIdsFormatted = minerIds.map((minerId: BigInt) => parseInt(minerId.toString()));
    return minerIdsFormatted
}

const formatStationData = (stationInfo: any): any => {
    return {
        id: Number(stationInfo.id),
        level: Number(stationInfo.level),
        multiplier: Number(stationInfo.multiplier),
        mineLocked: Number(convertWeiToEther(stationInfo.mine_locked)),
        lockTimestamp: Number(stationInfo.lock_timestamp),
        unlockTimestamp: Number(stationInfo.unlock_timestamp),
        pendingDowngrade: Number(stationInfo.pending_downgrade),
        minerCount: Number(stationInfo.miner_count),
        minerIds: stationInfo.minerIds,
    }
}

const getStationData = async (userAddress: string, stationId: number) => {

    const stationInfo = await StationSystemContract.get_station_info(userAddress, stationId);
    const minerIds = await getMinerIdsAssignedToStation(userAddress, stationId);
    return formatStationData({
        id: stationId,
        minerIds: minerIds,
        ...stationInfo
    });
};

const getAllStations = async (userAddress: string, stationCount: number) => {
    const stationsData = []

    for (let i = 1; i <= stationCount; i++) {
        const stationInfo = await getStationData(userAddress, i);
        stationsData.push(stationInfo);
    }

    return stationsData;
};

export const getStationsByOwner = async (account: AccountInterface, userAddress: string): Promise<MessageBase> => {
    try {
        const StationSystemContract = new Contract(
        ABI_STATION_SYSTEM,
        contracts.StationSystem,
        provider
      );

      const stationCount = parseInt(await StationSystemContract.get_user_station_count(userAddress));
      getMinerIdsAssignedToStation(userAddress, 1);
      if (!stationCount) {
        const initResult = await initStation(account);
        if (!initResult) {
          return {
            status: StatusEnum.ERROR,
            message: MessageEnum.STATION_INIT_FAILED,
            level: ErrorLevelEnum.WARNING,
            data: [],
          } as MessageBase;
        } else {
            const allStations = await getAllStations(userAddress, stationCount);
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
      return {
        status: StatusEnum.ERROR,
        message: MessageEnum.ERROR,
        level: ErrorLevelEnum.WARNING,
        data: [],
      }
    }
}