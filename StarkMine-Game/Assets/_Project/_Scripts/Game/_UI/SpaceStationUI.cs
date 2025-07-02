using System;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceStationUI : BasePopup
{
    [SerializeField] private ItemSpaceStationUI prefabItemSpaceStationUI;
    [SerializeField] private Transform containerItemSpaceStationUI;
    [SerializeField] private Button showButton;
    [SerializeField] private List<ItemSpaceStationUI> listItem = new();
    [SerializeField] private StationInformationUI stationInformationUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button downgradeButton;
    [SerializeField] private TextMeshProUGUI stationLevel;
    private StationData _stationData;
    private int _indexSelected = -1;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnChangeStationEventHandler += GameManagerOnOnChangeStationEventHandler;
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        downgradeButton.onClick.AddListener(OnDowngradeButtonClick);
    }

    private void OnDowngradeButtonClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        DowngradeSpaceStationUI upgradeSpaceShipUI = UIManager.Instance.downgradeSpaceStationUI;
        upgradeSpaceShipUI.SetUp(_stationData);
        upgradeSpaceShipUI.Show();
    }

    private void OnUpgradeButtonClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        UpgradeSpaceStationUI upgradeSpaceShipUI = UIManager.Instance.upgradeSpaceStationUI;
        upgradeSpaceShipUI.SetUp(_stationData);
        upgradeSpaceShipUI.Show();
    }

    private void GameManagerOnOnChangeStationEventHandler(object sender,
        GameManager.OnChangeStationEventHandlerEventArgs e)
    {
        Refresh(e.ListShipInNewPlanet);
        _stationData = e.CurrentStation;
        RefreshStationInformation(e.CurrentStation);
    }

    private void OnEnable()
    {
        ShipData[] listShipData = GameManager.Instance.GetShipOnCurrentPlanet();
        _stationData = GameManager.Instance.CurrentStation;
        RefreshStationInformation(_stationData);
        Refresh(listShipData);
        DataManager.Instance.OnAddShipToPlanetHandler += DataManagerOnOnAddShipToPlanetHandler;
        DataManager.Instance.OnRemoveShipToPlanetHandler += DataManagerOnRemoveShipToPlanetHandler;
    }


    public void RefreshStationInformation(StationData stationData)
    {
        stationLevel.text = $"Station LV{stationData.level}";
        downgradeButton.gameObject.SetActive(stationData.CanDowngrade());
    }

    private void Refresh(ShipData[] listShipData)
    {
        containerItemSpaceStationUI.DestroyChildren();
        listItem.Clear();
        for (int i = 0; i < 6; i++)
        {
            ItemSpaceStationUI itemSpaceStationUI = Instantiate(prefabItemSpaceStationUI, containerItemSpaceStationUI);
            ShipData shipData = listShipData[i];
            if (shipData != null)
            {
                itemSpaceStationUI.SetUp(i, shipData, false);
            }
            else
            {
                itemSpaceStationUI.SetUp(i, null, true);
            }

            listItem.Add(itemSpaceStationUI);
            itemSpaceStationUI.OnClickHandler += ItemSpaceStationUIOnOnClickHandler;
        }

        stationInformationUI.Refresh();
    }

    private void ItemSpaceStationUIOnOnClickHandler(object sender, ItemSpaceStationUI.OnClickHandlerEventArgs e)
    {
        ItemSpaceStationUI itemSpaceStationUI = sender as ItemSpaceStationUI;
        if (itemSpaceStationUI == null) return;
        SoundManager.Instance.PlayConfirmSound3();
        _indexSelected = e.ItemIndex;
        if (e.IsEmpty)
        {
            ChooseSpaceShipUI chooseSpaceShipUI = UIManager.Instance.chooseSpaceShipUI;
            chooseSpaceShipUI.spaceShipSelectedIndex = _indexSelected;
            chooseSpaceShipUI.Show();
        }
        else
        {
            // ShipData[] listShipData = GameManager.Instance.GetShipOnCurrentPlanet();
            ShipInformationUI shipInformationUI = UIManager.Instance.shipInformationUI;
            shipInformationUI.ItemSpaceStationUI = itemSpaceStationUI;
            shipInformationUI.SetUp(itemSpaceStationUI.ShipData, _indexSelected);
            shipInformationUI.Show();
            // UIManager.Instance.shipInformationUI.transform.position = transform.position;
        }
    }

    private void DataManagerOnRemoveShipToPlanetHandler(object sender, DataManager.OnRemoveShipToPlanetEventArgs e)
    {
        ItemSpaceStationUI itemEmpty = listItem[_indexSelected];
        if (itemEmpty == null) return;
        itemEmpty.SetUp(itemEmpty.Index, null, true);
        stationInformationUI.Refresh();
    }

    private void DataManagerOnOnAddShipToPlanetHandler(object sender, DataManager.OnAddShipToPlanetHandlerEventArgs e)
    {
        ItemSpaceStationUI itemEmpty = listItem[_indexSelected];
        if (itemEmpty == null) return;
        ShipData shipData = e.NewShipData;
        itemEmpty.SetUp(itemEmpty.Index, shipData, false);
        stationInformationUI.Refresh();
    }

    private ItemSpaceStationUI GetEmptyItemSpaceStationUI()
    {
        return listItem.FirstOrDefault(itemSpaceStationUI => itemSpaceStationUI.IsEmpty);
    }

    private void OnDisable()
    {
        containerItemSpaceStationUI.DestroyChildren();
        listItem.Clear();
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnAddShipToPlanetHandler -= DataManagerOnOnAddShipToPlanetHandler;
        }
    }

    public override void Show()
    {
        base.Show();
        showButton.gameObject.SetActive(false);
        SoundManager.Instance.PlayClickSound2();
    }

    public override void Hide()
    {
        base.Hide();
        showButton.gameObject.SetActive(true);
    }

    public void LaunchSpaceShip(ShipData shipData)
    {
        foreach (ItemSpaceStationUI itemSpaceStationUI in listItem)
        {
            if (itemSpaceStationUI.ShipData == shipData)
            {
                itemSpaceStationUI.SpaceShipOnDutyHandler();
            }
        }
    }
}