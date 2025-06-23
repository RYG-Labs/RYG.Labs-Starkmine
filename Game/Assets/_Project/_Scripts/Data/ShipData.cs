using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ShipData
{
    public event EventHandler<OnShipDataChangedEventArgs> OnShipDataChangedEventHandler;

    public class OnShipDataChangedEventArgs : EventArgs
    {
        public ShipData NewShipData;
    }

    [SerializeField] public int shipID;
    [SerializeField] public ShipSO shipSO;
    [SerializeField] public int level;
    [SerializeField] private CoreEngineSO coreEngine;
    [SerializeField] public int maintenanceLevel;
    [SerializeField] public int maintenanceDown;
    [SerializeField] public bool onDuty;

    public ShipData(ShipSO newShipSO)
    {
        shipSO = newShipSO;
        level = 1;
        maintenanceLevel = 100;
        maintenanceDown = 0;
        onDuty = false;
    }

    public CoreEngineSO CoreEngine
    {
        get => coreEngine;
        set
        {
            coreEngine = value;
            OnShipDataChangedEventHandler?.Invoke(this, new OnShipDataChangedEventArgs()
            {
                NewShipData = this
            });
        }
    }

    public float GetHashPower()
    {
        return Helpers.Round(shipSO.hashPower * (level * 1.1f));
    }

    public void Upgrade()
    {
        level++;
    }

    public int GetIncreasePowerForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return shipSO.powerShipPerLevel[level] - shipSO.powerShipPerLevel[level - 1];
    }

    public int GetCostForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return shipSO.costPerLevel[level];
    }

    public bool IsMaxLevel()
    {
        return level == shipSO.maxLevel;
    }
}