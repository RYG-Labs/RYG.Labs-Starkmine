using UnityEngine;

public class UIManager : StaticInstance<UIManager>
{
    public ShopUI shopUI;
    public SpaceSackUI spaceSackUI;
    public SpaceStationUI spaceStationUI;
    public ChooseSpaceShipUI chooseSpaceShipUI;
    public ShipInformationUI shipInformationUI;
    public DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI;
    public CreateCoreEngineUI createCoreEngineUI;
    public YesNoUI yesNoUI;
    public ShowNotificationUI showNotificationUI;
    public UpgradeSpaceShipUI upgradeSpaceShipUI;
    public UpgradeSpaceStationUI upgradeSpaceStationUI;
    public DowngradeSpaceStationUI downgradeSpaceStationUI;
    public HangarUI hangarUI;
    public MergeSpaceshipUI mergeSpaceshipUI;
    public DefuseCoreEngineUI defuseCoreEngineUI;
    public bool isHoverUI;

    public void HideAllUI()
    {
        shopUI.Hide();
        spaceSackUI.Hide();
    }
}