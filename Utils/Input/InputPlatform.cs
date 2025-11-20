using UnityEngine;

namespace Replay.Utils
{
    
    public abstract class InputPlatform
    {
        public abstract bool GetPointerDown();
        public abstract bool GetPointer();
        public abstract bool GetPointerUp();
        public abstract Vector2 GetPointerPosition();
    }
}