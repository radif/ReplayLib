using System;
using UnityEngine;

namespace Replay.Utils
{
    public static class PlatformUtils
    {
        //platform specific strings
        public static T GetPlatformSpecificObject<T>(T iOSObject, T androidObject)
        {
#if UNITY_EDITOR
            return iOSObject;
#endif
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return iOSObject;
                case RuntimePlatform.Android:
                    return androidObject;
            }
            
            throw new NotImplementedException("GetPlatformSpecificString not implemented for this platform");
            return iOSObject;
        }
        
    }

}