using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Component that makes a vertical ScrollRect pass horizontal drag events to parent horizontal ScrollRect.
/// Attach this to a GameObject with a ScrollRect component to enable touch passthrough.
/// </summary>
namespace Replay.Utils.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectTouchPassthrough : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IScrollHandler
    {
        private ScrollRect _scrollRect;
        private ScrollRect _parentScrollRect;
        private bool _routeToParent = false;

        [Header("Settings")] [SerializeField]
        private float directionThreshold = 1.5f; // How much stronger one direction needs to be

        [SerializeField] private bool debugMode = false;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();

            // Find parent ScrollRect
            Transform parent = transform.parent;
            while (parent != null)
            {
                _parentScrollRect = parent.GetComponent<ScrollRect>();
                if (_parentScrollRect != null)
                {
                    if (debugMode)
                        Debug.Log($"[ScrollRectTouchPassthrough] Found parent ScrollRect on: {parent.name}");
                    break;
                }

                parent = parent.parent;
            }

            if (_parentScrollRect == null && debugMode)
                Debug.LogWarning("[ScrollRectTouchPassthrough] No parent ScrollRect found!");
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
                _parentScrollRect.OnInitializePotentialDrag(eventData);
            _scrollRect.OnInitializePotentialDrag(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_parentScrollRect == null)
            {
                _scrollRect.OnBeginDrag(eventData);
                return;
            }

            // Determine if we should route to parent based on drag direction
            float horizontalDelta = Mathf.Abs(eventData.delta.x);
            float verticalDelta = Mathf.Abs(eventData.delta.y);

            if (debugMode)
                Debug.Log($"[ScrollRectTouchPassthrough] Drag delta - H: {horizontalDelta}, V: {verticalDelta}");

            // If this is a vertical ScrollRect and horizontal drag is stronger, route to parent
            if (_scrollRect.vertical && !_scrollRect.horizontal)
            {
                if (horizontalDelta > verticalDelta * directionThreshold)
                {
                    _routeToParent = true;
                    if (debugMode)
                        Debug.Log("[ScrollRectTouchPassthrough] Routing horizontal drag to parent");
                    _parentScrollRect.OnBeginDrag(eventData);
                    return;
                }
            }
            // If this is a horizontal ScrollRect and vertical drag is stronger, route to parent
            else if (_scrollRect.horizontal && !_scrollRect.vertical)
            {
                if (verticalDelta > horizontalDelta * directionThreshold)
                {
                    _routeToParent = true;
                    if (debugMode)
                        Debug.Log("[ScrollRectTouchPassthrough] Routing vertical drag to parent");
                    _parentScrollRect.OnBeginDrag(eventData);
                    return;
                }
            }

            _routeToParent = false;
            _scrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_routeToParent && _parentScrollRect != null)
            {
                _parentScrollRect.OnDrag(eventData);
            }
            else
            {
                _scrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_routeToParent && _parentScrollRect != null)
            {
                _parentScrollRect.OnEndDrag(eventData);
                _routeToParent = false;
            }
            else
            {
                _scrollRect.OnEndDrag(eventData);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            // For scroll wheel events, check if we're at the limits
            if (_parentScrollRect != null)
            {
                bool atTop = _scrollRect.verticalNormalizedPosition >= 1f;
                bool atBottom = _scrollRect.verticalNormalizedPosition <= 0f;
                bool scrollingUp = eventData.scrollDelta.y > 0;
                bool scrollingDown = eventData.scrollDelta.y < 0;

                // If we're at a limit and trying to scroll beyond it, pass to parent
                if ((atTop && scrollingUp) || (atBottom && scrollingDown))
                {
                    _parentScrollRect.OnScroll(eventData);
                    return;
                }
            }

            _scrollRect.OnScroll(eventData);
        }
    }
}