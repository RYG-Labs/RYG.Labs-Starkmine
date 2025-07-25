using _Project._Scripts.Game.Managers;

public class UIManager : StaticInstance<UIManager>
{
    public SpaceStationUI spaceStationUI;
    public ChooseSpaceShipUI chooseSpaceShipUI;
    public ShipInformationUI shipInformationUI;
    public DontHaveRequireCoreEngineUI dontHaveRequireCoreEngineUI;
    public CreateCoreEngineUI createCoreEngineUI;
    public YesNoUI yesNoUI;
    public UpgradeSpaceShipUI upgradeSpaceShipUI;
    public UpgradeSpaceStationUI upgradeSpaceStationUI;
    public DowngradeSpaceStationUI downgradeSpaceStationUI;
    public HangarUI hangarUI;
    public MergeSpaceshipUI mergeSpaceshipUI;

    public DefuseCoreEngineUI2 defuseCoreEngineUI;

    // public DefuseCoreEngineUI defuseCoreEngineUI;
    public LoadingUI loadingUI;
    public ConnectWalletUI connectWalletUI;
    public UserInfoUI userInfoUI;
    public ShowNotificationUI showNotificationUI;
    public ShowNotificationCantOffUI showNotificationCantOffUI;
    public TabPlanetUI tabPlanetUI;
    public ChooseCoreEngineUI chooseCoreEngineUI;
    public RepairCoreEngineUI repairCoreEngineUI;
    public ChooseCoreEngineToRepairUI chooseCoreEngineToRepairUI;
    public InitStationUI initStationUI;
    public CheckInStreakUI checkInStreakUI;
    public OpenTicketUI openTicketUI;
    public bool isHoverUI;

    private void Start()
    {
        WebResponse.Instance.OnResponseConnectWalletEventHandler += WebResponseOnResponseConnectWalletEventHandler;
    }

    private void WebResponseOnResponseConnectWalletEventHandler(object sender,
        WebResponse.OnResponseConnectWalletEventArgs e)
    {
        UserDTO userDto = e.Data;
        WebRequest.CallRequestTotalHashPower();
        WebRequest.CallRequestUserHashPower();
        WebRequest.CallRequestGetPendingReward();
        DataManager.Instance.UserData = new UserData
        {
            Address = userDto.address,
            Balance = userDto.balance,
        };
        DataManager.Instance.MineCoin = userDto.balance;
        connectWalletUI.Hide();
        loadingUI.Show();
    }
}