using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpaceSackUI : MonoBehaviour
{
    public event EventHandler<OnUseEventArgs> OnUse;

    public class OnUseEventArgs : EventArgs
    {
        public int index;
    }

    [SerializeField] Button useButton;
    [SerializeField] Image shipImage;
    public int indexButton = -1;

    public void Setup(Sprite shipImageSprite, int starLevel, int index)
    {
        shipImage.sprite = shipImageSprite;
        indexButton = index;
    }

    private void Start()
    {
        useButton.onClick.AddListener(OnUseButtonClicked);
    }

    public void OnUseButtonClicked()
    {
        OnUse.Invoke(this, new OnUseEventArgs() { index = indexButton });
    }
}