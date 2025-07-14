import { stationContract } from ".";
import { convertWeiToEther } from "@/utils/helper";

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
    const levelsConfig = await Promise.all([1, 2, 3, 4, 5].map(level => getLevelConfig(level)));

    return levelsConfig;
}

export default getStationLevelsConfig