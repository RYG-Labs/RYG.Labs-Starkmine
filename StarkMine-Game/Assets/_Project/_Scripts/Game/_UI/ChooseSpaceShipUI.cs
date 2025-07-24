using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;

public class ChooseSpaceShipUI : BasePopup
{
    [SerializeField] private ItemChooseSpaceShipUI prefabItemChooseSpaceShipUI;
    [SerializeField] private Transform containerItemChooseSpaceShipUI;
    [SerializeField] private List<ItemChooseSpaceShipUI> listItem = new();
    [SerializeField] private FilterSpaceShipUI filterSpaceShipUI;
    [SerializeField] private TextMeshProUGUI countBasicShipText;
    [SerializeField] private TextMeshProUGUI countEliteShipText;
    [SerializeField] private TextMeshProUGUI countProShipText;
    [SerializeField] private TextMeshProUGUI countGIGAShipText;
    public int spaceShipSelectedIndex = -1;

    protected override void Start()
    {
        base.Start();
        WebResponse.Instance.OnResponseAssignMinerToStationEventHandler +=
            WebResponseOnResponseAssignMinerToStationEventHandler;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, 1, 0,
            isAll: true, isAllType: true));
    }


    private void OnEnable()
    {
        countBasicShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Basic).ToString();
        countEliteShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Elite).ToString();
        countProShipText.text = DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Pro).ToString();
        countGIGAShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.GIGA).ToString();
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, 1, 0,
            isAll: true));
        filterSpaceShipUI.OnOptionFilterChangeEventHandler += FilterSpaceShipUIOnOnOptionFilterChangeEventHandler;
    }

    public void SetUp(List<ShipData> listShipData)
    {
        for (int i = 0; i < listShipData.Count; i++)
        {
            ItemChooseSpaceShipUI itemSpaceStationUI =
                Instantiate(prefabItemChooseSpaceShipUI, containerItemChooseSpaceShipUI);
            itemSpaceStationUI.SetUp(i, listShipData[i]);
            itemSpaceStationUI.OnYesButtonClickHandler += ItemSpaceStationUIOnOnYesButtonClickHandler;
            listItem.Add(itemSpaceStationUI);
        }
    }

    private void FilterSpaceShipUIOnOnOptionFilterChangeEventHandler(object sender,
        FilterSpaceShipUI.OnOptionFilterChangeEventArgs e)
    {
        Clear();
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(e.ListToggleSelected, 1, 0, isAll: true));
    }

    private void ItemSpaceStationUIOnOnYesButtonClickHandler(object sender,
        ItemChooseSpaceShipUI.OnYesButtonClickHandlerEventArgs e)
    {
        SoundManager.Instance.PlayConfirmSound1();
        UIManager.Instance.loadingUI.Show();
        WebRequest.CallRequestAssignMinerToStation(GameManager.Instance.CurrentStation.id, e.ShipData.id,
            spaceShipSelectedIndex + 1);
        Hide();
    }

    private void WebResponseOnResponseAssignMinerToStationEventHandler(object sender,
        WebResponse.OnResponseAssignMinerToStationEventArgs e)
    {
        ShipData shipData = DataManager.Instance.GetShipDataById(e.Data.minerId);
        Debug.Log(shipData.id);
        GameManager.Instance.AddShipToCurrentStation(shipData, e.Data.minerSlot - 1);
    }

    private void OnDisable()
    {
        Clear();
        spaceShipSelectedIndex = -1;
        filterSpaceShipUI.OnOptionFilterChangeEventHandler -= FilterSpaceShipUIOnOnOptionFilterChangeEventHandler;
    }

    public void Clear()
    {
        containerItemChooseSpaceShipUI.DestroyChildren();
        listItem.Clear();
    }

    private void ResetIndex()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].Index = i;
        }
    }
}