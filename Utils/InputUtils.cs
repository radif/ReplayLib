using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

namespace Replay.Utils
{
    public class InputHelper : ComponentSingleton<InputHelper>
    {
        [Preserve]
        public static bool ShouldCreateOwnGameObject() => true;
        [Preserve]
        public static bool ShouldEnableDontDestroyOnLoad() => true;

        public Action<bool> OnAnyTouchOverUIChanged;

        public bool IsAnyTouchOverUI { get; private set; } = false;
        private void Update()
        {
            bool value = InputUtils.IsAnyTouchOverUI();
            if(value != IsAnyTouchOverUI)
            {
                IsAnyTouchOverUI = value;

                OnAnyTouchOverUIChanged?.Invoke(value);
            }
        }
    }
    public static class InputUtils
    {
        public static bool IsAnyTouchOverUI()
        {
            bool retVal = false;
            foreach (var touch in Input.touches)
            {
                if (touch.IsOverUI())
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }
        public static bool IsOverUI(this Touch touch) => IsScreenPositionOverUI(touch.position);

        public static bool IsScreenPositionOverUI(Vector2 position)
        {
            // Check if position is in safe area for iOS
            //if (!IsPositionInSafeArea(position))
            //    return true; // Treat touches outside safe area as over UI
            
            if (ScreenUtils.IsTouchNearScreenEdge(position, 6))
                return true;

            PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
            PointerEventData.position = position;
            List<RaycastResult> retVal = new List<RaycastResult>();
            EventSystem.current.RaycastAll(PointerEventData, retVal);
            return retVal.Count > 0;
        }
        
        public static List<RaycastResult> GetUIObjectsForScreenPosition(Vector2 position)
        {
            PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
            PointerEventData.position = position;
            List<RaycastResult> retVal = new List<RaycastResult>();
            EventSystem.current.RaycastAll(PointerEventData, retVal);
            return retVal;
        }
        public static Collider RaycastToColliderForScreenPosition(Vector2 screenPosition)
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out hit);
            return hit.collider;
        }
    }

}

