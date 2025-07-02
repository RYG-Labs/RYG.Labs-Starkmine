using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipInformationUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI shipName;
    [SerializeField] private TextMeshProUGUI hashPower;
    [SerializeField] private StarLevelUI starLevelUI;
    [SerializeField] private MaintenanceLevelUI maintenanceLevelUI;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI coreEngineLevel;
    [SerializeField] private Image coreEngineImage;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button repairButton;
    [SerializeField] private Button launchButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button requireCoreEngineGroup;
    [SerializeField] private TextMeshProUGUI requireCoreEngineText;
    [SerializeField] private GameObject removeGroup;
    [SerializeField] private Button yesRemoveCoreEngineButton;
    [SerializeField] private Button noRemoveCoreEngineButton;
    [SerializeField] private Button coreEngineButton;
    [SerializeField] private Button callbackToRemoveButton;
    [SerializeField] public ItemSpaceStationUI ItemSpaceStationUI { get; set; }
    private int _shipIndex = -1;
    private ShipData _shipData;


    public void SetUp(ShipData shipData, int shipIndex)
    {
        requireCoreEngineGroup.gameObject.SetActive(false);
        callbackToRemoveButton.gameObject.SetActive(false);
        removeGroup.SetActive(false);
        _shipData = shipData;
        _shipIndex = shipIndex;
        shipName.text = shipData.shipSO.shipName;
        starLevelUI.SetUp(shipData.level);
        maintenanceLevelUI.SetUp(shipData.maintenanceLevel, shipData.maintenanceDown);
        repairButton.interactable = shipData.maintenanceLevel < 100;
        hashPower.text = $"177,765.12 - {shipData.GetHashPower()} $MINE/min";
        if (shipData.level < 5)
        {
            upgradeButton.interactable = true;
            description.text =
                $"Upgrade to <color=#FEE109>Lv{shipData.level + 1}</color> to reach <color=#FEE109>x1.{shipData.level + 1}</color> multiplier";
        }
        else
        {
            upgradeButton.interactable = false;
            description.text = "<color=#FEE109>Max Lv</color>";
        }


        if (shipData.CoreEngineData == null)
        {
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineRequire(shipData.shipSO.shipType);
            requireCoreEngineText.text = $"Core {coreEngineRequire.nameCoreEngine} required";
            coreEngineLevel.text = coreEngineRequire.nameCoreEngine;
            requireCoreEngineGroup.gameObject.SetActive(true);
            coreEngineImage.gameObject.SetActive(false);
        }
        else
        {
            coreEngineLevel.text = shipData.CoreEngineData.coreEngineSO.nameCoreEngine;
            requireCoreEngineGroup.gameObject.SetActive(false);
            coreEngineImage.sprite = shipData.CoreEngineData.coreEngineSO.sprite;
            coreEngineImage.gameObject.SetActive(true);
        }
    }

    protected override void Start()
    {
        base.Start();
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        repairButton.onClick.AddListener(OnClickRepairButton);
        launchButton.onClick.AddListener(OnClickLaunchButton);
        removeButton.onClick.AddListener(OnClickRemoveButton);
        coreEngineButton.onClick.AddListener(OnClickCoreEngineButton);
        callbackToRemoveButton.onClick.AddListener(OnClickCallbackToRemoveButton);
        requireCoreEngineGroup.onClick.AddListener(OnClickRequireCoreEngineButton);
        yesRemoveCoreEngineButton.onClick.AddListener(OnClickYesRemoveCoreEngineButton);
        noRemoveCoreEngineButton.onClick.AddListener(OnClickNoRemoveCoreEngineButton);
    }

    private void OnClickNoRemoveCoreEngineButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        removeGroup.gameObject.SetActive(false);
    }

    private void OnClickYesRemoveCoreEngineButton()
    {
        // SoundManager.Instance.PlayDataPointSound3();
        removeGroup.gameObject.SetActive(false);
        bool success = GameManager.Instance.CallbackSpaceShip(_shipData);
        SoundManager.Instance.PlayDataPointSound1();
        if (success)
        {
            ItemSpaceStationUI.SpaceShipOnCallbackHandler();
        }
    }

    private void OnClickCallbackToRemoveButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        callbackToRemoveButton.gameObject.SetActive(false);
    }

    private void OnClickCoreEngineButton()
    {
        SoundManager.Instance.PlayConfirmSound3();

        // if (_shipData.onDuty)
        // {
        //     callbackToRemoveButton.gameObject.SetActive(true);
        // }
        // else
        // {
        removeGroup.gameObject.SetActive(true);
        // }
    }

    private void OnClickRequireCoreEngineButton()
    {
        SoundManager.Instance.PlayConfirmSound3();

        bool isContainCoreEngineRequire =
            DataManager.Instance.IsContainCoreEngineRequireInInventory(_shipData.shipSO.shipType);
        if (!isContainCoreEngineRequire)
        {
            DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI = UIManager.Instance.dontHaveRequireCoreEngineUI;
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType);
            dontHaveRequireCoreEngineUI.SetUp(coreEngineRequire);
            dontHaveRequireCoreEngineUI.Show();
            return;
        }

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler += OnYesButtonClickAddCoreEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += OnNoButtonClickAddCoreEventHandler;
        yesNoUI.SetUp(
            $"Use 1 core engine <color=#FEE109>{DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType).nameCoreEngine}</color> for this spaceship?");
        yesNoUI.Show();
    }

    private void OnYesButtonClickAddCoreEventHandler(object sender, EventArgs e)
    {
        SoundManager.Instance.PlayBleepSound1();

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickAddCoreEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickAddCoreEventHandler;
        SetUp(_shipData, _shipIndex);

        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineRandomByShipType(_shipData.shipSO.shipType);
        WebResponse.Instance.OnResponseIgniteMinerEventHandler += WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler += WebResponseOnResponseIgniteMinerFailEventHandler;
        WebRequest.CallRequestIgniteMiner(_shipData.id, coreEngineData.id);
        UIManager.Instance.loadingUI.Show();
    }

    private void WebResponseOnResponseIgniteMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseIgniteMinerEventHandler -= WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler -= WebResponseOnResponseIgniteMinerFailEventHandler;
    }

    private void WebResponseOnResponseIgniteMinerEventHandler(object sender,
        WebResponse.OnResponseIgniteMinerEventArgs e)
    {
        // ItemSpaceStationUI.SpaceShipOnCallbackHandler();
        ItemSpaceStationUI.SpaceShipOnDutyHandler();
        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineDataById(e.Data.coreEngineId);
        DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineData, _shipData);
        GameManager.Instance.LaunchSpaceShip(_shipData);
        WebResponse.Instance.OnResponseIgniteMinerEventHandler -= WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler -= WebResponseOnResponseIgniteMinerFailEventHandler;
    }

    private void OnNoButtonClickAddCoreEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickAddCoreEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickAddCoreEventHandler;
    }

    private void OnClickLaunchButton()
    {
    }

    private void OnClickRemoveButton()
    {
        if (_shipData.onDuty)
        {
            SoundManager.Instance.PlayConfirmSound3();
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("Please callback before trying to remove the ship.");
            showNotificationUI.Show();
            return;
        }

        if (_shipData.CoreEngineData != null)
        {
            SoundManager.Instance.PlayConfirmSound3();
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("Please remove core engine before trying to remove the ship.");
            showNotificationUI.Show();
            return;
        }

        WebResponse.Instance.OnResponseRemoveMinerFromStationEventHandler +=
            WebResponseOnResponseRemoveMinerFromStationEventHandler;
        WebRequest.CallRequestRemoveMinerFromStation(GameManager.Instance.CurrentStation.id, _shipIndex);
        UIManager.Instance.loadingUI.Show();
    }

    private void WebResponseOnResponseRemoveMinerFromStationEventHandler(object sender,
        WebResponse.OnResponseRemoveMinerFromStationEventArgs e)
    {
        GameManager.Instance.RemoveShipToCurrentStation(_shipData, _shipIndex);
        SoundManager.Instance.PlayDataPointSound1();
        WebResponse.Instance.OnResponseRemoveMinerFromStationEventHandler -=
            WebResponseOnResponseRemoveMinerFromStationEventHandler;
        Hide();
    }


    private void OnClickRepairButton()
    {
        if (_shipData.maintenanceLevel < 100)
        {
            // int newMaintenanceLevel = _shipData.maintenanceLevel + 10;
            // _shipData.maintenanceLevel = newMaintenanceLevel <= 100 ? newMaintenanceLevel : 100;
            _shipData.maintenanceLevel = 100;
            SoundManager.Instance.PlayConfirmSound6();
        }

        maintenanceLevelUI.SetUp(_shipData.maintenanceLevel, 0);
        repairButton.interactable = false;
    }

    private void OnClickUpgradeButton()
    {
        if (_shipIndex == -1) return;
        SoundManager.Instance.PlayConfirmSound3();
        UpgradeSpaceShipUI upgradeSpaceShipUI = UIManager.Instance.upgradeSpaceShipUI;
        upgradeSpaceShipUI.SetUp(_shipData);
        upgradeSpaceShipUI.Show();
        Hide();
    }
}