using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;

public class ChooseCoreEngineToRepairUI : BasePopup
{
    [SerializeField] private ItemChooseCoreEngineUI prefabItemChooseSpaceShipUI;
    [SerializeField] private Transform containerItemChooseSpaceShipUI;
    [SerializeField] private List<ItemChooseCoreEngineUI> listItem = new();
    [SerializeField] private FilterCoreEngineUI filterCoreEngineUI;
    [SerializeField] private TextMeshProUGUI countBasicText;
    [SerializeField] private TextMeshProUGUI countEliteText;
    [SerializeField] private TextMeshProUGUI countProText;
    [SerializeField] private TextMeshProUGUI countGIGAText;

    protected override void Start()
    {
        base.Start();
        WebResponse.Instance.OnResponseAssignMinerToStationEventHandler +=
            WebResponseOnResponseAssignMinerToStationEventHandler;
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(filterCoreEngineUI.ListToggleIndexSelected));
    }

    private void OnEnable()
    {
        countBasicText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Basic, false)
                .ToString();
        countEliteText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Elite, false)
                .ToString();
        countProText.text = DataManager.Instance
            .CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Pro, false).ToString();
        countGIGAText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.GIGA, false)
                .ToString();
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(filterCoreEngineUI.ListToggleIndexSelected));
        filterCoreEngineUI.OnOptionFilterChangeEventHandler += FilterCoreEngineUIOnOnOptionFilterChangeEventHandler;
    }

    public void SetUp(List<CoreEngineData> listCoreEngineData)
    {
        for (int i = 0; i < listCoreEngineData.Count; i++)
        {
            ItemChooseCoreEngineUI itemChooseCoreEngineUI =
                Instantiate(prefabItemChooseSpaceShipUI, containerItemChooseSpaceShipUI);
            itemChooseCoreEngineUI.SetUp(i, listCoreEngineData[i],
                listCoreEngineData[i].GetDurabilityPercentage() < 1);
            itemChooseCoreEngineUI.OnYesButtonClickHandler += ItemChooseCoreEngineUIOnOnYesButtonClickHandler;
            listItem.Add(itemChooseCoreEngineUI);
        }
    }

    private void FilterCoreEngineUIOnOnOptionFilterChangeEventHandler(object sender,
        FilterCoreEngineUI.OnOptionFilterChangeEventArgs e)
    {
        Clear();
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(e.ListToggleSelected));
    }

    private void ItemChooseCoreEngineUIOnOnYesButtonClickHandler(object sender,
        ItemChooseCoreEngineUI.OnYesButtonClickHandlerEventArgs e)
    {
        UIManager.Instance.repairCoreEngineUI.SetupAndShow(e.CoreEngineData);
        Hide();
    }

    private void WebResponseOnResponseAssignMinerToStationEventHandler(object sender,
        WebResponse.OnResponseAssignMinerToStationEventArgs e)
    {
        ShipData shipData = DataManager.Instance.GetShipDataById(e.Data.minerId);
        GameManager.Instance.AddShipToCurrentStation(shipData, e.Data.minerSlot - 1);
    }

    private void OnDisable()
    {
        Clear();
        filterCoreEngineUI.OnOptionFilterChangeEventHandler -= FilterCoreEngineUIOnOnOptionFilterChangeEventHandler;
    }

    public void Clear()
    {
        containerItemChooseSpaceShipUI.DestroyChildren();
        listItem.Clear();
    }
}