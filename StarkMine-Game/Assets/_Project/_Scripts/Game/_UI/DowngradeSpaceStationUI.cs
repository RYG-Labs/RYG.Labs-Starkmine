using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class DowngradeSpaceStationUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI mineRequirementText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Button downgradeButton;
    [SerializeField] private TMP_InputField targetLevelTextField;
    private StationData _stationData;

    public void SetUp(StationData stationData)
    {
        _stationData = stationData;
        descriptionText.text =
            $"From <color=#FF59C2>Lv{stationData.level}</color> to <color=#FF59C2>Lv{stationData.level - 1}</color> will decrease multiplier to <color=#FF59C2>x{stationData.GetDecreaseMultiplierForPrevLevel()}</color>";
        targetLevelTextField.text = 0.ToString();
        mineRequirementText.text = Helpers.FormatCurrencyNumber(_stationData.GetReceivedDownLevel(0));
    }

    protected override void Start()
    {
        base.Start();
        downgradeButton.onClick.AddListener(OnDowngradeButtonClick);
        targetLevelTextField.onEndEdit.AddListener(OnTargetLevelTextFieldEnd);
    }

    private void OnTargetLevelTextFieldEnd(string arg0)
    {
        int targetLevel = int.Parse(targetLevelTextField.text);
        if (targetLevel >= _stationData.level || targetLevel < 0)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp(
                "The entered level must be greater than 0 and less than the current level of the station.");
            showNotificationUI.Show();
            return;
        }

        mineRequirementText.text = Helpers.FormatCurrencyNumber(_stationData.GetReceivedDownLevel(targetLevel));
        Debug.Log($"Target level text: {_stationData.GetReceivedDownLevel(targetLevel)}");
    }


    private void OnDowngradeButtonClick()
    {
        int targetLevel = int.Parse(targetLevelTextField.text);
        if (targetLevel >= _stationData.level || targetLevel < 0)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp(
                "The entered level must be greater than 0 and less than the current level of the station.");
            showNotificationUI.Show();
            return;
        }

        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.SetUp(
            $"Are you sure you want to downgrade to level {targetLevel} and received {_stationData.GetReceivedDownLevel(targetLevel)} $MINE?");
        yesNoUI.Show();
        yesNoUI.OnYesButtonClickEventHandler += YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += NoButtonClickEventHandler;
    }

    private void NoButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;
    }

    private void YesButtonClickEventHandler(object sender, EventArgs e)
    {
        int targetLevel = int.Parse(targetLevelTextField.text);
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseRequestDowngradeStationEventHandler +=
            WebResponseOnResponseRequestDowngradeStationEventHandler;
        WebResponse.Instance.OnResponseRequestDowngradeStationFailEventHandler +=
            WebResponseOnResponseRequestDowngradeStationFailEventHandler;
        WebRequest.CallRequestRequestDowngradeStation(_stationData.id, targetLevel);
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;
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
        // _stationData.Downgrade();
        _stationData.pendingDownGrade = _stationData.level;
        _stationData.pendingMineTime = e.Data.estimateSeconds;
        _stationData.level = e.Data.targetLevel;
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

        Hide();
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