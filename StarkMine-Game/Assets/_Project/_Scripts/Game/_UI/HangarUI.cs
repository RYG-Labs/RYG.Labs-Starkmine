using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HangarUI : BasePopup
{
    [SerializeField] private ItemHangarUI prefabItemHangarUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemHangarUI> listItem;
    [SerializeField] private Button buySpaceShipButton;
    [SerializeField] private Button buySpaceShipLargeButton;
    [SerializeField] private Button mergeSpaceShipButton;
    [SerializeField] private Button createCoreEngineButton;
    [SerializeField] private Button repairCoreEngineButton;
    [SerializeField] private Button defuseCoreEngineButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private FilterSpaceShipUI filterSpaceShipUI;
    [SerializeField] private TextMeshProUGUI countBasicShipText;
    [SerializeField] private TextMeshProUGUI countEliteShipText;
    [SerializeField] private TextMeshProUGUI countProShipText;
    [SerializeField] private TextMeshProUGUI countGIGAShipText;
    [SerializeField] private int size = 10;
    private int _page = 0;

    public void SetUp(List<ShipData> shipDatas)
    {
        buySpaceShipLargeButton.gameObject.SetActive(shipDatas.Count == 0);
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
        buySpaceShipLargeButton.onClick.AddListener(BuySpaceShipButtonOnClick);
        mergeSpaceShipButton.onClick.AddListener(MergeSpaceShipButtonOnClick);
        createCoreEngineButton.onClick.AddListener(CreateCoreEngineButtonOnClick);
        repairCoreEngineButton.onClick.AddListener(OnClickRepairCoreEngineButton);
        defuseCoreEngineButton.onClick.AddListener(DefuseCoreEngineButtonOnClick);
        nextPageButton.onClick.AddListener(OnClickNextPageButton);
        prevPageButton.onClick.AddListener(OnClickPrevPageButton);
        filterSpaceShipUI.OnOptionFilterChangeEventHandler += FilterSpaceShipUIOnOnOptionFilterChangeEventHandler;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, 1, 0,
            isAll: true, isAllType: true));
    }

    private void OnClickRepairCoreEngineButton()
    {
        UIManager.Instance.chooseCoreEngineToRepairUI.Show();
    }

    private void OnClickPrevPageButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
        if (_page == 1) return;
        _page--;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, _page, size));
    }

    private void OnClickNextPageButton()
    {
        SoundManager.Instance.PlayConfirmSound3();
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
        countBasicShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Basic).ToString();
        countEliteShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Elite).ToString();
        countProShipText.text = DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.Pro).ToString();
        countGIGAShipText.text =
            DataManager.Instance.CountAllSpaceShipInInventoryByType(ShipSO.ShipType.GIGA).ToString();
        _page = 1;
        SetUp(DataManager.Instance.ShipDataInInventoryFilter(filterSpaceShipUI.ListToggleIndexSelected, _page,
            size));
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
        Application.OpenURL("https://hyperflex.market/");
    }

    public void ClearChildren()
    {
        containerItem.DestroyChildren();
        listItem.Clear();
    }
}