using System;
using System.Collections.Generic;
using System.Linq;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;

public class SelectSpaceShipInUI : MonoBehaviour
{
    public event EventHandler<OnSelectSpaceShipEventArgs> OnSelectSpaceShipEventHandler;

    public class OnSelectSpaceShipEventArgs : EventArgs
    {
        public List<ShipData> ListShipSelected;
    }

    public event EventHandler OnUnSelectSpaceShipEventHandler;

    [SerializeField] private ItemChooseSpaceShipInUI prefabItemChooseSpaceShipInUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemChooseSpaceShipInUI> listItem = new();
    [SerializeField] private List<ItemChooseSpaceShipInUI> listItemSelected = new();
    [SerializeField] private Transform noSelectedText;
    [SerializeField] private TextMeshProUGUI totalSuccessRateText;
    private int _totalSuccessRate;

    public int TotalSuccessRate
    {
        get => _totalSuccessRate;
        set
        {
            _totalSuccessRate = value;
            // totalSuccessRateText.text = _totalSuccessRate + "%";
        }
    }

    public void SetUp(List<ShipData> listShipShow)
    {
        TotalSuccessRate = 0;
        ClearChildren();
        foreach (ShipData shipData in listShipShow)
        {
            InstanceNewItem(shipData);
        }

        noSelectedText.gameObject.SetActive(listShipShow.Count == 0);
    }

    // private void OnEnable()
    // {
    //     DataManager.Instance.OnAddShipToInventoryEventHandler += DataManagerOnAddShipToInventoryEventHandler;
    //     DataManager.Instance.OnRemoveShipToInventoryEventHandler += DataManagerOnRemoveShipToInventoryEventHandler;
    // }
    //
    // private void OnDisable()
    // {
    //     ClearChildren();
    //     DataManager.Instance.OnAddShipToInventoryEventHandler -= DataManagerOnAddShipToInventoryEventHandler;
    //     DataManager.Instance.OnRemoveShipToInventoryEventHandler -= DataManagerOnRemoveShipToInventoryEventHandler;
    // }

    // private void DataManagerOnRemoveShipToInventoryEventHandler(object sender,
    //     DataManager.OnRemoveShipToInventoryEventArgs e)
    // {
    //     SetUp(e.Ships.FindAll(shipData =>
    //         shipData.shipSO.shipType != ShipSO.ShipType.Giga));
    // }

    private void DestroyItem(ItemChooseSpaceShipInUI item)
    {
        listItem.Remove(item);
        Destroy(item.gameObject);
    }

    // private void DataManagerOnAddShipToInventoryEventHandler(object sender, DataManager.OnAddShipToInventoryEventArgs e)
    // {
    //     if (e.NewShip.shipSO.shipType != ShipSO.ShipType.Giga)
    //     {
    //         InstanceNewItem(e.NewShip);
    //     }
    // }

    private void InstanceNewItem(ShipData shipData)
    {
        ItemChooseSpaceShipInUI itemSelectSpaceShipResultUI =
            Instantiate(prefabItemChooseSpaceShipInUI, containerItem);
        itemSelectSpaceShipResultUI.SetUp(shipData);
        listItem.Add(itemSelectSpaceShipResultUI);
        itemSelectSpaceShipResultUI.OnClickItemEventHandler += ItemCreateCoreEngineUIOnOnClickItemEventHandler;
    }

    public List<ShipData> DestroyItemSelected()
    {
        List<ShipData> shipDataList = new();
        foreach (ItemChooseSpaceShipInUI item in listItemSelected)
        {
            shipDataList.Add(item.ShipData);
            Destroy(item.gameObject);
        }

        Debug.Log("===============" + shipDataList.Count);
        listItemSelected.Clear();
        return shipDataList;
    }

    public List<ShipData> GetShipSelected()
    {
        List<ShipData> shipDataList = new();
        foreach (ItemChooseSpaceShipInUI item in listItemSelected)
        {
            shipDataList.Add(item.ShipData);
        }

        return shipDataList;
    }

    private void ItemCreateCoreEngineUIOnOnClickItemEventHandler(object sender,
        ItemChooseSpaceShipInUI.OnClickItemEventArgs e)
    {
        ItemChooseSpaceShipInUI itemChooseSpaceShipInUI = sender as ItemChooseSpaceShipInUI;
        if (itemChooseSpaceShipInUI == null) return;

        if (listItemSelected.Contains(itemChooseSpaceShipInUI))
        {
            listItemSelected.Remove(itemChooseSpaceShipInUI);
            itemChooseSpaceShipInUI.ToggleSelect();
        }
        else
        {
            if (listItemSelected.Count == 2) return;
            listItemSelected.Add(itemChooseSpaceShipInUI);
            itemChooseSpaceShipInUI.ToggleSelect();
        }

        if (listItemSelected.Count == 2)
        {
            OnSelectSpaceShipEventHandler?.Invoke(this, new OnSelectSpaceShipEventArgs()
            {
                ListShipSelected = listItemSelected.Select(item => item.ShipData).ToList()
            });
        }
        else
        {
            OnUnSelectSpaceShipEventHandler?.Invoke(this, EventArgs.Empty);
        }

        TotalSuccessRate = CalculateTotalSuccessRate();
    }

    public int CalculateTotalSuccessRate()
    {
        int total = 0;
        foreach (ItemChooseSpaceShipInUI itemSelected in listItemSelected)
        {
            total += itemSelected.ShipData.level;
        }

        return total;
    }

    public void ClearChildren()
    {
        containerItem.DestroyChildren();
        listItem.Clear();
        listItemSelected.Clear();
    }

    private void ClearChildrenByShipSO(ShipSO shipSo)
    {
        foreach (ItemChooseSpaceShipInUI itemChooseSpaceShipInUI in listItem.ToList())
        {
            if (itemChooseSpaceShipInUI.ShipData.shipSO != shipSo)
            {
                Destroy(itemChooseSpaceShipInUI.gameObject);
                listItem.Remove(itemChooseSpaceShipInUI);
            }
        }
    }
}