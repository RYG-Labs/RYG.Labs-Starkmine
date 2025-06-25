using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSpaceStationUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject notificationText;
    [SerializeField] private Button visitingToHaveMineButton;
    [SerializeField] private TextMeshProUGUI mineRequirementText;
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Button upgradeButton;
    private StationData _stationData;

    public void SetUp(StationData stationData)
    {
        _stationData = stationData;
        descriptionText.text =
            $"From <color=#32DEAB>Lv{stationData.level}</color> to <color=#32DEAB>Lv{stationData.level + 1}</color> will increase <color=#FEE109>x{stationData.GetIncreaseMultiplierForNextLevel()}</color> power";
        mineRequirementText.text = Helpers.FormatCurrencyNumber(_stationData.GetCostForNextLevel());
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
        DataManager.Instance.MineCoin -= _stationData.GetCostForNextLevel();
        _stationData.Upgrade();
        SpaceStationUI spaceStationUI = UIManager.Instance.spaceStationUI;
        spaceStationUI.RefreshStationInformation(_stationData);
        ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
        SoundManager.Instance.PlayCompleteSound2();
        if (_stationData.IsMaxLevel())
        {
            showNotificationUI.SetUp("Max Level.");
            showNotificationUI.Show();
            Hide();
        }
        else
        {
            showNotificationUI.SetUp("Upgrade Successful.");
            SetUp(_stationData);
            showNotificationUI.Show();
        }
    }

    private void OnEnable()
    {
        DataManager.Instance.OnMineCoinUpdate += DataManagerOnOnMineCoinUpdate;
        double mineCoins = DataManager.Instance.MineCoin;
        bool isEnoughCoin = mineCoins > _stationData.GetCostForNextLevel();
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
        bool isEnoughCoin = mineCoins > _stationData.GetCostForNextLevel();
        balanceText.text = Helpers.FormatCurrencyNumber(mineCoins) + " $MINE";
        notificationText.SetActive(!isEnoughCoin);
        upgradeButton.interactable = isEnoughCoin;
    }
}