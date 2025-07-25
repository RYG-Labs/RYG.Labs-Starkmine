using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Enemies;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ChooseCoreEngineUI : BasePopup
{
    public EventHandler<OnItemSelectEventArgs> OnItemSelectEventHandler;

    public class OnItemSelectEventArgs : EventArgs
    {
        public CoreEngineData CoreEngineData;
    }

    [SerializeField] private ItemChooseCoreEngineUI prefabItemChooseCoreEngineUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemChooseCoreEngineUI> listItem = new();
    [SerializeField] private TextMeshProUGUI coreEngineNameText;
    private ShipData _shipData;

    // private void OnEnable()
    // {
    //     SetUp(_shipData);
    // }

    public void SetUpAndShow(ShipData shipData)
    {
        SetUp(shipData);
        Show();
    }

    public void SetUp(ShipData shipData)
    {
        _shipData = shipData;
        coreEngineNameText.text = $"Core-Engine {shipData.shipSO.shipName} required";
        List<CoreEngineData> listCoreEngineData =
            DataManager.Instance.GetListCoreEngineUnActiveByShipType(shipData.shipSO.shipType);
        for (int i = 0; i < listCoreEngineData.Count; i++)
        {
            ItemChooseCoreEngineUI itemSpaceStationUI =
                Instantiate(prefabItemChooseCoreEngineUI, containerItem);
            itemSpaceStationUI.SetUp(i, listCoreEngineData[i], listCoreEngineData[i].GetDurabilityPercentage() > 0);
            itemSpaceStationUI.OnYesButtonClickHandler += ItemSpaceStationUIOnOnYesButtonClickHandler;
            listItem.Add(itemSpaceStationUI);
        }
    }

    private void ItemSpaceStationUIOnOnYesButtonClickHandler(object sender,
        ItemChooseCoreEngineUI.OnYesButtonClickHandlerEventArgs e)
    {
        SoundManager.Instance.PlayConfirmSound1();
        UIManager.Instance.loadingUI.Show();
        OnItemSelectEventHandler?.Invoke(this, new OnItemSelectEventArgs { CoreEngineData = e.CoreEngineData });
        Hide();
    }

    private void OnDisable()
    {
        Clear();
        OnItemSelectEventHandler = null;
    }

    public void Clear()
    {
        containerItem.DestroyChildren();
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