using System.Runtime.InteropServices;

namespace Replay.Utils
{
    public static class KeychainPlugin
    {
        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern bool _SaveToKeychain(string key, string value);

        [DllImport("__Internal")]
        private static extern string _LoadFromKeychain(string key);

        [DllImport("__Internal")]
        private static extern bool _DeleteFromKeychain(string key);
        #endif

        public static bool SetString(string key, string value)
        {
            #if UNITY_IOS && !UNITY_EDITOR
            return _SaveToKeychain(key, value);
            #else
            return true;
            #endif
        }

        public static string GetString(string key)
        {
            #if UNITY_IOS && !UNITY_EDITOR
            return _LoadFromKeychain(key);
            #else
            return null;
            #endif
        }

        public static bool DeleteKey(string key)
        {
            #if UNITY_IOS && !UNITY_EDITOR
            return _DeleteFromKeychain(key);
            #else
            return true;
            #endif
        }
    }
}
