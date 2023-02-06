using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class UIBaseController : MonoBehaviour, IUIPanel
{
    protected Canvas canvas;
    protected GraphicRaycaster raycaster;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
    }

    public bool IsShow()
    {
        return canvas.enabled;
    }

    public virtual void Show()
    {
        canvas.enabled = true;
        raycaster.enabled = true;
    }

    public virtual void Hide()
    {
        canvas.enabled = false;
        raycaster.enabled = false;
    }

    public virtual void SetSortOrder(int _sortOrder)
    {
        canvas.sortingOrder = _sortOrder;
    }
}
