using System;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Managers;
using UnityEngine;

public class TabPlanetUI : BasePopup
{
    [SerializeField] private ItemTabPlanetUI itemTabPlanetUIPrefab;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private List<ItemTabPlanetUI> listItem = new();
    bool _isFirstShow = true;

    protected override void Start()
    {
        base.Start();
        DataManager.Instance.OnAddShipToPlanetHandler += InstanceOnOnAddShipToPlanetHandler;
    }

    private void InstanceOnOnAddShipToPlanetHandler(object sender, DataManager.OnAddShipToPlanetHandlerEventArgs e)
    {
        SetUpItem();
    }

    private void OnEnable()
    {
        List<StationData> listStationData = DataManager.Instance.listStationData;

        if (_isFirstShow)
        {
            for (int i = 0; i < listStationData.Count; i++)
            {
                StationData stationData = listStationData[i];
                ItemTabPlanetUI itemTabPlanetUI = Instantiate(itemTabPlanetUIPrefab, itemContainer);
                itemTabPlanetUI.Setup(stationData.planetSo, stationData);
                itemTabPlanetUI.OnUse += ItemTabPlanetUIOnOnUse;
                listItem.Add(itemTabPlanetUI);
            }

            _isFirstShow = false;
        }

        SetUpItem();
    }

    private void SetUpItem()
    {
        List<StationData> listStationData = DataManager.Instance.listStationData;
        for (int i = listStationData.Count - 1; i > 0; i--)
        {
            bool isShow = IsShowStation(listStationData[i], listStationData[i - 1]);
            listItem[i].gameObject
                .SetActive(isShow);
            if (isShow)
            {
                break;
            }
        }
    }

    private void ItemTabPlanetUIOnOnUse(object sender, ItemTabPlanetUI.OnUseEventArgs e)
    {
        GameManager.Instance.MoveToNewStation(e.StationData);
    }

    private bool IsShowStation(StationData stationData, StationData preStationData)
    {
        return stationData.level > 0 || !stationData.IsEmpty() || preStationData.IsFull() ||
               stationData.pendingDownGrade != 0;
    }
}