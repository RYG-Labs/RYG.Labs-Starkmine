using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DowngradeSpaceStationUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI mineRequirementText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Button downgradeButton;
    private StationData _stationData;

    public void SetUp(StationData stationData)
    {
        _stationData = stationData;
        descriptionText.text =
            $"From <color=#FF59C2>Lv{stationData.level}</color> to <color=#FF59C2>Lv{stationData.level - 1}</color> will decrease multiplier to <color=#FF59C2>x{stationData.GetDecreaseMultiplierForPrevLevel()}</color>";
        mineRequirementText.text = Helpers.FormatCurrencyNumber(_stationData.GetCostForPrevLevel());
    }

    protected override void Start()
    {
        base.Start();
        downgradeButton.onClick.AddListener(OnDowngradeButtonClick);
    }

    private void OnDowngradeButtonClick()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseRequestDowngradeStationEventHandler +=
            WebResponseOnResponseRequestDowngradeStationEventHandler;
        WebResponse.Instance.OnResponseRequestDowngradeStationFailEventHandler +=
            WebResponseOnResponseRequestDowngradeStationFailEventHandler;
        WebRequest.CallRequestRequestDowngradeStation(_stationData.id, _stationData.level - 1);
    }

    private void WebResponseOnResponseRequestDowngradeStationFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseRequestDowngradeStationEventHandler -=
            WebResponseOnResponseRequestDowngradeStationEventHandler;
        WebResponse.Instance.OnResponseRequestDowngradeStationFailEventHandler -=
            WebResponseOnResponseRequestDowngradeStationFailEventHandler;
    }

    private void WebResponseOnResponseRequestDowngradeStationEventHandler(object sender,
        WebResponse.OnResponseRequestDowngradeStationEventArgs e)
    {
        // DataManager.Instance.MineCoin += _stationData.GetCostForPrevLevel();
        _stationData.Downgrade();
        downgradeButton.interactable = !_stationData.IsMinLevel();
        SpaceStationUI spaceStationUI = UIManager.Instance.spaceStationUI;
        spaceStationUI.RefreshStationInformation(_stationData);
        ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
        SoundManager.Instance.PlayCompleteSound2();
        if (_stationData.IsMinLevel())
        {
            showNotificationUI.SetUp("Min Level.");
            showNotificationUI.Show();
            Hide();
        }
        else
        {
            showNotificationUI.SetUp("Downgrade Successful.");
            SetUp(_stationData);
            showNotificationUI.Show();
        }

        WebResponse.Instance.OnResponseRequestDowngradeStationEventHandler -=
            WebResponseOnResponseRequestDowngradeStationEventHandler;
        WebResponse.Instance.OnResponseRequestDowngradeStationFailEventHandler -=
            WebResponseOnResponseRequestDowngradeStationFailEventHandler;
    }

    private void OnEnable()
    {
        DataManager.Instance.OnMineCoinUpdate += DataManagerOnOnMineCoinUpdate;
        double mineCoins = DataManager.Instance.MineCoin;
        balanceText.text = Helpers.FormatCurrencyNumber(mineCoins) + " $MINE";
        downgradeButton.interactable = !_stationData.IsMinLevel();
    }

    private void OnDisable()
    {
        DataManager.Instance.OnMineCoinUpdate -= DataManagerOnOnMineCoinUpdate;
    }

    private void DataManagerOnOnMineCoinUpdate(object sender, DataManager.OnMineCoinUpdateEventArgs e)
    {
        double mineCoins = e.NewMineCoin;
        balanceText.text = Helpers.FormatCurrencyNumber(mineCoins) + " $MINE";
        downgradeButton.interactable = !_stationData.IsMinLevel();
    }
}