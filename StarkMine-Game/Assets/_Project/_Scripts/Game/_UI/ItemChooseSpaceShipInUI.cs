using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemChooseSpaceShipInUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<OnClickItemEventArgs> OnClickItemEventHandler;

    public class OnClickItemEventArgs : EventArgs
    {
        public int Index;
        public ShipData ShipData;
    }

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Image shipImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private StarLevelUI starLevelUI;
    [SerializeField] private TextMeshProUGUI successRateText;
    private ShipData _shipData;

    public ShipData ShipData
    {
        get { return _shipData; }
        set { _shipData = value; }
    }

    private bool _isSelected;

    public void SetUp(ShipData shipData)
    {
        _shipData = shipData;
        shipImage.sprite = shipData.shipSO.baseSprite;
        starLevelUI.SetUp(shipData.level);
        successRateText.text = $"+{shipData.level}% Success Rate";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ToggleSelect();
        SoundManager.Instance.PlayClickSound4();
        OnClickItemEventHandler?.Invoke(this, new OnClickItemEventArgs
        {
            ShipData = _shipData
        });
    }

    public void ToggleSelect()
    {
        if (_isSelected)
        {
            borderImage.gameObject.SetActive(true);
            borderImage.color = hoverColor;
            _isSelected = false;
        }
        else
        {
            borderImage.color = selectedColor;
            _isSelected = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected) return;
        SoundManager.Instance.PlayDataPointSound3();
        borderImage.gameObject.SetActive(true);
        borderImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected)
        {
            borderImage.gameObject.SetActive(true);
            borderImage.color = selectedColor;
        }
        else
        {
            borderImage.gameObject.SetActive(false);
        }
    }
}