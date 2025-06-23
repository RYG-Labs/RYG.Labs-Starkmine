using UnityEngine;
using UnityEngine.UI;

public class DontHaveRequireCoreEngineUI : BasePopup
{
    [SerializeField] private Button createCoreEngineButton;
    private CoreEngineSO _coreEngineRequire;

    protected override void Start()
    {
        base.Start();
        createCoreEngineButton.onClick.AddListener(CreateCoreEngineButtonClick);
    }

    public void SetUp(CoreEngineSO coreEngineRequire)
    {
        _coreEngineRequire = coreEngineRequire;
    }

    private void CreateCoreEngineButtonClick()
    {
        CreateCoreEngineUI createCoreEngineUI = UIManager.Instance.createCoreEngineUI;
        createCoreEngineUI.SetUp(_coreEngineRequire);
        createCoreEngineUI.Show();
        Hide();
    }
}