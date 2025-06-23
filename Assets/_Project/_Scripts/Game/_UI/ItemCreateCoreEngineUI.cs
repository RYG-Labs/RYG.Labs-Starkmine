using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCreateCoreEngineUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<OnClickItemEventArgs> OnClickItemEventHandler;

    public class OnClickItemEventArgs : EventArgs
    {
        public int Index;
        public CoreEngineSO CoreEngineSO;
    }

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Image coreEngineImage;
    [SerializeField] private Image borderImage;
    private CoreEngineSO _coreEngineSO;
    private bool _isSelected;
    private int _index;

    public void SetUp(int index, CoreEngineSO coreEngineSO)
    {
        _index = index;
        _coreEngineSO = coreEngineSO;
        coreEngineImage.sprite = coreEngineSO.sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Selected();
        OnClickItemEventHandler?.Invoke(this, new OnClickItemEventArgs
        {
            Index = _index,
            CoreEngineSO = _coreEngineSO
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