using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CoreEngineSO", menuName = "Scriptable Objects/CoreEngineSO")]
public class CoreEngineSO : ScriptableObject
{
    public enum CoreEngineType
    {
        Basic,
        Elite,
        Pro,
        GIGA
    }

    [SerializeField] public Sprite sprite;
    [SerializeField] public string nameCoreEngine;
    [SerializeField] public CoreEngineType coreEngineType;
    [SerializeField] public int cost;
    [SerializeField] public List<CoreEngineDownWithDurabilitySO> listCoreEngineDownWithDurabilitySO;
    [SerializeField] public int repairCostBase;
    [SerializeField] public int durabilityBlock;
    [SerializeField] public int efficiencyBonus;
    public int GetDownWithDurability(int currentDurability)
    {
        foreach (CoreEngineDownWithDurabilitySO coreEngineDownWithDurabilitySo in listCoreEngineDownWithDurabilitySO)
        {
            if (currentDurability >= coreEngineDownWithDurabilitySo.minDurability &&
                currentDurability <= coreEngineDownWithDurabilitySo.maxDurability)
            {
                return coreEngineDownWithDurabilitySo.downAmount;
            }
        }

        return 0;
    }
    
}