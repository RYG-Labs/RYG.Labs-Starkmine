using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using UnityEngine;

public class ShopUI : BasePopup
{
    [SerializeField] private List<ShipSO> ships;
    [SerializeField] private ItemShopUI itemShopUIPrefab;
    [SerializeField] private Transform itemContainer;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < ships.Count; i++)
        {
            ShipSO shipSO = ships[i];
            ItemShopUI itemShopUI = Instantiate(itemShopUIPrefab, itemContainer);
            itemShopUI.Setup(shipSO.baseSprite, shipSO.shipCost.ToString(), i);
            itemShopUI.OnBuy += ItemShopUIPrefabOnOnBuy;
        }
    }

    private void ItemShopUIPrefabOnOnBuy(object sender, ItemShopUI.OnBuyEventArgs e)
    {
        ShipSO shipSO = ships[e.index];
        DataManager.Instance.ShipInInventory.Add(new ShipData(shipSO));
        DataManager.Instance.MineCoin -= shipSO.shipCost;
    }
}