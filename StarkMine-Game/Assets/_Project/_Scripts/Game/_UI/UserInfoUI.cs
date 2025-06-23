using _Project._Scripts.Game.Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UserInfoUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI walletAddressText;
    [SerializeField] private TextMeshProUGUI mineCoinText;


    protected override void Start()
    {
        base.Start();
        walletAddressText.text = DataManager.Instance.WalletAddress;
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";

        DataManager.Instance.OnMineCoinUpdate += InstanceOnOnMineCoinUpdate;
    }

    private void InstanceOnOnMineCoinUpdate(object sender, DataManager.OnMineCoinUpdateEventArgs e)
    {
        mineCoinText.text = Helpers.FormatCurrencyNumber(DataManager.Instance.MineCoin) + " $MINE";
    }
}