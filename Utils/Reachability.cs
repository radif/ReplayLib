using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Replay.Utils
{
    public class Reachability : ComponentSingleton<Reachability>, IDebugLoggable
    {
        [Header("Ping Details:")]
        [SerializeField] private string[] pingURLs = new[] {
            "https://www.google.com/generate_204",
            "https://www.apple.com",
            "https://www.microsoft.com"
        };
        [SerializeField] private int pingTimeout = 1;
        [Tooltip("Interval between pings")]
        [SerializeField] float pingUpdateInterval = 5F;
        [SerializeField] public bool continuouslyTestInternetConnection = true;
        [Header("Startup Settings")]
        [SerializeField] public bool isConnectedOnStartAssumption = false;
        [Header("Backgrounding:")]
        [SerializeField] public bool listensToBackgroundingNotifications = true;
        [SerializeField] public bool listensToConnectivityNotifications = true;
        
        private bool _lastPingSuccess = false;
        private UnityWebRequest _webRequest = null;
        
#region MonoBehavior
        private void Awake()
        {
            _lastPingSuccess = isConnectedOnStartAssumption;
            RestartPingTimer();
        }
        private void OnDestroy()
        {
            ResetPing();
        }
        public void Update()
        {
            if(continuouslyTestInternetConnection && listensToConnectivityNotifications)
                ValidateResetPing();
            
            ValidatePing();
        }

        // private void OnApplicationFocus(bool hasFocus)
        // {
        //     if(hasFocus)
        //         if(listensToBackgroundingNotifications)
        //             ResetPing();
        // }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(!pauseStatus)
                if(listensToBackgroundingNotifications)
                    ResetPing();
        }

        #endregion

#region Ping Logic
        private bool _isPingDelayInProgress = false;
        void PingInternet(bool withDelay = true)
        {
            ResetPing();
            if (continuouslyTestInternetConnection)
            {
                _isPingDelayInProgress = true;
                this.Delay(withDelay ? pingUpdateInterval : 0F, () =>
                {
                    if (continuouslyTestInternetConnection)
                    {
                       
                        foreach (var url in pingURLs)
                        {
                            try
                            {
                                _webRequest = UnityWebRequest.Get(url);
                                _webRequest.timeout = pingTimeout;
                                _isPingDelayInProgress = false;
                                _webRequest.SendWebRequest();
                                break;
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning($"Reachability: Failed to ping {url}: {e.Message}");
                                ResetPing();
                            }
                        }
                        
                    }
                    else
                        ResetPing();
                });
            }
            else
                ResetPing();
        }

        public bool isPingScheduledOrInProgress => isPingInProgress || _isPingDelayInProgress;
        public bool isPingInProgress => _webRequest != null;

        private bool? _hadConnectionPastFrame = null;
        void ValidateResetPing()
        {
            bool hasConnection = Application.internetReachability != NetworkReachability.NotReachable;
            if (_hadConnectionPastFrame != null)
            {
                if (hasConnection && !_hadConnectionPastFrame.Value)
                {
                    ResetPing();
                }    
            }
            _hadConnectionPastFrame = hasConnection;
        }
        void ResetPing()
        {
            _isPingDelayInProgress = false;
            if(isPingInProgress)
            {
                _webRequest.Dispose();
                _webRequest = null;
            }
        }

        void ValidatePing()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ResetPing();
                _lastPingSuccess = false;
            }
            else
            {
                if(continuouslyTestInternetConnection)
                {
                    if (isPingScheduledOrInProgress && isPingInProgress)
                    {
                        if (_webRequest.isDone)
                        {
#if UNITY_2020_2_OR_NEWER
                            _lastPingSuccess = _webRequest.result == UnityWebRequest.Result.Success;
#else
                            _lastPingSuccess = !_webRequest.isNetworkError && !_webRequest.isHttpError;
#endif
                            ResetPing();
                        }
                    }

                    if (!isPingScheduledOrInProgress)
                    {
                        PingInternet(_lastPingSuccess);
                    }    
                }
                else
                    ResetPing();
            }
        }
#endregion

#region APIs
        public bool isNetworkReachable
        {
            get
            {
                //1. ping is the first option
                if (_lastPingSuccess)
                    return true;
                
                //2. fallback to the Unity Reachability API (this is for the network, it doesn't check the internet like ping)
                if (Application.internetReachability != NetworkReachability.NotReachable)
                    return true;
                
                //3. fallback to not reachable
                return false;
            }
        }
        public enum NetworkType
        {
            none,
            WiFi,
            Mobile   
        }
        public NetworkType networkType
        {
            get
            {
                switch (Application.internetReachability)
                {
                    case NetworkReachability.NotReachable:
                        return NetworkType.none;
                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        return NetworkType.WiFi;
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        return NetworkType.Mobile;
                    default:
                        return NetworkType.none;
                }
            }
        }
        
        public bool isInternetReachable => _lastPingSuccess;
        
        public void SetContinuouslyTestInternetConnectionEnabled(bool mContinuouslyTestInternetConnection)
        {
            if (continuouslyTestInternetConnection != mContinuouslyTestInternetConnection)
            {
                continuouslyTestInternetConnection = mContinuouslyTestInternetConnection;
                RestartPingTimer();
            }
        }

        public void RestartPingTimer()
        {
            ResetPing();
            PingInternet(false);  
        }
#endregion
        public string ToDebugString()
        {
            return "Continuous Internet Testing: " + continuouslyTestInternetConnection.ToBoolString() + "\n" +
                   "Network Reachable: " + isNetworkReachable.ToBoolString() + "\n" +
                   "Internet Reachable: " + isInternetReachable.ToBoolString() + "\n" +
                   "Is Ping Scheduled: " + isPingScheduledOrInProgress.ToBoolString() + "\n" +
                   "Is Ping In Progress: " + isPingInProgress.ToBoolString();
        }
    }
}