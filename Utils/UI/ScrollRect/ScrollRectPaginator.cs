using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Snap a scroll rect to its children items. All self contained.
/// Note: Only supports 1 direction
/// </summary>
namespace Replay.Utils.UI
{
    public class ScrollRectPaginator : UIBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public ScrollRect scrollRect; // the scroll rect to scroll
        public SnapDirection direction; // the direction we are scrolling
        public int itemCount; // how many items we have in our scroll rect

        public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // a curve for transitioning in order to give it a little bit of extra polish

        public float minVelocity = 0.61F;
        public float snapperSpeedMultiplier = 2.5F;
        public float deltaMultiplier = 1F;
        public bool snapsAtProximatePage = false;
        public bool excludeScrollRectWidthSubtractionFromSnapLogic = false;
        public Action onDoneScrollSnapping = null;
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            if (scrollRect == null) // if we are resetting or attaching our script, try and find a scroll rect for convenience 
                scrollRect = GetComponent<ScrollRect>();
        }
#endif
        public void OnBeginDrag(PointerEventData eventData)
        {
            _vel = 0;
            _StopAllCoroutines();
            StartCoroutine(MeasureSpeed());
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Mathf.Abs(_vel) < minVelocity)
            {
                _StopAllCoroutines();
                _snapRectPrefersDirection = false;
                StartCoroutine(SnapRect());
            }
            else
                StartCoroutine(SnapWhenSlow());
        }

        void _StopAllCoroutines()
        {
            _isSnappingRect = false;
            StopCoroutine(SnapRect());

            _snappingWhenSlow = false;
            StopCoroutine(SnapWhenSlow());

            _measuringSpeed = false;
            StopCoroutine(MeasureSpeed());
            
            StopCoroutine(nameof(ScrollToPageIndexAnimated));
        }


        float _vel = 0;
        float _lastOffset = 0F;
        bool _measuringSpeed = false;
        private IEnumerator MeasureSpeed()
        {
            _measuringSpeed = true;
            while (_measuringSpeed)
            {
                float offset;

                if (direction == SnapDirection.Horizontal)
                {
                    offset = scrollRect.content.anchoredPosition.x;
                }
                else
                {
                    offset = scrollRect.content.anchoredPosition.y;
                }

                float distance = offset - _lastOffset;
                _lastOffset = offset;

                float currVel = distance * Time.deltaTime;

                _vel = (_vel + currVel) / 2F;

                yield return new WaitForEndOfFrame();
            }
        }

        bool _snappingWhenSlow = false;
        private IEnumerator SnapWhenSlow()
        {
            _snappingWhenSlow = true;
            while (_snappingWhenSlow)
            {
                if (Mathf.Abs(_vel) < minVelocity || snapsAtProximatePage)
                {
                    _StopAllCoroutines();
                    _snapRectPrefersDirection = true;
                    StartCoroutine(SnapRect());
                }
                else
                    yield return new WaitForEndOfFrame();
            }
        }

        bool _snapRectPrefersDirection = false;
        bool _isSnappingRect = false;
        private IEnumerator SnapRect()
        {
            _isSnappingRect = true;
            if (scrollRect == null)
                throw new Exception("Scroll Rect can not be null");
            if (itemCount == 0)
                throw new Exception("Item count can not be zero");

            float startNormal = direction == SnapDirection.Horizontal ? scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition; // find our start position
            float delta = 1f / (float)(itemCount - 1); // percentage each item takes
            delta *= deltaMultiplier;
            float rawTarget = startNormal / delta;

            int target = 0; // this finds us the closest target based on our starting point
            if (_snapRectPrefersDirection)
            {
                if (_vel < 0)
                    target = Mathf.CeilToInt(rawTarget);
                else
                    target = Mathf.FloorToInt(rawTarget);
            }
            else
                target = Mathf.RoundToInt(rawTarget);

            if (target < 0)
                target = 0;

            if (target > itemCount - 1)
                target = itemCount - 1;

            float endNormal = delta * target; // this finds the normalized value of our target

            float normalizedSpeed = (0.5F / (float)itemCount) * snapperSpeedMultiplier;

            float duration = Mathf.Abs((endNormal - startNormal) / normalizedSpeed); // this calculates the time it takes based on our speed to get to our target

            float timer = 0f; // timer value of course
            while (timer < 1f && _isSnappingRect) // loop until we are done
            {
                timer = Mathf.Min(1f, timer + Time.deltaTime / duration); // calculate our timer based on our speed
                float value = Mathf.Lerp(startNormal, endNormal, curve.Evaluate(timer)); // our value based on our animation curve, cause linear is lame

                //only withing the scrollable area, otherwise, springy effect will take care
                if (direction == SnapDirection.Horizontal)
                {
                    float pos = scrollRect.content.anchoredPosition.x;

                    if (excludeScrollRectWidthSubtractionFromSnapLogic)
                    {
                        if (pos < 0 && pos > -(scrollRect.content.sizeDelta.x))
                        {
                            scrollRect.horizontalNormalizedPosition = value;
                        }
                    }
                    else
                    {
                        if (pos < 0 && pos > -(scrollRect.content.sizeDelta.x - scrollRect.GetComponent<RectTransform>().sizeDelta.x))
                        {
                            if (direction == SnapDirection.Horizontal) // depending on direction we set our horizontal or vertical position
                                scrollRect.horizontalNormalizedPosition = value;
                            else
                                scrollRect.verticalNormalizedPosition = value;
                        }
                    }
                }
                else
                {
                    float pos = scrollRect.content.anchoredPosition.y;

                    if (excludeScrollRectWidthSubtractionFromSnapLogic)
                    {
                        if (pos > 0 && pos < (scrollRect.content.sizeDelta.y))
                        {
                            scrollRect.verticalNormalizedPosition = value;
                        }
                    }
                    else
                    {
                        if (pos > 0 && pos < (scrollRect.content.sizeDelta.y - scrollRect.GetComponent<RectTransform>().sizeDelta.y))
                        {
                            scrollRect.verticalNormalizedPosition = value;
                        }
                    }
                }

                yield return new WaitForEndOfFrame(); // wait until next frame
            }
            onDoneScrollSnapping?.Invoke();
        }
        
        public int GetCurrentPage()
        {
            float delta = 1f / (float)(itemCount - 1);
            float rawTarget = (direction == SnapDirection.Horizontal ? scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition) / delta;
            int target = Mathf.RoundToInt(rawTarget);
            return target;
        }
        
        public void ScrollToPageIndex(int index, bool animated = true, Action onComplete = null)
        {
            if (index < 0 || index >= itemCount)
                return;

            float delta = 1f / (float)(itemCount - 1);
            float target = delta * index;
    
            if (animated)
                StartCoroutine(ScrollToPageIndexAnimated(target, onComplete));
            else
            {
                if (direction == SnapDirection.Horizontal)
                    scrollRect.horizontalNormalizedPosition = target;
                else
                    scrollRect.verticalNormalizedPosition = target;
                onComplete?.Invoke();
            }
        }

        private IEnumerator ScrollToPageIndexAnimated(float target, Action onComplete)
        {
            float duration = 0.3F;
            float timer = 0f;
            float startValue = direction == SnapDirection.Horizontal 
                ? scrollRect.horizontalNormalizedPosition 
                : scrollRect.verticalNormalizedPosition;
        
            while (timer < 1f)
            {
                timer = Mathf.Min(1f, timer + Time.deltaTime / duration);
                float value = Mathf.Lerp(startValue, target, curve.Evaluate(timer));
        
                if (direction == SnapDirection.Horizontal)
                    scrollRect.horizontalNormalizedPosition = value;
                else
                    scrollRect.verticalNormalizedPosition = value;
            
                yield return new WaitForEndOfFrame();
            }
    
            onDoneScrollSnapping?.Invoke();
            onComplete?.Invoke();
        }
    }

    // The direction we are snapping in
    public enum SnapDirection
    {
        Horizontal,
        Vertical,
    }
    
    
}