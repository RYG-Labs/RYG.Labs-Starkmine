using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ShipData
{
    public event EventHandler<OnShipDataChangedEventArgs> OnShipDataChangedEventHandler;

    public class OnShipDataChangedEventArgs : EventArgs
    {
        public ShipData NewShipData;
    }

    [SerializeField] public int id;
    [SerializeField] public ShipSO shipSO;

    [SerializeField] public int level;

    // [SerializeField] private CoreEngineSO coreEngine;
    private CoreEngineData coreEngineDataData;
    [SerializeField] public int maintenanceLevel;
    [SerializeField] public int maintenanceDown;
    [SerializeField] public bool onDuty;
    [SerializeField] public double hashPower;

    public ShipData(ShipSO newShipSO)
    {
        shipSO = newShipSO;
        level = 1;
        maintenanceLevel = 100;
        maintenanceDown = 0;
        onDuty = false;
    }

    public ShipData(ShipSO newShipSO, int level, double hashPower, int maintenanceLevel, bool onDuty)
    {
        shipSO = newShipSO;
        this.level = level;
        this.maintenanceLevel = maintenanceLevel;
        this.hashPower = hashPower;
        // this.maintenanceDown = maintenanceDown;
        this.onDuty = onDuty;
    }

    public ShipData(int id, ShipSO newShipSO, int level, double hashPower, bool onDuty)
    {
        this.id = id;
        shipSO = newShipSO;
        this.level = level;
        this.hashPower = hashPower;
        this.onDuty = onDuty;
    }

    public CoreEngineData CoreEngineData
    {
        get => coreEngineDataData;
        set
        {
            coreEngineDataData = value;
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