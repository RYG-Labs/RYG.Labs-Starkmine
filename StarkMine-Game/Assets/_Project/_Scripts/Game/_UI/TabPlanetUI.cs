using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Managers;
using UnityEngine;

public class TabPlanetUI : BasePopup
{
    [SerializeField] private ItemTabPlanetUI itemTabPlanetUIPrefab;
    [SerializeField] private Transform itemContainer;

    protected override void Start()
    {
        base.Start();
        List<StationData> listStationData = DataManager.Instance.listStationData;
        for (int i = 0; i < listStationData.Count; i++)
        {
            ItemTabPlanetUI itemTabPlanetUI = Instantiate(itemTabPlanetUIPrefab, itemContainer);
            itemTabPlanetUI.Setup(listStationData[i].planetSo, listStationData[i]);
            itemTabPlanetUI.OnUse += ItemTabPlanetUIOnOnUse;
        }
    }

    private void ItemTabPlanetUIOnOnUse(object sender, ItemTabPlanetUI.OnUseEventArgs e)
    {
        GameManager.Instance.MoveToNewStation(e.StationData);
    }
}