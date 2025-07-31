using System;
using System.Collections;
using _Project._Scripts.Game.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI walletAddressText;
    [SerializeField] private TextMeshProUGUI mineCoinText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI pendingHarvestText;
    [SerializeField] private TextMeshProUGUI monthlyPoolText;
    [SerializeField] private TextMeshProUGUI nextHalvingText;
    [SerializeField] private TextMeshProUGUI globalHashPowerText;
    [SerializeField] private TextMeshProUGUI yourTotalShareText;
    [SerializeField] private TextMeshProUGUI yourHashPowerText;
    [SerializeField] private TextMeshProUGUI suggestUpgradeStationText;
    [SerializeField] private TextMeshProUGUI currentStationMultiplierText;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button harvestButton;

    protected override void Start()
    {
        base.Start();
        walletAddressText.text = Helpers.FormatAddress(DataManager.Instance.UserData.Address);
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
        balanceText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
        pendingHarvestText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.PendingReward) + " $MINE";
        monthlyPoolText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MonthlyPool) + " $MINE";
        globalHashPowerText.text = Helpers.Round(DataManager.Instance.GlobalHashPower, 2).ToString();
        yourHashPowerText.text = Helpers.Round(DataManager.Instance.YourPower, 2).ToString();
        yourTotalShareText.text = CalculateYourTotalShare() + "%";
        nextHalvingText.text =
            $"{Helpers.AddSecondToUtcNow(DataManager.Instance.NextHavingSecond)} UTC ({Helpers.SecondToDay(DataManager.Instance.NextHavingSecond)} days)";

        StationData currentStationData = GameManager.Instance.CurrentStation;
        SetStationInfo(currentStationData);
        currentStationMultiplierText.text = $"x1.{currentStationData.level}";
        GameManager.Instance.OnChangeStationEventHandler += InstanceOnOnChangeStationEventHandler;

        DataManager.Instance.OnMineCoinUpdate += InstanceOnOnMineCoinUpdate;
        DataManager.Instance.OnUserDataChangedEventHandler += DataManagerOnUserDataChangedEventHandler;
        DataManager.Instance.OnPendingRewardChangeEventHandler += DataManagerOnPendingRewardChangeEventHandler;
        DataManager.Instance.OnGlobalHashPowerChangeEventHandler += DataManagerOnGlobalHashPowerChangeEventHandler;
        DataManager.Instance.OnYourPowerChangeEventHandler += InstanceOnOnYourPowerChangeEventHandler;

        harvestButton.onClick.AddListener(OnClickHarvestButton);
        refreshButton.onClick.AddListener(OnClickRefreshButton);
    }

    public void SetStationInfo(StationData stationData)
    {
        if (stationData.IsMaxLevel())
        {
            suggestUpgradeStationText.text =
                $"Current station is max level.";
        }
        else
        {
            suggestUpgradeStationText.text =
                $"Upgrade Station to <color=#32DEAB>Lv{stationData.level + 1}</color> required <color=#32DEAB>{stationData.GetCostForNextLevel()}</color> $MINE to have <color=#FEE109>x1.{stationData.level + 1}</color> multiplier";
        }
    }

    private void InstanceOnOnChangeStationEventHandler(object sender,
        GameManager.OnChangeStationEventHandlerEventArgs e)
    {
        StationData currentStationData = e.CurrentStation;
        SetStationInfo(currentStationData);
        currentStationMultiplierText.text = $"x1.{currentStationData.level}";
    }

    private void OnClickRefreshButton()
    {
        WebRequest.CallRequestTotalHashPower();
        WebRequest.CallRequestUserHashPower();
    }

    private void InstanceOnOnYourPowerChangeEventHandler(object sender, EventArgs e)
    {
        yourHashPowerText.text = Helpers.Round(DataManager.Instance.YourPower, 2).ToString();
        yourTotalShareText.text = CalculateYourTotalShare() + "%";
    }

    public string CalculateYourTotalShare()
    {
        if (DataManager.Instance.YourPower == 0 || DataManager.Instance.GlobalHashPower == 0) return "0";
        return Helpers.FormatCurrencyNumber(
            Helpers.Round(DataManager.Instance.YourPower / DataManager.Instance.GlobalHashPower, 2) * 100);
    }

    private void DataManagerOnGlobalHashPowerChangeEventHandler(object sender, EventArgs e)
    {
        globalHashPowerText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.GlobalHashPower);
        yourTotalShareText.text = CalculateYourTotalShare() + "%";
    }

    private void OnClickHarvestButton()
    {
        if (!DataManager.Instance.IsEnoughStreak())
        {
            UIManager.Instance.showNotificationUI.SetUpAndShow(
                $"You need to log in for <color=#FEE109>{DataManager.Instance.GetTimeLeftStreak()}</color> more consecutive days to be able to harvest your mine.");
            return;
        }

        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseClaimPendingRewardEventHandler +=
            InstanceOnOnResponseClaimPendingRewardEventHandler;
        WebResponse.Instance.OnResponseClaimPendingRewardFailEventHandler +=
            WebResponseOnResponseClaimPendingRewardFailEventHandler;
        WebRequest.CallRequestClaimPendingReward();
    }

    private void WebResponseOnResponseClaimPendingRewardFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseClaimPendingRewardEventHandler +=
            InstanceOnOnResponseClaimPendingRewardEventHandler;
        WebResponse.Instance.OnResponseClaimPendingRewardFailEventHandler +=
            WebResponseOnResponseClaimPendingRewardFailEventHandler;
    }

    private void InstanceOnOnResponseClaimPendingRewardEventHandler(object sender,
        WebResponse.OnResponseClaimPendingRewardEventArgs e)
    {
        DataManager.Instance.MineCoin += DataManager.Instance.PendingReward;
        DataManager.Instance.PendingReward = 0;
        WebResponse.Instance.OnResponseClaimPendingRewardEventHandler +=
            InstanceOnOnResponseClaimPendingRewardEventHandler;
        WebResponse.Instance.OnResponseClaimPendingRewardFailEventHandler +=
            WebResponseOnResponseClaimPendingRewardFailEventHandler;
    }

    private void DataManagerOnPendingRewardChangeEventHandler(object sender,
        DataManager.OnPendingRewardChangeEventArgs e)
    {
        pendingHarvestText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.PendingReward) + " $MINE";
    }

    private void DataManagerOnUserDataChangedEventHandler(object sender, DataManager.OnUserDataChangedEventArgs e)
    {
        walletAddressText.text = Helpers.FormatAddress(e.NewUserData.Address);
        balanceText.text = Helpers.FormatCurrencyNumber(e.NewUserData.Balance) + " $MINE";
        balanceText.text = Helpers.FormatCurrencyNumber(e.NewUserData.Balance) + " $MINE";
    }

    private void InstanceOnOnMineCoinUpdate(object sender, DataManager.OnMineCoinUpdateEventArgs e)
    {
        mineCoinText.text = Helpers.FormatCurrencyNumber(e.NewMineCoin) + " $MINE";
        balanceText.text = Helpers.FormatCurrencyNumber(e.NewMineCoin) + " $MINE";
    }

    public void UpdateSuggestStation(StationData stationData)
    {
        SetStationInfo(stationData);
        currentStationMultiplierText.text = $"x1.{stationData.level}";
    }
}