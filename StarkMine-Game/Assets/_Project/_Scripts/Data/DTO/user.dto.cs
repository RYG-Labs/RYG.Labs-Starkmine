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
    public string engineType;
    public bool isActive;
    public int lastUsedBlock;
}

public class ResponseIgniteMinerDTO
{
    public int minerId;
    public int coreEngineId;
}

public class ResponseExtinguishMinerDTO
{
    public int minerId;
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
    public JArray minerIds;
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
    public int index;
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

public class ResponseRequestDowngradeStation
{
    public int stationId;
    public int targetLevel;
}

public class ResponseMintCoreEngineDTO
{
    public string engineType;
    public int coreEngineId;
}

public class ResponseDefuseEngineDTO
{
    public int engineId;
}

public class ResponseUpgradeMinerDTO
{
    public int minerId;
}