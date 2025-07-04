const getLevelConfig = async (level: number) => {
    const mineRequired = [2000, 5000, 12000, 35000, 100000];
    return {
        level: level,
        mineRequired: mineRequired[level - 1],
    }
}

const getMinerLevelsConfig = async () => {
    const levelsConfig = await Promise.all([1, 2, 3, 4, 5].map(level => getLevelConfig(level)));

    return levelsConfig;
}

export default getMinerLevelsConfig