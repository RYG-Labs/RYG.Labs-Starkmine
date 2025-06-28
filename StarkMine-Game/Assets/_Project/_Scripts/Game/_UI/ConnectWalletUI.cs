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
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestConnectWallet();
#endif
    }

    [DllImport("__Internal")]
    private static extern void RequestConnectWallet();

    [DllImport("__Internal")]
    private static extern void RequestDisconnectConnectWallet();

    public void OnClickDisconnectWalletButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
// #if UNITY_WEBGL && !UNITY_EDITOR
        // RequestDisconnectConnectWallet();
        DataManager.Instance.HandleDisconnectConnectWallet();
// #endif
        UIManager.Instance.userInfoUI.Hide();
        Show();
    }

    public override void Show()
    {
        base.Show();
        disconnectWalletButton.gameObject.SetActive(false);
        UIManager.Instance.userInfoUI.Hide();
        UIManager.Instance.tabPlanetUI.Hide();
    }

    public override void Hide()
    {
        base.Hide();
        disconnectWalletButton.gameObject.SetActive(true);
        UIManager.Instance.userInfoUI.Show();
        UIManager.Instance.tabPlanetUI.Show();
    }

    public void OnClickTestButton(string text)
    {
        UIManager.Instance.ResponseConnectWallet(text);
    }
}