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

    // [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image coreEngineImage;

    [SerializeField] private Button upgradeButton;

    // [SerializeField] private Button repairButton;
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
        shipName.text = $"{shipData.shipSO.shipName} #{shipData.id}";
        starLevelUI.SetUp(shipData.level);
        // titleText.text = $"{shipData.shipSO.shipName} Spaceship #{shipData.id}";
        if (shipData.CoreEngineData != null)
        {
            maintenanceLevelUI.gameObject.SetActive(true);
            maintenanceLevelUI.SetUp(Mathf.CeilToInt(shipData.CoreEngineData.GetDurabilityPercentage() * 100),
                Mathf.CeilToInt(shipData.CoreEngineData.GetEarningRate() * 100 - 100));
        }
        else
        {
            maintenanceLevelUI.gameObject.SetActive(false);
        }

        // repairButton.interactable = shipData.maintenanceLevel < 100;
        hashPower.text = $"{shipData.GetHashPower()}";
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
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineByShipType(shipData.shipSO.shipType);
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
        // repairButton.onClick.AddListener(OnClickRepairButton);
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
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseExtinguishMinerEventHandler += WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler +=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
        WebRequest.CallRequestExtinguishMiner(_shipData.id);
    }

    private void WebResponseOnResponseExtinguishMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseExtinguishMinerEventHandler -= WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler -=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
    }

    private void WebResponseOnResponseExtinguishMinerEventHandler(object sender,
        WebResponse.OnResponseExtinguishMinerEventArgs e)
    {
        removeGroup.gameObject.SetActive(false);
        bool success = GameManager.Instance.CallbackSpaceShip(_shipData);
        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineDataById(e.Data.CoreEngineDto.tokenId);
        coreEngineData.blockUsed = e.Data.CoreEngineDto.blocksUsed;
        SoundManager.Instance.PlayDataPointSound1();
        if (success)
        {
            ItemSpaceStationUI.SpaceShipOnCallbackHandler();
            SetUp(_shipData, _shipIndex);
        }

        WebResponse.Instance.OnResponseExtinguishMinerEventHandler -= WebResponseOnResponseExtinguishMinerEventHandler;
        WebResponse.Instance.OnResponseExtinguishMinerFailEventHandler -=
            WebResponseOnResponseExtinguishMinerFailEventHandler;
    }

    private void OnClickCallbackToRemoveButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        callbackToRemoveButton.gameObject.SetActive(false);
    }

    private void OnClickCoreEngineButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        removeGroup.gameObject.SetActive(true);
    }

    private void OnClickRequireCoreEngineButton()
    {
        SoundManager.Instance.PlayConfirmSound3();

        bool isContainCoreEngineRequire =
            DataManager.Instance.IsContainCoreEngineRequireInInventory(_shipData.shipSO.shipType);
        if (!isContainCoreEngineRequire)
        {
            DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI = UIManager.Instance.dontHaveRequireCoreEngineUI;
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineByShipType(_shipData.shipSO.shipType);
            dontHaveRequireCoreEngineUI.SetUp(coreEngineRequire);
            dontHaveRequireCoreEngineUI.Show();
            return;
        }

        ChooseCoreEngineUI chooseCoreEngineUI = UIManager.Instance.chooseCoreEngineUI;
        chooseCoreEngineUI.SetUpAndShow(_shipData);
        chooseCoreEngineUI.OnItemSelectEventHandler += ChooseCoreEngineUIOnItemSelectEventHandler;
    }

    private void ChooseCoreEngineUIOnItemSelectEventHandler(object sender, ChooseCoreEngineUI.OnItemSelectEventArgs e)
    {
        SoundManager.Instance.PlayBleepSound1();
        CoreEngineData coreEngineData = e.CoreEngineData;
        WebResponse.Instance.OnResponseIgniteMinerEventHandler += WebResponseOnResponseIgniteMinerEventHandler;
        WebResponse.Instance.OnResponseIgniteMinerFailEventHandler += WebResponseOnResponseIgniteMinerFailEventHandler;
        WebRequest.CallRequestIgniteMiner(_shipData.id, coreEngineData.id);
        UIManager.Instance.loadingUI.Show();

        ChooseCoreEngineUI chooseCoreEngineUI = UIManager.Instance.chooseCoreEngineUI;
        chooseCoreEngineUI.OnItemSelectEventHandler -= ChooseCoreEngineUIOnItemSelectEventHandler;
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
        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineDataById(e.Data.CoreEngineDto.tokenId);
        coreEngineData.lastUsedBlock = e.Data.CoreEngineDto.lastUsedBlock;
        DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineData, _shipData);
        GameManager.Instance.LaunchSpaceShip(_shipData);
        SetUp(_shipData, _shipIndex);
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
        if (_shipData.CoreEngineData == null)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("Don't have Core-Engine in this ship.");
            showNotificationUI.Show();
            return;
        }

        if ((int)_shipData.CoreEngineData.GetDurabilityPercentage() == 100)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("The Core Engine is currently in its best condition, no repair needed.");
            showNotificationUI.Show();
            return;
        }

        UIManager.Instance.repairCoreEngineUI.SetupAndShow(_shipData.CoreEngineData);
        Hide();
        // repairButton.interactable = false;
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