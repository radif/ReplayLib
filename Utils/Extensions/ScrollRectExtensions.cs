using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Replay.Utils
{
    public static class ScrollRectExtensions
    {
        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect scrollRect, RectTransform child)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
            Vector2 childLocalPosition   = child.localPosition;
            Vector2 result = new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
            return result;
        }
        
        public static void SnapTo(this ScrollRect scrollRect, RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            var content = scrollRect.content;
            content.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }
    }
}