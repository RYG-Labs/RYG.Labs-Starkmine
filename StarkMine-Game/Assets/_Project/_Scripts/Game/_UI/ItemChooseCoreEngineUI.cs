using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemChooseCoreEngineUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler<OnYesButtonClickHandlerEventArgs> OnYesButtonClickHandler;

    public class OnYesButtonClickHandlerEventArgs : EventArgs
    {
        public CoreEngineData CoreEngineData;
    }

    public event EventHandler<OnNoButtonClickHandlerEventArgs> OnNoButtonClickHandler;

    public class OnNoButtonClickHandlerEventArgs : EventArgs
    {
        public CoreEngineData CoreEngineData;
    }

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Transform hoverGroup;
    [SerializeField] private Image shipImage;
    [SerializeField] private TextMeshProUGUI durabilityText;
    public int Index { get; set; } = 0;
    private CoreEngineData _coreEngineData;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClick);
        noButton.onClick.AddListener(OnNoButtonClick);
        hoverGroup.gameObject.SetActive(false);
    }


    public void SetUp(int index, CoreEngineData coreEngineData, bool interactableYesButton)
    {
        Index = index;
        _coreEngineData = coreEngineData;
        shipImage.gameObject.SetActive(true);
        shipImage.sprite = coreEngineData.coreEngineSO.sprite;
        durabilityText.text = "Durability: " + Mathf.CeilToInt(coreEngineData.GetDurabilityPercentage() * 100) + "%";
        yesButton.interactable = interactableYesButton;
    }

    private void OnNoButtonClick()
    {
        OnNoButtonClickHandler?.Invoke(this, new OnNoButtonClickHandlerEventArgs()
        {
            CoreEngineData = _coreEngineData
        });
    }

    private void OnYesButtonClick()
    {
        OnYesButtonClickHandler?.Invoke(this, new OnYesButtonClickHandlerEventArgs()
        {
            CoreEngineData = _coreEngineData
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