using System;
using UnityEngine;

[Serializable]
public class CoreEngineData
{
    public int id;
    public CoreEngineSO coreEngineSO;

    public bool isActive;

    // public int durability;
    public int blockUsed;
    public int lastUsedBlock;
    public int currentEfficiencyBonus;

    public CoreEngineData(int id, CoreEngineSO coreEngineSO, bool isActive, int blockUsed, int currentEfficiencyBonus,
        int lastUsedBlock)
    {
        this.id = id;
        this.coreEngineSO = coreEngineSO;
        this.isActive = isActive;
        // this.durability = durability;
        this.blockUsed = blockUsed;
        this.lastUsedBlock = lastUsedBlock;
    }

    // public int GetDownDurability()
    // {
    //     return coreEngineSO.GetDownWithDurability(coreEngineSO.durabilityBlock);
    // }

    public float GetEarningRate()
    {
        if (GetCurrentEfficiencyBonus() <= 0)
        {
            return 0f;
        }

        return (float)GetCurrentEfficiencyBonus() / coreEngineSO.efficiencyBonus;
    }

    public int GetCurrentEfficiencyBonus()
    {
        if (IsLostDurability())
        {
            return 0;
        }

        float multiplier = 50 + (GetDurabilityPercentage() / 2);
        return Mathf.CeilToInt((coreEngineSO.efficiencyBonus * multiplier) / 100);
    }

    public int GetMineRequirmentDurability(int targetDurability)
    {
        return targetDurability * 50;
    }

    public int GetMineReceiveEstimate()
    {
        return (int)((0.4 + GetDurabilityPercentage() * 0.2) * coreEngineSO.cost);
    }

    public float GetDurabilityPercentage()
    {
        if (coreEngineSO.durabilityBlock - blockUsed < 0)
        {
            return 0;
        }

        return (float)(coreEngineSO.durabilityBlock - blockUsed) / coreEngineSO.durabilityBlock;
    }

    public int GetDurabilityPercentToRestore(int targetDurability)
    {
        return targetDurability - coreEngineSO.durabilityBlock;
    }

    public bool CanRepair(int targetDurability)
    {
        return targetDurability > 0 && targetDurability <= blockUsed;
    }

    public bool IsLostDurability()
    {
        return blockUsed >= coreEngineSO.durabilityBlock;
    }

    public bool IsLostDurability(int currentBlock)
    {
        return currentBlock >= coreEngineSO.durabilityBlock - blockUsed + lastUsedBlock;
    }

    public override string ToString()
    {
        return $"=={id}::{coreEngineSO}::{blockUsed}::{lastUsedBlock}::{GetDurabilityPercentage()}";
    }

    public void ResetBlockUsed()
    {
        blockUsed = 0;
    }
}