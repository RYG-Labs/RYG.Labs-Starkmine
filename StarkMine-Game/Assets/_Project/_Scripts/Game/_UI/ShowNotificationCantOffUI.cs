using TMPro;
using UnityEngine;

public class ShowNotificationCantOffUI : BasePopup
{
    [SerializeField] private TextMeshProUGUI notificationText;

    public void SetUp(string notification)
    {
        notificationText.text = notification;
    }
}
