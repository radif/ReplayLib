using Replay.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollRectClicker : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public enum ScrollMode {  All = 0, Vertical = 10, Horizontal = 20  }
    [SerializeField] private ScrollMode scrollMode = ScrollMode.All;
    [SerializeField] private float maxClickableDistance = 20F;
    
    ScrollRectClickerComponent[] GetClickers() => GetComponentsInChildren<ScrollRectClickerComponent>();
    
    private Vector2 _startPos;
    public void OnBeginDrag (PointerEventData data)
    {
        _startPos = data.position;
        var clickers = GetClickers();
        foreach (var clicker in clickers)
            clicker.OnScrollRectBeginDrag(data);
    }
  
    public void OnEndDrag (PointerEventData data)
    {
      
        var clickers = GetClickers();
        foreach (var clicker in clickers)
            clicker.OnScrollRectEndDrag(data);

        float distance;

        switch (scrollMode)
        {
            
            case ScrollMode.Vertical:
                distance = Mathf.Abs(_startPos.y - data.position.y);
                break;
            case ScrollMode.Horizontal:
                distance = Mathf.Abs(_startPos.x - data.position.x);
                break;
            case ScrollMode.All:
            default:
                distance = Vector2.Distance (_startPos, data.position);
                break;
        }
        
        if (distance < maxClickableDistance) {
            foreach (var clicker in clickers)
            {
                var raycastObject = data.pointerCurrentRaycast.gameObject;
                if(clicker.gameObject.HasChildObject(raycastObject))
                    clicker.OnScrollRectClick(data);
            }
        }
    }
}
