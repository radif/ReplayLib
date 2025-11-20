namespace Replay.Utils
{
    public static class NativeSplashScreen
    {
        public static void RemoveNativeSplashScreen()
        {
#if !UNITY_EDITOR
#if UNITY_IOS
        NativeSplashScreen_RemoveNativeSplashScreeniOS();
#elif UNITY_ANDROID
        NativeSplashScreen_RemoveNativeSplashScreenAndroid();
#endif
#endif
        }
        
#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void NativeSplashScreen_RemoveNativeSplashScreeniOS();
#endif
        
#if UNITY_ANDROID && !UNITY_EDITOR
    //[System.Runtime.InteropServices.DllImport("__Internal")]
    //private static extern void NativeSplashScreen_RemoveNativeSplashScreenAndroid();
#endif
        
    }
}