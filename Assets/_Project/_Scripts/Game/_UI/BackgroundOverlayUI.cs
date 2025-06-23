using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundOverlayUI : MonoBehaviour, IPointerClickHandler
{
    public event EventHandler OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this, EventArgs.Empty);
    }
}