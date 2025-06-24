using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemChooseSpaceShipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<OnYesButtonClickHandlerEventArgs> OnYesButtonClickHandler;

    public class OnYesButtonClickHandlerEventArgs : EventArgs
    {
        public int ItemIndex;
    }

    public event EventHandler<OnNoButtonClickHandlerEventArgs> OnNoButtonClickHandler;

    public class OnNoButtonClickHandlerEventArgs : EventArgs
    {
        public int ItemIndex;
    }

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Transform hoverGroup;
    [SerializeField] private Image shipImage;
    [SerializeField] private ImageAnimation imageAnimation;
    public int Index { get; set; } = 0;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClick);
        noButton.onClick.AddListener(OnNoButtonClick);
    }


    public void SetUp(int index, ImageAnimationSO shipAnimationSO)
    {
        Index = index;
        shipImage.gameObject.SetActive(true);
        shipImage.sprite = shipAnimationSO.sprites[0];
        shipImage.SetNativeSize();
        imageAnimation.ImageAnimationSO = shipAnimationSO;
    }

    private void OnNoButtonClick()
    {
        OnNoButtonClickHandler?.Invoke(this, new OnNoButtonClickHandlerEventArgs()
        {
            ItemIndex = Index
        });
    }

    private void OnYesButtonClick()
    {
        OnYesButtonClickHandler?.Invoke(this, new OnYesButtonClickHandlerEventArgs()
        {
            ItemIndex = Index
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlayDataPointSound3();
        hoverGroup.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverGroup.gameObject.SetActive(false);
    }
}