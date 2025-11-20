using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A ScrollRect that passes drag events to parent ScrollRect when dragging in the non-scrollable direction.
/// Useful for nested ScrollRects where child handles vertical scrolling and parent handles horizontal scrolling.
/// </summary>
namespace Replay.Utils.UI
{
    public class NestedScrollRect : ScrollRect
    {
        private bool _routeToParent = false;
        private ScrollRect _parentScrollRect;
        private ScrollRectPaginator _parentPaginator;

        protected override void Awake()
        {
            base.Awake();

            // Find parent ScrollRect
            Transform parent = transform.parent;
            while (parent != null)
            {
                _parentScrollRect = parent.GetComponent<ScrollRect>();
                if (_parentScrollRect != null)
                {
                    // Also check for ScrollRectPaginator on the same GameObject
                    _parentPaginator = parent.GetComponent<ScrollRectPaginator>();
                    break;
                }
                parent = parent.parent;
            }
        }

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
                _parentScrollRect.OnInitializePotentialDrag(eventData);
            //if (_parentPaginator != null)
            //    _parentPaginator.OnInitializePotentialDrag(eventData);
            base.OnInitializePotentialDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (_parentScrollRect == null)
            {
                base.OnBeginDrag(eventData);
                return;
            }

            // Determine if we should route to parent based on drag direction
            float horizontalDelta = Mathf.Abs(eventData.delta.x);
            float verticalDelta = Mathf.Abs(eventData.delta.y);

            // If this is a vertical ScrollRect and horizontal drag is stronger, route to parent
            if (vertical && !horizontal)
            {
                if (horizontalDelta > verticalDelta * 1.5f) // 1.5f threshold for detecting horizontal intent
                {
                    _routeToParent = true;
                    _parentScrollRect.OnBeginDrag(eventData);
                    if (_parentPaginator != null)
                        _parentPaginator.OnBeginDrag(eventData);
                    return;
                }
            }
            // If this is a horizontal ScrollRect and vertical drag is stronger, route to parent
            else if (horizontal && !vertical)
            {
                if (verticalDelta > horizontalDelta * 1.5f)
                {
                    _routeToParent = true;
                    _parentScrollRect.OnBeginDrag(eventData);
                    if (_parentPaginator != null)
                        _parentPaginator.OnBeginDrag(eventData);
                    return;
                }
            }

            _routeToParent = false;
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_routeToParent)
            {
                if (_parentScrollRect != null)
                    _parentScrollRect.OnDrag(eventData);
                //if (_parentPaginator != null)
                //    _parentPaginator.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_routeToParent)
            {
                if (_parentScrollRect != null)
                    _parentScrollRect.OnEndDrag(eventData);
                if (_parentPaginator != null)
                    _parentPaginator.OnEndDrag(eventData);
                _routeToParent = false;
            }
            else
            {
                base.OnEndDrag(eventData);
            }
        }

        public override void OnScroll(PointerEventData eventData)
        {
            // For scroll wheel events, check if we can scroll in the direction
            // If we can't, pass to parent
            if (_parentScrollRect != null)
            {
                bool canScrollVertical =
                    vertical && (verticalNormalizedPosition > 0f && verticalNormalizedPosition < 1f);
                bool canScrollHorizontal =
                    horizontal && (horizontalNormalizedPosition > 0f && horizontalNormalizedPosition < 1f);

                if (!canScrollVertical && !canScrollHorizontal)
                {
                    _parentScrollRect.OnScroll(eventData);
                    return;
                }
            }

            base.OnScroll(eventData);
        }
    }
}