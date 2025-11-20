using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class TransformExtensions
    {
        //position
        public static void SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
        }

        public static void BumpLocalPositionXBy(this Transform transform, float x)
        {
            Vector3 pos = transform.localPosition;
            pos.x += x;
            transform.localPosition = pos;
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
        }

        public static void BumpLocalPositionYBy(this Transform transform, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.y += y;
            transform.localPosition = pos;
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
        }

        public static void BumpLocalPositionZBy(this Transform transform, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.z += z;
            transform.localPosition = pos;
        }

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void SetLocalPosition(this Transform transform, float x, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            pos.y = y;
            transform.localPosition = pos;
        }

        public static void BumpLocalPositionBy(this Transform transform, float x, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.x += x;
            pos.y += y;
            transform.localPosition = pos;
        }

        public static void SetLocalPositionXZ(this Transform transform, float x, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            pos.z = z;
            transform.localPosition = pos;
        }

        public static void BumpLocalPositionXZBy(this Transform transform, float x, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.x += x;
            pos.z += z;
            transform.localPosition = pos;
        }

        public static void SetLocalPosition(this Transform transform, float x, float y, float z)
        {
            transform.localPosition = new Vector3(x, y, z);
        }

        public static void BumpLocalPositionBy(this Transform transform, float x, float y, float z)
        {
            transform.localPosition += new Vector3(x, y, z);
        }

        public static void SetPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            transform.position = pos;
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.position;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.position;
            pos.z = z;
            transform.position = pos;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void SetPosition(this Transform transform, float x, float y)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;
        }

        public static void SetPositionXZ(this Transform transform, float x, float z)
        {
            Vector3 pos = transform.position;
            pos.x = x;
            pos.z = z;
            transform.position = pos;
        }

        public static void SetPosition(this Transform transform, float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        //rotation
        public static void SetLocalRotationX(this Transform transform, float x)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.x = x;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void BumpLocalRotationXBy(this Transform transform, float x)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.x += x;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetLocalRotationY(this Transform transform, float y)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.y = y;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void BumpLocalRotationYBy(this Transform transform, float y)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.y += y;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetLocalRotationZ(this Transform transform, float z)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.z = z;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void BumpLocalRotationZBy(this Transform transform, float z)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.z += z;
            transform.localRotation = Quaternion.Euler(rot);
        }

        public static void SetLocalRotation(this Transform transform, float x, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(x, y, z));
        }

        public static void BumpLocalRotationBy(this Transform transform, float x, float y, float z)
        {
            transform.localRotation *= Quaternion.Euler(new Vector3(x, y, z));
        }

        public static void SetRotationX(this Transform transform, float x)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.x = x;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void BumpRotationXBy(this Transform transform, float x)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.x += x;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void SetRotationY(this Transform transform, float y)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y = y;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void BumpRotationYBy(this Transform transform, float y)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += y;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void SetRotationZ(this Transform transform, float z)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = z;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void BumpRotationZBy(this Transform transform, float z)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z += z;
            transform.rotation = Quaternion.Euler(rot);
        }

        public static void SetRotation(this Transform transform, float x, float y, float z)
        {
            transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
        }

        public static void BumpRotationBy(this Transform transform, float x, float y, float z)
        {
            transform.rotation *= Quaternion.Euler(new Vector3(x, y, z));
        }

        public static void ApplyComponentSettings(this Transform transform, Transform other, bool local = false)
        {
            if (local)
            {
                transform.localPosition = other.localPosition;
                transform.localRotation = other.localRotation;
            }
            else
            {
                transform.position = other.position;
                transform.rotation = other.rotation;
            }
            
            transform.localScale = other.localScale;
        }

        //scale
        public static void SetLocalScaleUniform(this Transform transform, float s)
        {
            transform.localScale = new Vector3(s, s, s);
        }


        //children
        public static void IterateChildren(this Transform transform, Action<Transform> onEachHandle)
        {
            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
                onEachHandle?.Invoke(transform.GetChild(i));
        }

        public static List<Transform> GetChildrenCopy(this Transform transform)
        {
            var retVal = new List<Transform>();
            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
                retVal.Add(transform.GetChild(i));
            return retVal;
        }

        public static void DestroyChildrenGameObjects(this Transform transform)
        {
            var children = transform.GetChildrenCopy();
            foreach (var child in children)
                GameObject.Destroy(child.gameObject);
        }

        //LookAt
        public static void LookAwayFrom(this Transform transform, Vector3 position, Vector3? up = null)
        {
            // Calculate direction from position to transform
            Vector3 directionAway = transform.position - position;
    
            // If the direction is not zero, rotate to look away
            if (directionAway != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionAway, up ?? Vector3.up);
            }
        }

        //Canvas
        public static RectTransform GetRectTransform(this Transform transform) => transform.GetComponent<RectTransform>();

    }

}

