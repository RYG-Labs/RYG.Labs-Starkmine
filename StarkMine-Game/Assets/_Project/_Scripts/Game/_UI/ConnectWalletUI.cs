using System;
using System.Runtime.InteropServices;
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
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestDisconnectConnectWallet();
#endif
        UIManager.Instance.userInfoUI.Hide();
        Show();
    }

    public override void Show()
    {
        base.Show();
        disconnectWalletButton.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        base.Hide();
        disconnectWalletButton.gameObject.SetActive(true);
    }

    public void OnClickTestButton(string text)
    {
        UIManager.Instance.ResponseConnectWallet(text);
    }
}