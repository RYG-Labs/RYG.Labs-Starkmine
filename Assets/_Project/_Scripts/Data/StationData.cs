using System;
using UnityEngine;

[Serializable]
public class StationData
{
    public event EventHandler<OnStationDataChangedEventArgs> OnStationDataChangedEventHandler;

    public class OnStationDataChangedEventArgs : EventArgs
    {
        public StationData NewStationData;
    }

    [SerializeField] public int level;

    [SerializeField] public SpaceStationSO spaceStationSo;

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
        return level > 1;
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
        return level <= 1;
    }
}