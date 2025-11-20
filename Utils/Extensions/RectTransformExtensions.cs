using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class RectTransformExtensions
    {

        //position
        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.x = x;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = y;
            rectTransform.anchoredPosition = pos;
        }

        public static void SetSizeDeltaX(this RectTransform rectTransform, float x)
        {
            Vector2 contentSize = rectTransform.sizeDelta;
            contentSize.x = x;
            rectTransform.sizeDelta = contentSize;
        }

        public static void SetSizeDeltaY(this RectTransform rectTransform, float y)
        {
            Vector2 contentSize = rectTransform.sizeDelta;
            contentSize.y = y;
            rectTransform.sizeDelta = contentSize;
        }

        //rotation
        public static void SetLocalRotationX(this RectTransform rectTransform, float x)
        {
            Vector3 rot = rectTransform.localRotation.eulerAngles;
            rot.x = x;
            rectTransform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetLocalRotationY(this RectTransform rectTransform, float y)
        {
            Vector3 rot = rectTransform.localRotation.eulerAngles;
            rot.y = y;
            rectTransform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetLocalRotationZ(this RectTransform rectTransform, float z)
        {
            Vector3 rot = rectTransform.localRotation.eulerAngles;
            rot.z = z;
            rectTransform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetRotationX(this RectTransform rectTransform, float x)
        {
            Vector3 rot = rectTransform.rotation.eulerAngles;
            rot.x = x;
            rectTransform.rotation = Quaternion.Euler(rot);
        }

        public static void SetRotationY(this RectTransform rectTransform, float y)
        {
            Vector3 rot = rectTransform.rotation.eulerAngles;
            rot.y = y;
            rectTransform.rotation = Quaternion.Euler(rot);
        }

        public static void SetRotationZ(this RectTransform rectTransform, float z)
        {
            Vector3 rot = rectTransform.rotation.eulerAngles;
            rot.z = z;
            rectTransform.rotation = Quaternion.Euler(rot);
        }

        //scale
        public static void SetLocalScaleUniform(this RectTransform rectTransform, float s)
        {
            rectTransform.localScale = new Vector3(s, s, s);
        }


        //children
        public static void IterateChildren(this RectTransform rectTransform, Action<Transform> onEachHandle)
        {
            int count = rectTransform.childCount;
            for (int i = 0; i < count; ++i)
                onEachHandle?.Invoke(rectTransform.GetChild(i));
        }

        public static List<Transform> GetChildrenCopy(this RectTransform rectTransform)
        {
            var retVal = new List<Transform>();
            int count = rectTransform.childCount;
            for (int i = 0; i < count; ++i)
                retVal.Add(rectTransform.GetChild(i));
            return retVal;
        }

        public static void DestroyChildrenGameObjects(this RectTransform rectTransform)
        {
            var children = rectTransform.GetChildrenCopy();
            foreach (var child in children)
                GameObject.Destroy(child.gameObject);
        }

        //Transform
        public static Transform GetTransform(this RectTransform rectTransform) => rectTransform.GetComponent<Transform>();

        //coordinates
        public static Vector2? ScreenPointToLocalPoint(this RectTransform rectTransform, Vector2 screenPoint, Camera camera = null)
        {
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 endPosition))
                return endPosition;
            return null;
        }
        public static bool ContainsScreenPoint(this RectTransform rectTransform, Vector2 screenPoint, Camera camera = null)
        {
           if(camera == null)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);

            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, camera);
        }


        //misc
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            if (rectTransform == null)
                throw new ArgumentNullException(nameof(rectTransform));

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 bottomLeft = corners[0];

            Vector2 size = new Vector2(
                rectTransform.lossyScale.x * rectTransform.rect.size.x,
                rectTransform.lossyScale.y * rectTransform.rect.size.y);

            return new Rect(bottomLeft, size);
        }
    }

}

