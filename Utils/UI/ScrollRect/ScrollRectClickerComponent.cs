using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ScrollRectClickerComponent : MonoBehaviour
{
    [Serializable]
    public class ButtonEventType : UnityEvent { }
    public ButtonEventType onClick = new ();
    
    public Action<PointerEventData> onScrollRectBeginDrag, onScrollRectEndDrag, onScrollRectClick;

    public virtual void OnScrollRectBeginDrag(PointerEventData data)
    {
        onScrollRectBeginDrag?.Invoke(data);
    }

    public virtual void OnScrollRectEndDrag(PointerEventData data)
    {
        onScrollRectEndDrag?.Invoke(data);
    }

    public virtual void OnScrollRectClick(PointerEventData data)
    {
        onScrollRectClick?.Invoke(data);
        onClick?.Invoke();
    }
}