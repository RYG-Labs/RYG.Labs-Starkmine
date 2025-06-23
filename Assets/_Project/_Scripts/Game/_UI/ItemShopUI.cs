using System;
using _Project._Scripts.Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{
    public event EventHandler<OnBuyEventArgs> OnBuy;

    public class OnBuyEventArgs : EventArgs
    {
        public int index;
    }

    [SerializeField] Button buyButton;
    [SerializeField] Image shipImage;
    [SerializeField] TextMeshProUGUI shipPriceTextMeshPro;
    private int indexButton = -1;

    public void Setup(Sprite shipImageSprite, String shipPriceText, int index)
    {
        shipImage.sprite = shipImageSprite;
        shipPriceTextMeshPro.text = shipPriceText;
        indexButton = index;
    }

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    public void OnBuyButtonClicked()
    {
        OnBuy.Invoke(this, new OnBuyEventArgs() { index = indexButton });
    }
}