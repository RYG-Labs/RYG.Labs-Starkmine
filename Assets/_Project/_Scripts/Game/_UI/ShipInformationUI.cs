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


        if (shipData.CoreEngine == null)
        {
            CoreEngineSO coreEngineRequire = DataManager.Instance.GetCoreEngineRequire(shipData.shipSO.shipType);
            requireCoreEngineText.text = $"Core {coreEngineRequire.nameCoreEngine} required";
            coreEngineLevel.text = coreEngineRequire.nameCoreEngine;
            requireCoreEngineGroup.gameObject.SetActive(true);
            coreEngineImage.gameObject.SetActive(false);
        }
        else
        {
            coreEngineLevel.text = shipData.CoreEngine.nameCoreEngine;
            requireCoreEngineGroup.gameObject.SetActive(false);
            coreEngineImage.sprite = shipData.CoreEngine.sprite;
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
        removeGroup.gameObject.SetActive(false);
    }

    private void OnClickYesRemoveCoreEngineButton()
    {
        removeGroup.gameObject.SetActive(false);
        CoreEngineSO coreEngineSo = _shipData.CoreEngine;
        DataManager.Instance.AddCoreEngine(coreEngineSo, 1);
        _shipData.CoreEngine = null;
        SetUp(_shipData, _shipIndex);
    }

    private void OnClickCallbackToRemoveButton()
    {
        callbackToRemoveButton.gameObject.SetActive(false);
    }

    private void OnClickCoreEngineButton()
    {
        if (_shipData.onDuty)
        {
            callbackToRemoveButton.gameObject.SetActive(true);
        }
        else
        {
            removeGroup.gameObject.SetActive(true);
        }
    }

    private void OnClickRequireCoreEngineButton()
    {
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
        yesNoUI.OnYesButtonClickEventHandler += OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += OnNoButtonClickEventHandler;
        yesNoUI.SetUp(
            $"Use 1 core engine <color=#FEE109>{DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType).nameCoreEngine}</color> for this spaceship?");
        yesNoUI.Show();
    }

    private void OnYesButtonClickEventHandler(object sender, EventArgs e)
    {
        CoreEngineSO coreEngineSo = DataManager.Instance.GetCoreEngineRequire(_shipData.shipSO.shipType);
        DataManager.Instance.AddCoreEngineToSpaceShip(coreEngineSo, _shipData);

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickEventHandler;
        SetUp(_shipData, _shipIndex);
    }

    private void OnNoButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= OnYesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= OnNoButtonClickEventHandler;
    }

    private void OnClickLaunchButton()
    {
    }

    private void OnClickRemoveButton()
    {
        if (_shipData.onDuty)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("Please callback before trying to remove the ship.");
            showNotificationUI.Show();
            return;
        }

        if (_shipData.CoreEngine != null)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("Please remove core engine before trying to remove the ship.");
            showNotificationUI.Show();
            return;
        }

        GameManager.Instance.RemoveShipToCurrentPlanet(_shipData, _shipIndex);
        Hide();
    }

    private void OnClickRepairButton()
    {
        if (_shipData.maintenanceLevel < 100)
        {
            // int newMaintenanceLevel = _shipData.maintenanceLevel + 10;
            // _shipData.maintenanceLevel = newMaintenanceLevel <= 100 ? newMaintenanceLevel : 100;
            _shipData.maintenanceLevel = 100;
        }

        maintenanceLevelUI.SetUp(_shipData.maintenanceLevel, 0);
    }

    private void OnClickUpgradeButton()
    {
        if (_shipIndex == -1) return;
        UpgradeSpaceShipUI upgradeSpaceShipUI = UIManager.Instance.upgradeSpaceShipUI;
        upgradeSpaceShipUI.SetUp(_shipData);
        upgradeSpaceShipUI.Show();
        Hide();
    }
}