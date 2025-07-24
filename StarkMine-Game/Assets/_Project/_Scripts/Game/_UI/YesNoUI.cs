using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesNoUI : BasePopup
{
    public EventHandler OnYesButtonClickEventHandler;
    public EventHandler OnNoButtonClickEventHandler;

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private TextMeshProUGUI notificationText;
    private object _objectSend;

    public object ObjectSend => _objectSend;

    public void SetUp(string notification, object objectSend = null)
    {
        notificationText.text = notification;
        _objectSend = objectSend;
    }

    protected override void Start()
    {
        base.Start();

        yesButton.onClick.AddListener(OnYesButtonClick);
        noButton.onClick.AddListener(OnNoButtonClick);
        backgroundOverlayUI.OnClick += BackgroundOverlayUIOnOnClick;
    }

    private void BackgroundOverlayUIOnOnClick(object sender, EventArgs e)
    {
        OnNoButtonClickEventHandler?.Invoke(this, EventArgs.Empty);
    }

    private void OnYesButtonClick()
    {
        OnYesButtonClickEventHandler?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void OnNoButtonClick()
    {
        OnNoButtonClickEventHandler?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void OnDisable()
    {
        _objectSend = null;
    }
}