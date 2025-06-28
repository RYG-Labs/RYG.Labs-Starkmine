using _Project._Scripts.Game.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
    public DefuseCoreEngineUI defuseCoreEngineUI;
    public LoadingUI loadingUI;
    public ConnectWalletUI connectWalletUI;
    public UserInfoUI userInfoUI;
    public ShowNotificationUI showNotificationUI;
    public ShowNotificationCantOffUI showNotificationCantOffUI;

    public bool isHoverUI;

    public void ResponseConnectWallet(string responseString)
    {
        showNotificationCantOffUI.Hide();
        showNotificationUI.Hide();
        Debug.Log("ResponseConnectWallet:" + responseString);
        MessageBase<JObject> response = JsonConvert.DeserializeObject<MessageBase<JObject>>(responseString);
        if (!response.IsSuccess())
        {
            switch (response.level)
            {
                case MessageBase<JObject>.MessageEnum.WARNING:
                    Debug.Log("showNotificationUI");
                    showNotificationUI.SetUp(response.message);
                    showNotificationUI.Show();
                    break;
                case MessageBase<JObject>.MessageEnum.ERROR:
                    Debug.Log("showNotificationCantOffUI");
                    showNotificationCantOffUI.SetUp(response.message);
                    showNotificationCantOffUI.Show();
                    break;
            }

            return;
        }

        UserDTO userDto = response.data.ToObject<UserDTO>();
        Debug.Log("Address:" + userDto.address);
        DataManager.Instance.UserData = new UserData
        {
            Address = userDto.address,
            Balance = userDto.balance,
        };
        DataManager.Instance.MineCoin = userDto.balance;
        userInfoUI.Show();
        connectWalletUI.Hide();
    }
}