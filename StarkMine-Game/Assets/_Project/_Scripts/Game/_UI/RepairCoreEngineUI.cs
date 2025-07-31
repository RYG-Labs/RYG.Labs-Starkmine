using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairCoreEngineUI : BasePopup
{
    [SerializeField] private TMP_InputField durabilityInputField;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private TextMeshProUGUI mineRequirmentText;
    [SerializeField] private Button repairButton;
    private CoreEngineData _coreEngineData;
    private int _min;
    private int _max;

    public void SetupAndShow(CoreEngineData coreEngineData)
    {
        SetUp(coreEngineData);
        Show();
    }

    public void SetUp(CoreEngineData coreEngineData)
    {
        mineRequirmentText.text = "0";
        durabilityInputField.text = "0";
        _coreEngineData = coreEngineData;
        durabilityText.text = $"Fill with gas ({1}-{_coreEngineData.blockUsed})";
    }

    protected override void Start()
    {
        base.Start();

        durabilityInputField.onEndEdit.AddListener(OnEndEditDurability);
        repairButton.onClick.AddListener(OnClickRepairButton);
    }

    private void OnClickRepairButton()
    {
        int durabilityToRestore = int.Parse(durabilityInputField.text);
        if (!_coreEngineData.CanRepair(durabilityToRestore))
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp(
                $"The entered must be greater than 0 and less than {_coreEngineData.blockUsed}.");
            showNotificationUI.Show();
            return;
        }

        mineRequirmentText.text =
            Helpers.FormatCurrencyNumber(_coreEngineData.GetMineRequirmentDurability(durabilityToRestore));
        UIManager.Instance.loadingUI.Show();
        WebResponse.Instance.OnResponseRepairCoreEngineEventHandler +=
            WebResponseOnResponseRepairCoreEngineEventHandler;
        WebResponse.Instance.OnResponseRepairCoreEngineFailEventHandler +=
            InstanceOnOnResponseRepairCoreEngineFailEventHandler;
        WebRequest.CallRequestRepairCoreEngine(_coreEngineData.id, durabilityToRestore);
    }

    private void InstanceOnOnResponseRepairCoreEngineFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseRepairCoreEngineEventHandler -=
            WebResponseOnResponseRepairCoreEngineEventHandler;
        WebResponse.Instance.OnResponseRepairCoreEngineFailEventHandler -=
            InstanceOnOnResponseRepairCoreEngineFailEventHandler;
    }

    private void WebResponseOnResponseRepairCoreEngineEventHandler(object sender,
        WebResponse.OnResponseRepairCoreEngineEventArgs e)
    {
        int durabilityToRestore = int.Parse(durabilityInputField.text);
        DataManager.Instance.MineCoin -= _coreEngineData.GetMineRequirmentDurability(durabilityToRestore);
        _coreEngineData.blockUsed -= durabilityToRestore;

        WebResponse.Instance.OnResponseRepairCoreEngineEventHandler -=
            WebResponseOnResponseRepairCoreEngineEventHandler;
        WebResponse.Instance.OnResponseRepairCoreEngineFailEventHandler -=
            InstanceOnOnResponseRepairCoreEngineFailEventHandler;
        Hide();
    }

    private void OnEndEditDurability(string arg0)
    {
        int targetDurability = int.Parse(durabilityInputField.text);
        if (!_coreEngineData.CanRepair(targetDurability))
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp(
                $"The entered must be greater than 0 and less than {_coreEngineData.blockUsed}.");
            showNotificationUI.Show();
            return;
        }

        mineRequirmentText.text = _coreEngineData.GetMineRequirmentDurability(targetDurability).ToString();
    }
}