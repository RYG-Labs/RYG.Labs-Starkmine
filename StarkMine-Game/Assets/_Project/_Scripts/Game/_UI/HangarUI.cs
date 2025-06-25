using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HangarUI : BasePopup
{
    [SerializeField] private ItemHangarUI prefabItemHangarUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemHangarUI> listItem;
    [SerializeField] private Button buySpaceShipButton;
    [SerializeField] private Button mergeSpaceShipButton;
    [SerializeField] private Button createCoreEngineButton;
    [SerializeField] private Button defuseCoreEngineButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private FilterSpaceShipUI filterSpaceShipUI;
    [SerializeField] private int size = 10;
    private int _page = 0;

    public void SetUp(List<ShipData> shipDatas)
    {
        ClearChildren();
        for (int i = 0; i < 10; i++)
        {
            ItemHangarUI itemHangarUI = Instantiate(prefabItemHangarUI, containerItem);
            itemHangarUI.SetUp(i < shipDatas.Count ? shipDatas[i].shipSO.imageAnimationSO : null);
            listItem.Add(itemHangarUI);
        }
    }

    protected override void Start()
    {
        base.Start();
        buySpaceShipButton.onClick.AddListener(BuySpaceShipButtonOnClick);
        mergeSpaceShipButton.onClick.AddListener(MergeSpaceShipButtonOnClick);
        createCoreEngineButton.onClick.AddListener(CreateCoreEngineButtonOnClick);
        defuseCoreEngineButton.onClick.AddListener(DefuseCoreEngineButtonOnClick);
        nextPageButton.onClick.AddListener(OnClickNextPageButton);
        prevPageButton.onClick.AddListener(OnClickPrevPageButton);
        filterSpaceShipUI.OnOptionFilterChangeEventHandler += FilterSpaceShipUIOnOnOptionFilterChangeEventHandler;
    }

    private void OnClickPrevPageButton()
    {
        if (_page == 1) return;
        _page--;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, _page, size));
    }

    private void OnClickNextPageButton()
    {
        List<ShipData> result =
            DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, _page + 1, size);
        if (result.Count == 0) return;
        _page++;
        SetUp(result);
    }

    private void FilterSpaceShipUIOnOnOptionFilterChangeEventHandler(object sender,
        FilterSpaceShipUI.OnOptionFilterChangeEventArgs e)
    {
        _page = 1;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(e.ListToggleSelected, _page, size));
    }


    private void OnEnable()
    {
        _page = 1;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, _page,
            size, isAllType: true));
        DataManager.Instance.OnAddShipToInventoryEventHandler += DataManagerOnAddShipToInventoryEventHandler;
        DataManager.Instance.OnRemoveShipToInventoryEventHandler += DataManagerOnRemoveShipToInventoryEventHandler;
    }

    private void DataManagerOnRemoveShipToInventoryEventHandler(object sender,
        DataManager.OnRemoveShipToInventoryEventArgs e)
    {
        SetUp(e.Ships);
    }

    private void DataManagerOnAddShipToInventoryEventHandler(object sender, DataManager.OnAddShipToInventoryEventArgs e)
    {
        SetUp(e.Ships);
    }

    private void OnDisable()
    {
        containerItem.DestroyChildren();
    }

    private void DefuseCoreEngineButtonOnClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        UIManager.Instance.defuseCoreEngineUI.Show();
    }

    private void CreateCoreEngineButtonOnClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        CreateCoreEngineUI createCoreEngineUI = UIManager.Instance.createCoreEngineUI;
        createCoreEngineUI.SetUp(null);
        createCoreEngineUI.Show();
    }

    private void MergeSpaceShipButtonOnClick()
    {
        SoundManager.Instance.PlayConfirmSound3();
        UIManager.Instance.mergeSpaceshipUI.Show();
    }

    private void BuySpaceShipButtonOnClick()
    {
    }

    public void ClearChildren()
    {
        containerItem.DestroyChildren();
        listItem.Clear();
    }
}