import { coreEngineContract, minerContract } from ".";
import { getAllStations } from "./getStationByOwner";


const getNewHashPower = async (minerId: number, address: string) => {
    const miner = await minerContract.get_miner_info(minerId);
    const coreEngine = Number(BigInt(miner.core_engine_id)) ? await coreEngineContract.get_engine_info(miner.core_engine_id) : undefined;

    // only hash power by level
    const newHashPowerByLevel = Number(BigInt(miner.hash_power) / BigInt(1e12)) * (1 + (Number(miner.level) + 1) * 0.1) * (Number(miner.efficiency) / 100);

    // core engine multiplier
    const currentCoreEngineBonus = coreEngine ? await coreEngineContract.get_current_efficiency_bonus(miner.core_engine_id) : 0;
    const coreEngineMultiplier = (10000 +  Number(BigInt(currentCoreEngineBonus))) / 10000;

    // station multiplier
    let stationMultiplier = 1;
    const stationIncludeMiner = (await getAllStations(address, 10)).find((station) => {
        return station.minerIds.includes(minerId);
    });
    if(stationIncludeMiner) {
        stationMultiplier = stationIncludeMiner.multiplier / 10000;
    }

    console.log({
        baseHashPower: newHashPowerByLevel,
        coreEngineMultiplier: coreEngineMultiplier,
        stationMultiplier: stationMultiplier,
        totalHashPower: Math.round(newHashPowerByLevel * coreEngineMultiplier * stationMultiplier),
    });

    return {
        baseHashPower: newHashPowerByLevel,
        coreEngineMultiplier: coreEngineMultiplier,
        stationMultiplier: stationMultiplier,
        totalHashPower: Math.round(newHashPowerByLevel * coreEngineMultiplier * stationMultiplier),
    }
}

export default getNewHashPower;