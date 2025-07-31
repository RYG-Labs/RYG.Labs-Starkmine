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
    [SerializeField] private TextMeshProUGUI upgradeButtonText;
    [SerializeField] private Button downgradeButton;
    [SerializeField] private TextMeshProUGUI stationLevel;
    [SerializeField] private Button pendingMineButton;
    [SerializeField] private Button cancelDowngradeButton;
    [SerializeField] private TextMeshProUGUI pendingMineText;

    // [SerializeField] private PendingMineUI pendingMineUI;
    private StationData _stationData;
    private int _indexSelected = -1;

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnChangeStationEventHandler += GameManagerOnOnChangeStationEventHandler;
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        downgradeButton.onClick.AddListener(OnDowngradeButtonClick);
        pendingMineButton.onClick.AddListener(OnClickPendingMineButton);
        cancelDowngradeButton.onClick.AddListener(OnClickCancelDownGradeButton);
        HandleIsPending(_stationData.pendingDownGrade != 0);
    }

    private void OnClickCancelDownGradeButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseCancelDowngradeEventHandler += InstanceOnOnResponseCancelDowngradeEventHandler;
        WebResponse.Instance.OnResponseCancelDowngradeFailEventHandler +=
            InstanceOnOnResponseCancelDowngradeFailEventHandler;
        WebRequest.CallRequestCancelDowngrade(_stationData.id);
    }

    private void InstanceOnOnResponseCancelDowngradeFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseCancelDowngradeEventHandler -= InstanceOnOnResponseCancelDowngradeEventHandler;
        WebResponse.Instance.OnResponseCancelDowngradeFailEventHandler -=
            InstanceOnOnResponseCancelDowngradeFailEventHandler;
    }

    private void InstanceOnOnResponseCancelDowngradeEventHandler(object sender,
        WebResponse.OnResponseCancelDowngradeEventArgs e)
    {
        _stationData.pendingMineTime = 0;
        _stationData.level = _stationData.pendingDownGrade;
        _stationData.pendingDownGrade = 0;
        HandleIsPending(false);
        RefreshStationInformation(_stationData);
        UIManager.Instance.userInfoUI.UpdateSuggestStation(_stationData);
        WebResponse.Instance.OnResponseCancelDowngradeEventHandler -= InstanceOnOnResponseCancelDowngradeEventHandler;
        WebResponse.Instance.OnResponseCancelDowngradeFailEventHandler -=
            InstanceOnOnResponseCancelDowngradeFailEventHandler;
    }

    public void HandleIsPending(bool isPending)
    {
        cancelDowngradeButton.gameObject.SetActive(isPending);
        pendingMineButton.gameObject.SetActive(isPending);
        upgradeButton.gameObject.SetActive(!isPending);
        downgradeButton.gameObject.SetActive(!isPending && _stationData.level > 0);
    }

    private void CountDowngradeStationChangeEventHandler(object sender, EventArgs e)
    {
        if (_stationData.pendingMineTime == 0)
        {
            pendingMineText.text = $"Claim {Helpers.FormatCurrencyNumber(_stationData.GetPendingMineValue())} $Mine";
            pendingMineButton.interactable = true;
            return;
        }

        pendingMineButton.interactable = false;
        pendingMineText.text =
            $"Pending {Helpers.FormatCurrencyNumber(_stationData.GetPendingMineValue())} $Mine: {Helpers.TimeFormater(_stationData.pendingMineTime)}";
    }

    private void OnClickPendingMineButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseExecuteDowngradeEventHandler += InstanceOnOnResponseExecuteDowngradeEventHandler;
        WebResponse.Instance.OnResponseExecuteDowngradeFailEventHandler +=
            InstanceOnOnResponseExecuteDowngradeFailEventHandler;
        WebRequest.CallRequestExecuteDowngrade(_stationData.id);
    }

    private void InstanceOnOnResponseExecuteDowngradeFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseExecuteDowngradeEventHandler -= InstanceOnOnResponseExecuteDowngradeEventHandler;
        WebResponse.Instance.OnResponseExecuteDowngradeFailEventHandler -=
            InstanceOnOnResponseExecuteDowngradeFailEventHandler;
    }

    private void InstanceOnOnResponseExecuteDowngradeEventHandler(object sender,
        WebResponse.OnResponseExecuteDowngradeEventArgs e)
    {
        _stationData.pendingMineTime = 0;
        _stationData.pendingDownGrade = 0;
        HandleIsPending(false);
        RefreshStationInformation(_stationData);
        WebResponse.Instance.OnResponseExecuteDowngradeEventHandler -= InstanceOnOnResponseExecuteDowngradeEventHandler;
        WebResponse.Instance.OnResponseExecuteDowngradeFailEventHandler -=
            InstanceOnOnResponseExecuteDowngradeFailEventHandler;
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
        if (_stationData.IsMaxLevel()) return;
        SoundManager.Instance.PlayConfirmSound3();
        UpgradeSpaceStationUI upgradeSpaceShipUI = UIManager.Instance.upgradeSpaceStationUI;
        upgradeSpaceShipUI.SetUp(_stationData);
        upgradeSpaceShipUI.Show();
    }

    private void GameManagerOnOnChangeStationEventHandler(object sender,
        GameManager.OnChangeStationEventHandlerEventArgs e)
    {
        RefreshShip(e.ListShipInNewPlanet);
        _stationData = e.CurrentStation;
        RefreshStationInformation(e.CurrentStation);
        HandleIsPending(_stationData.pendingDownGrade != 0);
    }

    private void OnEnable()
    {
        ShipData[] listShipData = GameManager.Instance.GetShipOnCurrentPlanet();
        _stationData = GameManager.Instance.CurrentStation;
        RefreshStationInformation(_stationData);
        RefreshShip(listShipData);

        DataManager.Instance.OnAddShipToPlanetHandler += DataManagerOnOnAddShipToPlanetHandler;
        DataManager.Instance.OnRemoveShipToPlanetHandler += DataManagerOnRemoveShipToPlanetHandler;
        DataManager.Instance.OnCountDowngradeStationChangeEventHandler += CountDowngradeStationChangeEventHandler;
    }


    public void RefreshStationInformation(StationData stationData)
    {
        stationLevel.text = $"Station LV{stationData.level}";
        upgradeButtonText.text = stationData.IsMaxLevel() ? "Max Level" : "Upgrade";
        downgradeButton.gameObject.SetActive(stationData.CanDowngrade());
        HandleIsPending(_stationData.pendingDownGrade != 0);
    }

    private void RefreshShip(ShipData[] listShipData)
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

        DataManager.Instance.OnCountDowngradeStationChangeEventHandler -= CountDowngradeStationChangeEventHandler;
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