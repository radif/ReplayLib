using System;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public class InputHandler : MonoBehaviour
    {
        public enum Platform 
        { 
            MOBILE, 
            DESKTOP 
        }
        public Platform inputPlatform = Platform.DESKTOP;
        private Dictionary<Platform, InputPlatform> _platformInput = new Dictionary<Platform, InputPlatform>();
        public Action PointerUp;
        private void Awake()
        {
            _platformInput.Add(Platform.MOBILE, new MobileInput());
            _platformInput.Add(Platform.DESKTOP, new DesktopInput());
        }
        private void Update()
        {
            if (GetPointerUp()) 
                PointerUp?.Invoke();
        }
        public bool GetPointerDown() => _platformInput[inputPlatform].GetPointerDown();
        public bool GetPointer() => _platformInput[inputPlatform].GetPointer();
        public bool GetPointerUp() => _platformInput[inputPlatform].GetPointerUp();
        public Vector2 GetPointerPosition() => _platformInput[inputPlatform].GetPointerPosition();
    }
}