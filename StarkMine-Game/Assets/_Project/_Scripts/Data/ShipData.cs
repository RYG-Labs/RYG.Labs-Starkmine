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
        level = 0;
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
        return Helpers.Round(shipSO.baseHashPower * (1 + level * 0.1f));
    }

    public float GetHashPower(int levelShip)
    {
        return Helpers.Round(shipSO.baseHashPower * (1 + levelShip * 0.1f));
    }

    public void Upgrade()
    {
        level++;
    }

    public float GetIncreasePowerForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return GetHashPower(level + 1) - GetHashPower(level);
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

    public bool IsMinLevel()
    {
        return level == 0;
    }
}