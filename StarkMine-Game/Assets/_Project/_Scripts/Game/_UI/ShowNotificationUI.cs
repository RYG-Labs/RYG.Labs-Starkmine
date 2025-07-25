using TMPro;
using UnityEngine;

public class ShowNotificationUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI notificationText;

    public void SetUp(string notification)
    {
        notificationText.text = notification;
    }

    public void SetUpAndShow(string notification)
    {
        notificationText.text = notification;
        Show();
    }
}