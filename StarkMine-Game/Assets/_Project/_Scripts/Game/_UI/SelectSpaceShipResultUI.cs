using System;
using System.Collections.Generic;
using System.Globalization;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectSpaceShipResultUI : MonoBehaviour
{
    public event EventHandler<OnSelectSpaceShipResultEventArgs> OnSelectSpaceShipResultEventHandler;

    public class OnSelectSpaceShipResultEventArgs : EventArgs
    {
        public ShipSO ShipSo;
    }

    [SerializeField] private ItemSelectSpaceShipResultUI prefabItemSelectSpaceShipResultUI;
    [SerializeField] private Transform containerItem;
    [SerializeField] private List<ItemSelectSpaceShipResultUI> listItem = new();
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform informationShipGroup;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI hashPower;

    private ItemSelectSpaceShipResultUI _itemSelected;
    public ShipSO ShipSOSelected => _itemSelected == null ? null : _itemSelected.ShipSO;

    public void SetUp(List<ShipSO> listShipSo)
    {
        ClearChildren();
        for (int i = 0; i < listShipSo.Count; i++)
        {
            ItemSelectSpaceShipResultUI itemSelectSpaceShipResultUI =
                Instantiate(prefabItemSelectSpaceShipResultUI, containerItem);
            itemSelectSpaceShipResultUI.SetUp(i, listShipSo[i]);
            listItem.Add(itemSelectSpaceShipResultUI);
            itemSelectSpaceShipResultUI.OnClickItemEventHandler += ItemCreateCoreEngineUIOnOnClickItemEventHandler;
        }

        if (listShipSo.Count > 0)
        {
            listItem[0].Selected();
            _itemSelected = listItem[0];
        }

        informationShipGroup.gameObject.SetActive(listShipSo.Count != 0);
    }

    private void SetUpShipInformation(ShipData shipData)
    {
        nameText.text = ShipSOSelected.name;
        hashPower.text = ShipSOSelected.baseHashPower.ToString(CultureInfo.InvariantCulture);
    }

    // public void Refresh()
    // {
    //     ClearChildren();
    //     SetUp(new List<ShipSO>());
    // }

    private void ItemCreateCoreEngineUIOnOnClickItemEventHandler(object sender,
        ItemSelectSpaceShipResultUI.OnClickItemEventArgs e)
    {
        ItemSelectSpaceShipResultUI newItemSelected = sender as ItemSelectSpaceShipResultUI;

        if (_itemSelected != newItemSelected)
        {
            _itemSelected.UnSelected();
            _itemSelected = newItemSelected;
            nameText.text = newItemSelected.ShipSO.shipName;
            OnSelectSpaceShipResultEventHandler?.Invoke(this,
                new OnSelectSpaceShipResultEventArgs { ShipSo = newItemSelected.ShipSO });
        }
    }

    // private void OnDisable()
    // {
    //     ClearChildren();
    // }

    private void ClearChildren()
    {
        containerItem.DestroyChildren();
        listItem.Clear();
    }
}