using _Project._Scripts.Game.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UserInfoUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI walletAddressText;
    [SerializeField] private TextMeshProUGUI mineCoinText;
    [SerializeField] private TextMeshProUGUI balanceText;

    protected override void Start()
    {
        base.Start();
        walletAddressText.text = DataManager.Instance.UserData.Address;
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
        balanceText.text = Helpers.FormatAddress(DataManager.Instance.UserData.Address) + " $MINE";
        DataManager.Instance.OnMineCoinUpdate += InstanceOnOnMineCoinUpdate;
        DataManager.Instance.OnUserDataChangedEventHandler += DataManagerOnUserDataChangedEventHandler;
    }

    private void DataManagerOnUserDataChangedEventHandler(object sender, DataManager.OnUserDataChangedEventArgs e)
    {
        walletAddressText.text = e.NewUserData.Address;
        balanceText.text = e.NewUserData.Balance + " $MINE";
    }

    private void InstanceOnOnMineCoinUpdate(object sender, DataManager.OnMineCoinUpdateEventArgs e)
    {
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
    }
}