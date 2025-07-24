using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PendingMineUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI pendingTimeText;
    [SerializeField] private TextMeshProUGUI pendingValueText;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button cancelButton;
    private StationData _stationData;

    public void SetUp(StationData stationData)
    {
        _stationData = stationData;
        pendingTimeText.text = Helpers.TimeFormater(stationData.pendingMineTime);
        pendingValueText.text = stationData.GetPendingMineValue() + " $Mine";
        bool canClaim = stationData.pendingMineTime == 0;
        claimButton.gameObject.SetActive(canClaim);
        cancelButton.gameObject.SetActive(!canClaim);
    }

    private void OnEnable()
    {
        DataManager.Instance.OnCountDowngradeStationChangeEventHandler += CountDowngradeStationChangeEventHandler;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnCountDowngradeStationChangeEventHandler -= CountDowngradeStationChangeEventHandler;
    }

    private void CountDowngradeStationChangeEventHandler(object sender, EventArgs e)
    {
        SetUp(_stationData);
    }

    public void SetUpAndShow(StationData stationData)
    {
        SetUp(stationData);
        Show();
    }

    protected override void Start()
    {
        base.Start();
        claimButton.onClick.AddListener(OnClickClaimButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    private void OnClickCancelButton()
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.SetUp("Are you sure you want to cancel?");
        yesNoUI.OnYesButtonClickEventHandler += YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += NoButtonClickEventHandler;
        yesNoUI.Show();
    }

    private void NoButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;
    }

    private void YesButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;
    }

    private void OnClickClaimButton()
    {
        ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
        showNotificationUI.SetUpAndShow("Claim Successfully.");
        DataManager.Instance.MineCoin += _stationData.GetPendingMineValue();
    }
}