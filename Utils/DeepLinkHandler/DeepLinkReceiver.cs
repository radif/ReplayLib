using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Replay.Utils
{
    public class DeepLinkReceiver : ComponentSingleton<DeepLinkReceiver>, IDebugLoggable
    {
        public string logTag => "DeepLinkReceiver";
        
        [Preserve]
        public static bool ShouldCreateOwnGameObject() => true;
        [Preserve]
        public static bool ShouldEnableDontDestroyOnLoad() => true;
        
        [Preserve]
        public static bool ShouldNotRenameGameObject() => true;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() =>
            Load();

        public string lastReceivedDeepLink { get; private set; } = "";
        public DateTime lastReceivedDeepLinkTime { get; private set; } = default;

        public Action<string> onDeepLinkActivated;
        public void ActivateAppLaunchDeepLink()
        {
            if(!string.IsNullOrEmpty(Application.absoluteURL))
                OnDeepLinkReceived(Application.absoluteURL);    
        }
        
        bool _isSubscribedToNativeDeepLinkingEvents = false;
        public void SubscribeToNativeDeepLinks()
        {
            if (!_isSubscribedToNativeDeepLinkingEvents)
            {
                Application.deepLinkActivated += OnDeepLinkReceived;
                _isSubscribedToNativeDeepLinkingEvents = true;    
            }
        }
        public void UnsubscribeFromNativeDeepLinks()
        {
            if (_isSubscribedToNativeDeepLinkingEvents)
            {
                Application.deepLinkActivated -= OnDeepLinkReceived;
                _isSubscribedToNativeDeepLinkingEvents = false;    
            }
        }

        private void Awake()
        {
            //SubscribeToNativeDeepLinks();
        }

        private void OnDestroy()
        {
            UnsubscribeFromNativeDeepLinks();
        }

        
        [Preserve]
        public void OnDeepLinkReceived(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {

                //checking for duplicate deep links
                // if(lastReceivedDeepLink == url && lastReceivedDeepLinkTime > ServerTimeRetriever.Instance.currentDeviceTimeUniversal.AddSeconds(-1F))
                // {
                //     this.DevLogWarning("Received duplicate deep link: " + url);
                // }
                // else
                // {
                    if (onDeepLinkActivated != null)
                    {
                        this.DevLog($"+Received deep link: {url}");
                        onDeepLinkActivated(url);
                    }
                    else
                        this.DevLog($"-Received deep link: {url}");

                    lastReceivedDeepLink = url;
                    lastReceivedDeepLinkTime = ServerTimeRetriever.Instance.currentDeviceTimeUniversal;
                //}
            }
            else
            {
                this.DevLogWarning("Received null or empty URL from native layer");
            }
        }
        
        public string ToDebugString()
        {
            string retVal = "DeepLinkReceiver\n";
            retVal += "lastReceivedDeepLink: " + lastReceivedDeepLink + "\n";
            retVal += "Application.absoluteURL: " + Application.absoluteURL + "\n";
            retVal += "onDeepLinkActivated: " + (onDeepLinkActivated != null).ToBoolString() + "\n";
            return retVal;
        }

        
    }
}
