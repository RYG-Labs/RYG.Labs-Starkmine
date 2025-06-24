using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefuseCoreEngineUI : BasePopup
{
    [SerializeField] private ItemCreateCoreEngineUI prefabItemCreateCoreEngineUI;
    [SerializeField] private Transform containerItemCreateCoreEngineUI;
    [SerializeField] private List<ItemCreateCoreEngineUI> listItem = new();
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI applyForText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button defuseButton;
    [SerializeField] private int indexSelected = 0;

    protected override void Start()
    {
        base.Start();
        defuseButton.onClick.AddListener(OnDefuseButtonButtonClick);
    }

    // public void SetUp(CoreEngineSO coreEngineRequire)
    // {
    //     indexSelected = DataManager.Instance.listCoreEngineSO.IndexOf(coreEngineRequire);
    // }

    private void OnDefuseButtonButtonClick()
    {
        CoreEngineSO coreEngineSelected = DataManager.Instance.listCoreEngineSO[indexSelected];

        bool success = DataManager.Instance.RemoveCoreEngine(coreEngineSelected, 1);
        if (!success)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("There is no longer Core Engine");
            showNotificationUI.Show();
            return;
        }
        SoundManager.Instance.PlayCompleteSound2();
        DataManager.Instance.MineCoin += coreEngineSelected.cost;
        Hide();
    }

    private void OnEnable()
    {
        List<CoreEngineSO> listCoreEngineSo = DataManager.Instance.listCoreEngineSO;
        for (int i = 0; i < listCoreEngineSo.Count; i++)
        {
            ItemCreateCoreEngineUI itemCreateCoreEngineUI =
                Instantiate(prefabItemCreateCoreEngineUI, containerItemCreateCoreEngineUI);
            itemCreateCoreEngineUI.SetUp(i, listCoreEngineSo[i]);
            listItem.Add(itemCreateCoreEngineUI);
            itemCreateCoreEngineUI.OnClickItemEventHandler += ItemCreateCoreEngineUIOnOnClickItemEventHandler;
        }

        indexSelected = 0;
        listItem[indexSelected].Selected();
        CoreEngineSO coreEngineSO = DataManager.Instance.listCoreEngineSO[indexSelected];
        nameText.text = coreEngineSO.nameCoreEngine;
        applyForText.text = $"{coreEngineSO.coreEngineType.ToString()} Spaceship";
        costText.text = $"{coreEngineSO.cost.ToString()} $MINE";
    }

    private void ItemCreateCoreEngineUIOnOnClickItemEventHandler(object sender,
        ItemCreateCoreEngineUI.OnClickItemEventArgs e)
    {
        if (e.Index != indexSelected)
        {
            listItem[indexSelected].UnSelected();
        }

        indexSelected = e.Index;
        CoreEngineSO coreEngineSO = e.CoreEngineSO;
        nameText.text = coreEngineSO.nameCoreEngine;
        applyForText.text = $"{coreEngineSO.coreEngineType.ToString()} Spaceship";
        costText.text = $"{coreEngineSO.cost.ToString()} $MINE";
    }

    private void OnDisable()
    {
        containerItemCreateCoreEngineUI.DestroyChildren();
        listItem.Clear();
    }
}