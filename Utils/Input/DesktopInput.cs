using UnityEngine;

namespace Replay.Utils
{
    public class DesktopInput : InputPlatform
    {
        public override bool GetPointer() => Input.GetMouseButton(0);
        public override bool GetPointerDown() => Input.GetMouseButtonDown(0);
        public override bool GetPointerUp() => Input.GetMouseButtonUp(0);
        public override Vector2 GetPointerPosition() => Input.mousePosition;
    }
}