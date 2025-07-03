using System;
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
        SoundManager.Instance.PlayClickSound();
        CoreEngineSO coreEngineSelected = DataManager.Instance.listCoreEngineSO[indexSelected];
        CoreEngineData coreEngineData =
            DataManager.Instance.GetCoreEngineRandomByType(coreEngineSelected.coreEngineType);
        if (coreEngineData == null)
        {
            ShowNotificationUI showNotificationUI = UIManager.Instance.showNotificationUI;
            showNotificationUI.SetUp("There is no longer Core Engine");
            showNotificationUI.Show();
            return;
        }

        UIManager.Instance.loadingUI.Show();

        WebResponse.Instance.OnResponseDefuseEngineEventHandler += WebResponseOnResponseDefuseEngineEventHandler;
        WebResponse.Instance.OnResponseDefuseEngineFailEventHandler +=
            WebResponseOnResponseDefuseEngineFailEventHandler;
        WebRequest.CallRequestDefuseEngine(coreEngineData.id);
    }

    private void WebResponseOnResponseDefuseEngineFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseDefuseEngineEventHandler += WebResponseOnResponseDefuseEngineEventHandler;
        WebResponse.Instance.OnResponseDefuseEngineFailEventHandler +=
            WebResponseOnResponseDefuseEngineFailEventHandler;
    }

    private void WebResponseOnResponseDefuseEngineEventHandler(object sender,
        WebResponse.OnResponseDefuseEngineEventArgs e)
    {
        CoreEngineSO coreEngineSelected = DataManager.Instance.listCoreEngineSO[indexSelected];
        CoreEngineData coreEngineData =
            DataManager.Instance.GetCoreEngineRandomByType(coreEngineSelected.coreEngineType);
        DataManager.Instance.RemoveCoreEngine(coreEngineData);

        SoundManager.Instance.PlayCompleteSound2();
        DataManager.Instance.MineCoin += coreEngineSelected.cost;
        Hide();

        WebResponse.Instance.OnResponseDefuseEngineEventHandler += WebResponseOnResponseDefuseEngineEventHandler;
        WebResponse.Instance.OnResponseDefuseEngineFailEventHandler +=
            WebResponseOnResponseDefuseEngineFailEventHandler;
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