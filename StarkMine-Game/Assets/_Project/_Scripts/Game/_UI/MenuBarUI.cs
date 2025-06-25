using UnityEngine;
using UnityEngine.UI;

public class MenuBarUI : BasePopup
{
    [SerializeField] private Button explorationButton;
    [SerializeField] private Button referralButton;
    [SerializeField] private Button hangarButton;

    protected override void Start()
    {
        base.Start();
        hangarButton.onClick.AddListener(OnHangarButtonClick);
    }

    private void OnHangarButtonClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        UIManager.Instance.hangarUI.Show();
    }
}