using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpaceSackUI : BasePopup
{
    [SerializeField] private ItemSpaceSackUI itemShopUIPrefab;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private List<ItemSpaceSackUI> listItem = new();

    private void OnEnable()
    {
        itemContainer.DestroyChildren();
        listItem.Clear();
        List<ShipData> ships = DataManager.Instance.ShipInInventory;
        for (int i = 0; i < ships.Count; i++)
        {
            ShipData shipData = ships[i];
            ItemSpaceSackUI itemSpaceSackUI = Instantiate(itemShopUIPrefab, itemContainer);
            itemSpaceSackUI.Setup(shipData.shipSO.baseSprite, 5, i);
            itemSpaceSackUI.OnUse += ItemShopUIOnOnUse;
            listItem.Add(itemSpaceSackUI);
        }
    }

    private void ItemShopUIOnOnUse(object sender, ItemSpaceSackUI.OnUseEventArgs e)
    {
        List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
        // GameManager.Instance.AddShipToCurrentPlanet(listShipData[e.index]);
        listShipData.Remove(listShipData[e.index]);
        ItemSpaceSackUI itemSpaceSackUI = listItem[e.index];
        itemSpaceSackUI.OnUse -= ItemShopUIOnOnUse;
        listItem.Remove(itemSpaceSackUI);
        Destroy(itemSpaceSackUI.gameObject);
        ResetIndex();
    }

    private void ResetIndex()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].indexButton = i;
        }
    }
}