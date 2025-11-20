using System;
using System.Runtime.InteropServices;
using Replay.Utils;


//public class SharePlugin : MonoBehaviour
public class ShareSheet : Singleton<ShareSheet>, IDebugLoggable
{
    public string logTag => "ShareSheet";
    delegate void on_complete_native_type(bool completed);
    private Action<bool> _onComplete;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void ShowShareSheet(string message, string imagePath, on_complete_native_type onCompleteFunc);
#endif

    public void ShowShareSheet(string message, string imagePath, Action<bool> onComplete = null)
    {
        _onComplete = onComplete;
#if UNITY_EDITOR
        this.Log("Running on Unity Editor!");
        
        //Mock
        CoroutineUtils.Delay(2.0f, () =>
        {
            OnCompleteNative(true);
        });
#else
#if UNITY_IOS
        this.Log("Running on iOS!");
        ShowShareSheet(message, imagePath, (on_complete_native_type)OnCompleteNative);
#elif UNITY_ANDROID
    // This code only runs on Android
    this.Log("Running on Android!");
    // Android-specific API calls
#else
        // This code runs on all other platforms
        this.Log("Running on Unknown Platform!");

        //Mock
        CoroutineUtils.Delay(2.0f, () =>
        {
            OnCompleteNative();
        });
#endif
#endif
    }
    
    [AOT.MonoPInvokeCallback(typeof(on_complete_native_type))]
    public static void OnCompleteNative(bool completed)
    {
        ThreadUtils.Instance.ExecuteOnMainThreadUpdate(() =>
        {
            if (IsLoaded)
                Instance.OnComplete(completed);
        });
    }
    public void OnComplete(bool completed)
    {
        _onComplete?.Invoke(completed);
        Unload();
    }
} 