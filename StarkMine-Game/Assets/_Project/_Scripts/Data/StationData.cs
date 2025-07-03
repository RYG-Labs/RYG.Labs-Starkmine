using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StationData
{
    public event EventHandler<OnStationDataChangedEventArgs> OnStationDataChangedEventHandler;

    public class OnStationDataChangedEventArgs : EventArgs
    {
        public StationData NewStationData;
    }

    [SerializeField] public int id;
    [SerializeField] public int level;
    [SerializeField] public SpaceStationSO spaceStationSo;
    [SerializeField] public PlanetSO planetSo;
    private ShipData[] _listShipData = new ShipData[6];

    public ShipData[] ListShipData => _listShipData;

    public void AddShipData(ShipData shipData)
    {
        for (int i = 0; i < _listShipData.Length; i++)
        {
            if (_listShipData[i] != null) continue;
            _listShipData[i] = shipData;
            return;
        }
    }

    public void RemoveShipData(ShipData shipData)
    {
        for (int i = 0; i < _listShipData.Length; i++)
        {
            if (_listShipData[i] == shipData) continue;
            _listShipData[i] = null;
            return;
        }
    }

    public void ResetShipData()
    {
        for (int i = 0; i < _listShipData.Length; i++)
        {
            _listShipData[i] = null;
        }
    }

    public void Upgrade()
    {
        OnStationDataChangedEventHandler?.Invoke(this, new OnStationDataChangedEventArgs()
        {
            NewStationData = this
        });
        level++;
    }

    public void Downgrade()
    {
        OnStationDataChangedEventHandler?.Invoke(this, new OnStationDataChangedEventArgs()
        {
            NewStationData = this
        });
        level--;
    }

    public bool CanDowngrade()
    {
        return level > 0;
    }

    public float GetIncreaseMultiplierForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return spaceStationSo.listMultiplierPerLevel[level];
    }

    public float GetDecreaseMultiplierForPrevLevel()
    {
        if (IsMinLevel()) return 0;
        return spaceStationSo.listMultiplierPerLevel[level - 1];
    }

    public int GetCostForNextLevel()
    {
        if (IsMaxLevel()) return 0;
        return spaceStationSo.listCostPerLevel[level];
    }

    public int GetCostForPrevLevel()
    {
        if (IsMinLevel()) return 0;
        return spaceStationSo.listCostPerLevel[level - 1];
    }

    public bool IsMaxLevel()
    {
        return level >= spaceStationSo.maxLevel;
    }

    public bool IsMinLevel()
    {
        return level <= 0;
    }
}