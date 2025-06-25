using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemChooseSpaceShipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<OnYesButtonClickHandlerEventArgs> OnYesButtonClickHandler;

    public class OnYesButtonClickHandlerEventArgs : EventArgs
    {
        public ShipData ShipData;
    }

    public event EventHandler<OnNoButtonClickHandlerEventArgs> OnNoButtonClickHandler;

    public class OnNoButtonClickHandlerEventArgs : EventArgs
    {
        public ShipData ShipData;
    }

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Transform hoverGroup;
    [SerializeField] private Image shipImage;
    [SerializeField] private ImageAnimation imageAnimation;
    public int Index { get; set; } = 0;
    private ShipData _shipData;
    private void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClick);
        noButton.onClick.AddListener(OnNoButtonClick);
    }


    public void SetUp(int index, ShipData shipData)
    {
        Index = index;
        _shipData = shipData;
        shipImage.gameObject.SetActive(true);
        shipImage.sprite = shipData.shipSO.imageAnimationSO.sprites[0];
        shipImage.SetNativeSize();
        imageAnimation.ImageAnimationSO = shipData.shipSO.imageAnimationSO;
    }

    private void OnNoButtonClick()
    {
        OnNoButtonClickHandler?.Invoke(this, new OnNoButtonClickHandlerEventArgs()
        {
            ShipData = _shipData
        });
    }

    private void OnYesButtonClick()
    {
        OnYesButtonClickHandler?.Invoke(this, new OnYesButtonClickHandlerEventArgs()
        {
            ShipData = _shipData
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