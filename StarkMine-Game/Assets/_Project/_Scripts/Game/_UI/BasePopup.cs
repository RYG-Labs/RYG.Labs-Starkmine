using System;
using _Project._Scripts.Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasePopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button btnBack;
    [SerializeField] protected BackgroundOverlayUI backgroundOverlayUI;
    protected bool isShowing;
    // private PopupCallBack callback;
    //
    // public delegate void PopupCallBack();

    protected virtual void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        btnBack?.onClick.AddListener(Hide);
        if (backgroundOverlayUI != null)
        {
            backgroundOverlayUI.OnClick += BackgroundOverlayUIOnOnClick;
        }
    }

    private void BackgroundOverlayUIOnOnClick(object sender, EventArgs e)
    {
        Hide();
    }

    public virtual void Show()
    {
        if (isShowing)
        {
            return;
        }

        gameObject.SetActive(true);

        isShowing = true;
    }

    public virtual void Hide()
    {
        SoundManager.Instance.PlayBleepSound7();
        gameObject.SetActive(false);
        isShowing = false;
        UIManager.Instance.isHoverUI = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.isHoverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.isHoverUI = false;
    }
}