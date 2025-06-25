using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemSelectSpaceShipResultUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    public event EventHandler<OnClickItemEventArgs> OnClickItemEventHandler;

    public class OnClickItemEventArgs : EventArgs
    {
        public int Index;
        public ShipSO ShipSO;
    }

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Image shipImage;
    [SerializeField] private Image borderImage;
    private ShipSO _shipSO;

    public ShipSO ShipSO
    {
        get => _shipSO;
        set => _shipSO = value;
    }

    private bool _isSelected;
    private int _index;

    public void SetUp(int index, ShipSO shipSo)
    {
        _index = index;
        _shipSO = shipSo;
        shipImage.sprite = shipSo.baseSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Selected();
        SoundManager.Instance.PlayClickSound4();
        OnClickItemEventHandler?.Invoke(this, new OnClickItemEventArgs
        {
            Index = _index,
            ShipSO = _shipSO
        });
    }

    public void Selected()
    {
        borderImage.color = selectedColor;
        _isSelected = true;
    }

    public void UnSelected()
    {
        borderImage.color = unSelectedColor;
        _isSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isSelected) return;
        SoundManager.Instance.PlayDataPointSound3();
        borderImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            borderImage.color = unSelectedColor;
            return;
        }

        borderImage.color = selectedColor;
    }
}