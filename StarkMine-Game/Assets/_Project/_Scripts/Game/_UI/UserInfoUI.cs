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
    [SerializeField] private Button harvestButton;

    protected override void Start()
    {
        base.Start();
        walletAddressText.text = Helpers.FormatAddress(DataManager.Instance.UserData.Address);
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
        balanceText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
        pendingHarvestText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.PendingReward) + " $MINE";
        DataManager.Instance.OnMineCoinUpdate += InstanceOnOnMineCoinUpdate;
        DataManager.Instance.OnUserDataChangedEventHandler += DataManagerOnUserDataChangedEventHandler;
        DataManager.Instance.OnPendingRewardChangeEventHandler += DataManagerOnPendingRewardChangeEventHandler;
        harvestButton.onClick.AddListener(OnClickHarvestButton);
    }

    private void OnClickHarvestButton()
    {
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
}