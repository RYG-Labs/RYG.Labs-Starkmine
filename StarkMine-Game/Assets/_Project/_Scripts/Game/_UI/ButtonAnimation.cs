using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private ButtonAnimationSO beforeButtonAnimationSO;
    [SerializeField] private ButtonAnimationSO afterButtonAnimationSO;

    [SerializeField] private Image borderImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image icon;

    [Header("Animation Settings")] [SerializeField]
    private float colorTransitionDuration = 0.1f; // Thời gian chuyển đổi màu

    [SerializeField] private float scaleTransitionDuration = 0.1f; // Thời gian chuyển đổi scale
    [SerializeField] private float fillTransitionDuration = 0.2f; // Thời gian chuyển đổi fill amount
    [SerializeField] private float clickScaleFactor = 0.9f; // Tỷ lệ scale khi nhấn giữ
    [SerializeField] private float clickDuration = 0.1f; // Thời gian animation khi nhấn/thả
    [SerializeField] private float clickFillAmount = 1f; // Fill amount khi nhấn giữ (mặc định đầy)

    private Vector3 originalScale; // Lưu scale ban đầu của button
    private float originalFillAmount; // Lưu fill amount ban đầu của fillImage
    private Button button;

    private void Start()
    {
        // Lưu scale và fill amount ban đầu
        originalScale = transform.localScale;
        originalFillAmount = fillImage != null ? fillImage.fillAmount : 0f;

        // Áp dụng trạng thái ban đầu từ beforeButtonAnimationSO
        ApplyButtonState(beforeButtonAnimationSO, useAnimation: true);
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        ApplyButtonState(beforeButtonAnimationSO, useAnimation: true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        // Áp dụng trạng thái hover từ afterButtonAnimationSO
        ApplyButtonState(afterButtonAnimationSO, useAnimation: true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Quay lại trạng thái ban đầu từ beforeButtonAnimationSO
        ApplyButtonState(beforeButtonAnimationSO, useAnimation: true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        // Animation khi nhấn giữ: thu nhỏ và thay đổi fill amount
        transform.DOScale(originalScale * clickScaleFactor, clickDuration).SetEase(Ease.InOutSine);

        if (fillImage != null)
        {
            fillImage.DOFillAmount(clickFillAmount, clickDuration).SetEase(Ease.InOutSine);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Animation khi thả chuột: trở lại scale và fill amount ban đầu
        transform.DOScale(originalScale, clickDuration).SetEase(Ease.InOutSine);

        if (fillImage != null)
        {
            fillImage.DOFillAmount(originalFillAmount, clickDuration).SetEase(Ease.InOutSine);
        }
    }

    private void ApplyButtonState(ButtonAnimationSO state, bool useAnimation)
    {
        if (useAnimation)
        {
            // Cập nhật màu sắc với animation mượt mà
            if (borderImage != null)
            {
                borderImage.DOColor(state.borderColor, colorTransitionDuration).SetEase(Ease.OutSine);
            }

            if (backgroundImage != null)
            {
                backgroundImage.DOColor(state.backgroundColor, colorTransitionDuration).SetEase(Ease.OutSine);
            }

            if (fillImage != null)
            {
                fillImage.DOColor(state.fillColor, colorTransitionDuration).SetEase(Ease.OutSine);
                fillImage.DOFillAmount(state.fillAmount, fillTransitionDuration).SetEase(Ease.OutSine);
            }

            if (text != null)
            {
                text.DOColor(state.textColor, colorTransitionDuration).SetEase(Ease.OutSine);
            }

            if (icon != null)
            {
                icon.DOColor(state.textColor, colorTransitionDuration).SetEase(Ease.OutSine);
            }

            // Cập nhật scale với animation mượt mà
            transform.DOScale(state.scale, scaleTransitionDuration).SetEase(Ease.OutSine);
        }
        else
        {
            // Cập nhật trạng thái ngay lập tức (dùng trong Editor)
            if (borderImage != null)
            {
                borderImage.color = state.borderColor;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = state.backgroundColor;
            }

            if (fillImage != null)
            {
                fillImage.color = state.fillColor;
                fillImage.fillAmount = state.fillAmount;
            }

            if (text != null)
            {
                text.color = state.textColor;
            }

            if (icon != null)
            {
                icon.color = state.textColor;
            }

            transform.localScale = state.scale;
        }
    }

    [ContextMenu("Apply Before State in Editor")]
    private void ApplyBeforeStateInEditor()
    {
        ApplyButtonState(beforeButtonAnimationSO, useAnimation: false);
    }

    [ContextMenu("Apply After State in Editor")]
    private void ApplyAfterStateInEditor()
    {
        ApplyButtonState(afterButtonAnimationSO, useAnimation: false);
    }

    private void OnValidate()
    {
        // Kiểm tra và cảnh báo nếu thiếu tham chiếu
        // if (beforeButtonAnimationSO == null)
        //     Debug.LogWarning("beforeButtonAnimationSO is not assigned!", this);
        // if (afterButtonAnimationSO == null)
        //     Debug.LogWarning("afterButtonAnimationSO is not assigned!", this);
        // if (borderImage == null)
        //     Debug.LogWarning("borderImage is not assigned!", this);
        // if (backgroundImage == null)
        //     Debug.LogWarning("backgroundImage is not assigned!", this);
        // if (fillImage == null)
        //     Debug.LogWarning("fillImage is not assigned!", this);
        // if (text == null)
        //     Debug.LogWarning("text is not assigned!", this);
        // Kiểm tra xem fillImage có phải là Filled Image không
        if (fillImage != null && fillImage.type != Image.Type.Filled)
        {
            Debug.LogWarning("fillImage must have Image Type set to Filled!", this);
        }

        // Áp dụng trạng thái ban đầu trong Editor để xem trước
        if (!Application.isPlaying && beforeButtonAnimationSO != null)
        {
            ApplyButtonState(beforeButtonAnimationSO, useAnimation: false);
        }
    }
}