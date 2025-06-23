using System;
using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using UnityEngine;

public class ChooseSpaceShipUI : BasePopup
{
    [SerializeField] private ItemChooseSpaceShipUI prefabItemChooseSpaceShipUI;
    [SerializeField] private Transform containerItemChooseSpaceShipUI;
    [SerializeField] private List<ItemChooseSpaceShipUI> listItem = new();
    public int spaceShipSelectedIndex = -1;

    private void OnEnable()
    {
        List<ShipData> listShipSO = DataManager.Instance.ShipInInventory;
        for (int i = 0; i < listShipSO.Count; i++)
        {
            ItemChooseSpaceShipUI itemSpaceStationUI =
                Instantiate(prefabItemChooseSpaceShipUI, containerItemChooseSpaceShipUI);
            itemSpaceStationUI.SetUp(i, listShipSO[i].shipSO.imageAnimationSO);
            itemSpaceStationUI.OnYesButtonClickHandler += ItemSpaceStationUIOnOnYesButtonClickHandler;
            listItem.Add(itemSpaceStationUI);
        }
    }

    private void ItemSpaceStationUIOnOnYesButtonClickHandler(object sender,
        ItemChooseSpaceShipUI.OnYesButtonClickHandlerEventArgs e)
    {
        List<ShipData> listShipData = DataManager.Instance.ShipInInventory;
        ShipData shipData = listShipData[e.ItemIndex];
        GameManager.Instance.AddShipToCurrentPlanet(shipData, spaceShipSelectedIndex);
        ItemChooseSpaceShipUI itemChooseSpaceShipUI = listItem[e.ItemIndex];
        listItem.Remove(itemChooseSpaceShipUI);
        if (itemChooseSpaceShipUI.gameObject != null)
        {
            Destroy(itemChooseSpaceShipUI.gameObject);
        }

        ResetIndex();
        Hide();
    }

    private void OnDisable()
    {
        containerItemChooseSpaceShipUI.DestroyChildren();
        listItem.Clear();
        spaceShipSelectedIndex = -1;
    }

    private void ResetIndex()
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].Index = i;
        }
    }
}