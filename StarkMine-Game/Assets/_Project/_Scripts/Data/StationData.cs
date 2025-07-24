using System;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] public long pendingMineTime;

    // [SerializeField] public int pendingMineValue;
    public int pendingDownGrade;
    private ShipData[] _listShipData = new ShipData[6];

    public ShipData[] ListShipData => _listShipData;

    public int CountShipData()
    {
        int count = 0;
        for (int i = 0; i < _listShipData.Length; i++)
        {
            if (_listShipData[i] != null)
            {
                count++;
            }
        }

        return count;
    }

    public bool IsFull()
    {
        return ListShipData.All(shipData => shipData != null);
    }

    public bool IsEmpty()
    {
        return _listShipData.All(shipData => shipData == null);
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

    public int GetReceivedDownLevel(int targetLevel)
    {
        int sum = 0;
        for (int i = targetLevel; i < level; i++)
        {
            sum += spaceStationSo.listCostPerLevel[i];
        }

        return sum;
    }

    public int GetPendingMineValue()
    {
        int sum = 0;
        for (int i = level; i < pendingDownGrade; i++)
        {
            sum += spaceStationSo.listCostPerLevel[i];
        }

        return sum;
    }
}