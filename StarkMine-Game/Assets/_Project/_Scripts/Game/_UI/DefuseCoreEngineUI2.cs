using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DefuseCoreEngineUI2 : BasePopup
{
    [SerializeField] private ItemChooseCoreEngineUI prefabItemChooseSpaceShipUI;
    [SerializeField] private Transform containerItemChooseSpaceShipUI;
    [SerializeField] private List<ItemChooseCoreEngineUI> listItem = new();
    [SerializeField] private FilterCoreEngineUI filterCoreEngineUI;
    [SerializeField] private TextMeshProUGUI countBasicText;
    [SerializeField] private TextMeshProUGUI countEliteText;
    [SerializeField] private TextMeshProUGUI countProText;
    [SerializeField] private TextMeshProUGUI countGIGAText;
    public int coreEngineSelectedIndex = -1;

    protected override void Start()
    {
        base.Start();
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(filterCoreEngineUI.ListToggleIndexSelected));
    }

    private void OnEnable()
    {
        countBasicText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Basic, false)
                .ToString();
        countEliteText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Elite, false)
                .ToString();
        countProText.text = DataManager.Instance
            .CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.Pro, false).ToString();
        countGIGAText.text =
            DataManager.Instance.CountAllCoreEngineInInventoryByType(CoreEngineSO.CoreEngineType.GIGA, false)
                .ToString();
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(filterCoreEngineUI.ListToggleIndexSelected));
        filterCoreEngineUI.OnOptionFilterChangeEventHandler += FilterCoreEngineUIOnOnOptionFilterChangeEventHandler;
    }

    public void SetUp(List<CoreEngineData> listCoreEngineData)
    {
        for (int i = 0; i < listCoreEngineData.Count; i++)
        {
            ItemChooseCoreEngineUI itemChooseCoreEngineUI =
                Instantiate(prefabItemChooseSpaceShipUI, containerItemChooseSpaceShipUI);
            itemChooseCoreEngineUI.SetUp(i, listCoreEngineData[i], true);
            itemChooseCoreEngineUI.OnYesButtonClickHandler += ItemChooseCoreEngineUIOnOnYesButtonClickHandler;
            listItem.Add(itemChooseCoreEngineUI);
        }
    }

    private void FilterCoreEngineUIOnOnOptionFilterChangeEventHandler(object sender,
        FilterCoreEngineUI.OnOptionFilterChangeEventArgs e)
    {
        Clear();
        SetUp(DataManager.Instance.GetCoreEngineDataUnActiveByListType(e.ListToggleSelected));
    }

    private void ItemChooseCoreEngineUIOnOnYesButtonClickHandler(object sender,
        ItemChooseCoreEngineUI.OnYesButtonClickHandlerEventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.SetUp("Are you sure you want to defuse this core engine and receive <color=#FEE109>" +
                      e.CoreEngineData.GetMineReceiveEstimate() + "</color>?", e.CoreEngineData);
        yesNoUI.Show();
        yesNoUI.OnYesButtonClickEventHandler += YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler += NoButtonClickEventHandler;
    }

    private void NoButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;
    }

    private void YesButtonClickEventHandler(object sender, EventArgs e)
    {
        YesNoUI yesNoUI = UIManager.Instance.yesNoUI;
        yesNoUI.OnYesButtonClickEventHandler -= YesButtonClickEventHandler;
        yesNoUI.OnNoButtonClickEventHandler -= NoButtonClickEventHandler;

        SoundManager.Instance.PlayConfirmSound1();
        UIManager.Instance.loadingUI.Show();
        //todo: call request defuse 
        WebResponse.Instance.OnResponseDefuseEngineEventHandler += WebResponseOnResponseDefuseEngineEventHandler;
        WebResponse.Instance.OnResponseDefuseEngineFailEventHandler +=
            WebResponseOnResponseDefuseEngineFailEventHandler;
        WebRequest.CallRequestDefuseEngine(((CoreEngineData)yesNoUI.ObjectSend).id);
        Hide();
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
        CoreEngineData coreEngineData = DataManager.Instance.GetCoreEngineDataById(e.Data.engineId);
        DataManager.Instance.MineCoin += coreEngineData.GetMineReceiveEstimate();
        DataManager.Instance.RemoveCoreEngine(coreEngineData);
        SoundManager.Instance.PlayCompleteSound2();
        Hide();

        WebResponse.Instance.OnResponseDefuseEngineEventHandler += WebResponseOnResponseDefuseEngineEventHandler;
        WebResponse.Instance.OnResponseDefuseEngineFailEventHandler +=
            WebResponseOnResponseDefuseEngineFailEventHandler;
    }

    private void OnDisable()
    {
        Clear();
        coreEngineSelectedIndex = -1;
        filterCoreEngineUI.OnOptionFilterChangeEventHandler -= FilterCoreEngineUIOnOnOptionFilterChangeEventHandler;
    }

    public void Clear()
    {
        containerItemChooseSpaceShipUI.DestroyChildren();
        listItem.Clear();
    }

    private void ResetIndex()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].Index = i;
        }
    }
}