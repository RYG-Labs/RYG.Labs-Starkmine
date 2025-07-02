import { ABI_STATION_SYSTEM } from "@/type/ABI_STATION_SYSTEM";
import { Contract } from "starknet"
import { provider } from ".";
import { contracts } from "@/configs/contracts";
import { convertWeiToEther } from "@/utils/helper";

const stationContract = new Contract(ABI_STATION_SYSTEM, contracts.StationSystem, provider);

const getLevelConfig = async (level: number) => {
    const levelConfig = await stationContract.get_level_config(level);
    return {
        level: level,
        mineRequired: Number(convertWeiToEther(levelConfig.mine_required)),
        multiplier: Number(levelConfig.multiplier),
        unlockPeriod: Number(levelConfig.unlock_period),
    }
}

const getStationLevelsConfig = async () => {
    const levelsConfig = await Promise.all([1, 2, 3, 4].map(level => getLevelConfig(level)));
    console.log("🚀 ~ getStationLevelsConfig ~ levelsConfig:", levelsConfig)

    return levelsConfig;
}

export default getStationLevelsConfig