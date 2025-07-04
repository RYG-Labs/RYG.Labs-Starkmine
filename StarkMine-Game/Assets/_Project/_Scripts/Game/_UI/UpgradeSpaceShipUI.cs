using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSpaceShipUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject notificationText;
    [SerializeField] private Button visitingToHaveMineButton;
    [SerializeField] private TextMeshProUGUI mineRequirementText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Button upgradeButton;
    private ShipData _shipData;

    public void SetUp(ShipData shipData)
    {
        _shipData = shipData;
        descriptionText.text =
            $"From <color=#32DEAB>Lv{shipData.level}</color> to <color=#32DEAB>Lv{shipData.level + 1}</color> will increase <color=#FEE109>+{shipData.GetIncreasePowerForNextLevel()}</color> power";
        mineRequirementText.text = Helpers.FormatCurrencyNumber(shipData.GetCostForNextLevel());
    }

    protected override void Start()
    {
        base.Start();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        visitingToHaveMineButton.onClick.AddListener(OnVisitingToHaveMineButtonClick);
    }

    private void OnVisitingToHaveMineButtonClick()
    {
    }

    private void OnUpgradeButtonClick()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseUpgradeMinerEventHandler += WebResponseOnResponseUpgradeMinerEventHandler;
        WebResponse.Instance.OnResponseUpgradeMinerFailEventHandler +=
            WebResponseOnResponseUpgradeMinerFailEventHandler;
        WebRequest.CallRequestUpgradeMiner(_shipData.id);
    }

    private void WebResponseOnResponseUpgradeMinerFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseUpgradeMinerEventHandler -= WebResponseOnResponseUpgradeMinerEventHandler;
        WebResponse.Instance.OnResponseUpgradeMinerFailEventHandler -=
            WebResponseOnResponseUpgradeMinerFailEventHandler;
    }

    private void WebResponseOnResponseUpgradeMinerEventHandler(object sender,
        WebResponse.OnResponseUpgradeMinerEventArgs e)
    {
        DataManager.Instance.MineCoin -= _shipData.GetCostForNextLevel();
        _shipData.Upgrade();
        ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
        if (_shipData.IsMaxLevel())
        {
            showNotificationUI.SetUp("Max Level.");
            showNotificationUI.Show();
            Hide();
        }
        else
        {
            SoundManager.Instance.PlayCompleteSound2();
            showNotificationUI.SetUp("Upgrade Successful.");
            SetUp(_shipData);
            showNotificationUI.Show();
        }

        WebResponse.Instance.OnResponseUpgradeMinerEventHandler -= WebResponseOnResponseUpgradeMinerEventHandler;
        WebResponse.Instance.OnResponseUpgradeMinerFailEventHandler -=
            WebResponseOnResponseUpgradeMinerFailEventHandler;
    }

    private void OnEnable()
    {
        DataManager.Instance.OnMineCoinUpdate += DataManagerOnOnMineCoinUpdate;
        double mineCoins = DataManager.Instance.MineCoin;
        bool isEnoughCoin = mineCoins > _shipData.GetCostForNextLevel();
        balanceText.text = Helpers.FormatCurrencyNumber(mineCoins) + " $MINE";
        notificationText.SetActive(!isEnoughCoin);
        upgradeButton.interactable = isEnoughCoin;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnMineCoinUpdate -= DataManagerOnOnMineCoinUpdate;
    }

    private void DataManagerOnOnMineCoinUpdate(object sender, DataManager.OnMineCoinUpdateEventArgs e)
    {
        double mineCoins = e.NewMineCoin;
        bool isEnoughCoin = mineCoins > _shipData.GetCostForNextLevel();
        balanceText.text = Helpers.FormatCurrencyNumber(mineCoins) + " $MINE";
        notificationText.SetActive(!isEnoughCoin);
        upgradeButton.interactable = isEnoughCoin;
    }
}