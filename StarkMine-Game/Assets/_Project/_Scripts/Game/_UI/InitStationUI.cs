using UnityEngine;
using UnityEngine.UI;

public class InitStationUI : BasePopup
{
    [SerializeField] private Button initStationButton;

    protected override void Start()
    {
        base.Start();
        initStationButton.onClick.AddListener(OnClickInitStationButton);
    }

    private void OnClickInitStationButton()
    {
        UIManager.Instance.loadingUI.Show();
        WebRequest.CallRequestInitStation();
    }
}