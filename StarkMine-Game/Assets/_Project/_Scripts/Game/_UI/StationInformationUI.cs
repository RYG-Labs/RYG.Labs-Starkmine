using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.UI;

public class StationInformationUI : MonoBehaviour
{
    [SerializeField] private ItemStationInformationUI prefabItemStationInformationUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemStationInformationUI> listItem;

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDisable()
    {
        containerItem.DestroyChildren();
    }

    public void CreateItemStationInformationUI(ShipSO.ShipType shipType)
    {
        ItemStationInformationUI basicMiner = Instantiate(prefabItemStationInformationUI, containerItem);
        PlanetSO currentPlanet = GameManager.Instance.CurrentPlanetId;
        string minerType = $"{shipType.ToString()} Miner";
        int amount = DataManager.Instance.SumAmountOfTypeShipInPlanet(shipType, currentPlanet);
        float hashPower = DataManager.Instance.SumHashPowerOfTypeShipInPlanet(shipType, currentPlanet);
        basicMiner.SetUp(minerType, amount, hashPower);
        listItem.Add(basicMiner);
    }

    public void Refresh()
    {
        containerItem.DestroyChildren();
        CreateItemStationInformationUI(ShipSO.ShipType.Basic);
        CreateItemStationInformationUI(ShipSO.ShipType.Elite);
        CreateItemStationInformationUI(ShipSO.ShipType.Pro);
        CreateItemStationInformationUI(ShipSO.ShipType.GIGA);
    }
}