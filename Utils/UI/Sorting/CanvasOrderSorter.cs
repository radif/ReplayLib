using UnityEngine;
using Replay.Utils.UI;
using SortingLayer = Replay.Utils.UI.SortingLayer;

[RequireComponent(typeof(Canvas))]
public class CanvasOrderSorter : MonoBehaviour
{
    [Header("Canvas Sorting Order Groupped:")]
    public SortingOrder canvasSortingOrderGroup = SortingOrder.undef;
    public int canvasSortingOrderInGroup = 0;
        
    [Header("World Space Only:")]
    public SortingLayer canvasSortingLayer = SortingLayer.Default;

    void Awake()
    {
        ApplyCanvasSorting();
    }
    
    Canvas _canvas = null;
    public Canvas canvas
    {
        get {
            if(_canvas == null)
                _canvas = GetComponent<Canvas>();
            return _canvas;
        }
    }
    
    public void ApplyCanvasSorting()
    {
        canvas.overrideSorting = true;
        canvasSortingLayer.SetOn(canvas);
        canvasSortingOrderGroup.SetOn(canvas, canvasSortingOrderInGroup);
    }
    public int sortingOrder
    {
        get => canvas.sortingOrder;
        set => canvas.sortingOrder = value;
    }
    public int sortingLayerID
    {
        get => canvas.sortingLayerID;
        set => canvas.sortingLayerID = value;
    }

    public string sortingLayerName
    {
        get => canvas.sortingLayerName;
        set => canvas.sortingLayerName = value;
    }
}
