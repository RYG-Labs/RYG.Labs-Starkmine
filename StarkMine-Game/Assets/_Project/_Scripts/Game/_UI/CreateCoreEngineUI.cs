using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCoreEngineUI : BasePopup
{
    [SerializeField] private ItemCreateCoreEngineUI prefabItemCreateCoreEngineUI;
    [SerializeField] private Transform containerItemCreateCoreEngineUI;
    [SerializeField] private List<ItemCreateCoreEngineUI> listItem = new();
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI applyForText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button createButton;
    [SerializeField] private int indexSelected = 0;

    protected override void Start()
    {
        base.Start();
        createButton.onClick.AddListener(OnCreateButtonClick);
    }

    public void SetUp(CoreEngineSO coreEngineRequire)
    {
        if (coreEngineRequire == null)
        {
            indexSelected = 0;
            return;
        }

        indexSelected = DataManager.Instance.listCoreEngineSO.IndexOf(coreEngineRequire);
    }

    private void OnCreateButtonClick()
    {
        SoundManager.Instance.PlayClickSound();
        UIManager.Instance.loadingUI.Show();
        CoreEngineSO coreEngineSelected = DataManager.Instance.listCoreEngineSO[indexSelected];
        WebResponse.Instance.OnResponseMintCoreEngineEventHandler += WebResponseOnResponseMintCoreEngineEventHandler;
        WebResponse.Instance.OnResponseMintCoreEngineFailEventHandler +=
            WebResponseOnResponseMintCoreEngineFailEventHandler;
        WebRequest.CallRequestMintCoreEngine(coreEngineSelected.nameCoreEngine);
    }

    private void WebResponseOnResponseMintCoreEngineFailEventHandler(object sender, EventArgs e)
    {
        WebResponse.Instance.OnResponseMintCoreEngineEventHandler -= WebResponseOnResponseMintCoreEngineEventHandler;
        WebResponse.Instance.OnResponseMintCoreEngineFailEventHandler -=
            WebResponseOnResponseMintCoreEngineFailEventHandler;
    }

    private void WebResponseOnResponseMintCoreEngineEventHandler(object sender,
        WebResponse.OnResponseMintCoreEngineEventArgs e)
    {
        SoundManager.Instance.PlayCompleteSound2();
        CoreEngineSO coreEngineSelected = DataManager.Instance.listCoreEngineSO[indexSelected];
        CoreEngineData coreEngineData =
            new CoreEngineData(e.Data.coreEngineId, coreEngineSelected, false);
        DataManager.Instance.MineCoin -= coreEngineSelected.cost;
        DataManager.Instance.CreateCoreEngine(coreEngineData);
        Hide();

        WebResponse.Instance.OnResponseMintCoreEngineEventHandler -= WebResponseOnResponseMintCoreEngineEventHandler;
        WebResponse.Instance.OnResponseMintCoreEngineFailEventHandler -=
            WebResponseOnResponseMintCoreEngineFailEventHandler;
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