using System;
using System.Runtime.InteropServices;
using _Project._Scripts.Game.Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ConnectWalletUI : BasePopup
{
    [SerializeField] private Button connectWalletButton;
    [SerializeField] private Button disconnectWalletButton;

    protected override void Start()
    {
        base.Start();
        connectWalletButton.onClick.AddListener(OnClickConnectWalletButton);
        // disconnectWalletButton.onClick.AddListener(OnClickDisconnectWalletButton);
    }

    private void OnClickConnectWalletButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        WebRequest.CallRequestConnectWallet();
    }

    public void OnClickDisconnectWalletButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        WebRequest.CallRequestDisconnectWallet();
        UIManager.Instance.userInfoUI.Hide();
        UIManager.Instance.spaceStationUI.Hide();
        UIManager.Instance.loadingUI.Hide();
        UIManager.Instance.initStationUI.Hide();
        DataManager.Instance.UserData = null;
        Show();
        // WebResponse.Instance.ResetLoadData();
    }

    public override void Show()
    {
        base.Show();
        disconnectWalletButton.gameObject.SetActive(false);
        UIManager.Instance.userInfoUI.Hide();
        // UIManager.Instance.tabPlanetUI.Hide();
    }

    public override void Hide()
    {
        base.Hide();
        disconnectWalletButton.gameObject.SetActive(true);
        UIManager.Instance.userInfoUI.Show();
        // UIManager.Instance.tabPlanetUI.Show();
    }

    public void Test()
    {
        WebRequest.CallRequestAssignMinerToStation(1, 2, 3);
    }
}