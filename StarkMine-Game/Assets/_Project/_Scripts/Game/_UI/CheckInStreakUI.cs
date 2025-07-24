using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckInStreakUI : BasePopup
{
    [SerializeField] private Button checkInButton;
    [SerializeField] private TextMeshProUGUI notificationText;

    protected override void Start()
    {
        base.Start();
        checkInButton.onClick.AddListener(OnClickCheckInButton);
    }

    private void OnClickCheckInButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseRecordLoginEventHandler += WebResponseOnResponseRecordLoginEventHandler;
        WebResponse.Instance.OnResponseRecordLoginFailEventHandler += WebResponseOnResponseRecordLoginFailEventHandler;

        WebRequest.CallRequestRecordLogin();
    }

    private void WebResponseOnResponseRecordLoginFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseRecordLoginEventHandler -= WebResponseOnResponseRecordLoginEventHandler;
        WebResponse.Instance.OnResponseRecordLoginFailEventHandler -= WebResponseOnResponseRecordLoginFailEventHandler;
    }

    private void WebResponseOnResponseRecordLoginEventHandler(object sender,
        WebResponse.OnResponseRecordLoginEventArgs e)
    {
        DataManager.Instance.Streak++;
        DataManager.Instance.RemainingTimeToRecordLogin = 24 * 60 * 60;
        DataManager.Instance.StartCountDownRemainingTimeToRecordLoginCoroutine();
        Hide();
        WebResponse.Instance.OnResponseRecordLoginEventHandler -= WebResponseOnResponseRecordLoginEventHandler;
        WebResponse.Instance.OnResponseRecordLoginFailEventHandler -= WebResponseOnResponseRecordLoginFailEventHandler;
    }

    private void OnEnable()
    {
        int streak = DataManager.Instance.Streak;
        int streakToClaim = DataManager.Instance.StreakToClaimReward;

        if (streak == streakToClaim - 1)
        {
            notificationText.text =
                "This is your last check-in opportunity, check in now to start receiving your mine.";
        }
        else
        {
            notificationText.text =
                $"Your current streak is <color=#FEE109>{streak}</color>, check in for <color=#FEE109>{streakToClaim}</color> consecutive days to start receiving your mine.";
        }
    }
}