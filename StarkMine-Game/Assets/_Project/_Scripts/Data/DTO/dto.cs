using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class UserDTO
{
    public string address;
    public double balance;
}

public class ResponseGetPendingRewardDTO
{
    public int pendingReward;
}

public class ResponseLoginStreakDTO
{
    public int currentStreak;
    public int remainingTimeToRecordLogin;
    public int streakToClaimReward;
}

public class ResponseClaimPendingRewardDTO
{
    public string address;
}

public class ShipDTO
{
    public int tokenId;
    public string tier;
    public double hashPower;
    public int level;
    public int efficiency;
    public string lastMaintenance;
    public int coreEngineId;
    public bool isIgnited;

    public override string ToString()
    {
        return
            $"ShipDTO {{ tokenId={tokenId}, tier={tier}, hashPower={hashPower}, level={level}, efficiency={efficiency}, lastMaintenance={lastMaintenance}, coreEngineId={coreEngineId}, isIgnited={isIgnited} }}";
    }
}

public class CoreEngineDTO
{
    public int tokenId;
    public int attachedMiner;
    public int blocksUsed;
    public int durability;
    public int efficiencyBonus;
    public int currentEfficiencyBonus;
    public string engineType;
    public bool isActive;
    public int lastUsedBlock;
    public int durabilityPercent;
}

public class EngineConfigDTO
{
    public int durability;
    public string engineType;
    public int mintCost;
    public int repairCostBase;
    public int efficiencyBonus;
}

public class ResponseIgniteMinerDTO
{
    public int minerId;
    public JObject coreEngine;
    public CoreEngineDTO CoreEngineDto => coreEngine.ToObject<CoreEngineDTO>();
}

public class ResponseExtinguishMinerDTO
{
    public int minerId;
    public JObject coreEngine;
    public CoreEngineDTO CoreEngineDto => coreEngine.ToObject<CoreEngineDTO>();
    public int durabilityPercent;
}

public class StationDTO
{
    public int id;
    public int level;
    public int lockTimestamp;
    public int mineLocked;
    public int minerCount;
    public int multiplier;
    public int pendingDowngrade;
    public int unlockTimestamp;
    public int estimateSecond;
    public JArray minersAssigned;
    public List<MinerAssigned> MinersAssigned => minersAssigned.ToObject<List<MinerAssigned>>();
}

public class MinerAssigned
{
    public int tokenId;
    public int slot;
}

public class ResponseMinerLevelsConfigDTO
{
    public int level;
    public int mineRequired;
}

public class ResponseStationLevelsConfigDTO
{
    public int level;
    public int mineRequired;
    public int multiplier;
    public int unlockPeriod;
}

public class AssignMinerToStationDTO
{
    public int stationId;
    public int minerId;
    public int minerSlot;
}

public class RemoveMinerFromStationDTO
{
    public int stationId;
    public int minerSlot;
}

public class ResponseUpgradeStationDTO
{
    public int stationId;
    public int targetLevel;
}

public class ResponseDowngradeStationDTO
{
    public int stationId;
    public int targetLevel;
    public int timeUntilUnlock;
    public int estimateSeconds;
}

public class ResponseMintCoreEngineDTO
{
    public string engineType;
    public int coreEngineId;
}

public class ResponseDefuseEngineDTO
{
    public int engineId;
    public int mineRecive;
}

public class ResponseUpgradeMinerDTO
{
    public int minerId;
}

public class ResponseMergeMinerDTO
{
    public int tokenId1;
    public int tokenId2;
    public string fromTier;
    public string toTier;
    public bool isMergeSuccessful;
    public int newTokenId;
    public int baseSuccessRate;
    public int failureBonus;
}

public class ResponseCurrentMergeStatusByUserDTO
{
    public string fromTier;
    public string toTier;
    public int baseSuccessRate;
    public int successRateBonus;
    public int costStrk;
    public int coseMine;
    public JObject tierConfig;
    public TierConfig TierConfig => tierConfig.ToObject<TierConfig>();
}

public class TierConfig
{
    public int baseHashPower;
    public int mintCostMine;
    public int mintCostStrk;
    public int mintedCount;
    public int supplyLimit;
    public int tierBonus;
}

public class ResponseRepairCoreEngineDTO
{
    public int engineId;
    public int durabilityToRestore;
}

public class ResponseTotalHashPowerDTO
{
    public float totalHashPower;
}

public class ResponseUserHashPowerDTO
{
    public float userHashPower;
}

public class ResponseRemainingBlockForHavingDTO
{
    public int remainingBlock;
    public int estimateSeconds;
}

public class ResponseCancelDowngradeDTO
{
    public int stationId;
}

public class ResponseCurrentBlockDTO
{
    public int currentBlock;
}

public class ResponseRecordLoginDTO
{
}

public class ResponseMintTicketDTO
{
    public int ticketId;
}

public class ResponseOpenTicketDTO
{
    public int ticketId;
    public string tier;
    public int tokenId;
}

public class ResponseExecuteDowngradeDTO
{
    public int stationId;
}