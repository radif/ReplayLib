using System;
using UnityEngine;
using Replay.Utils;
using Replay.BackEnd;
using UnityEngine.Events;

public class ServerTimeRetriever : ComponentSingleton<ServerTimeRetriever>, IDebugLoggable
{
    [SerializeField] float timeUpdateInterval = 120F;
    [SerializeField] float retryInterval = 15F;
    [SerializeField] int maxConsecutiveFailures = 3;
    public string logTag => "ServerTime";
    
    public Action<DateTime> onServerTimeUpdated;
    [SerializeField] private UnityEvent onServerTimeUpdatedEvent;
    public DateTime lastServerTimeUniversal { get; private set; } = default;
    public DateTime lastServerTimeLocal => lastServerTimeUniversal.ToLocalTime();
    public bool lastServerTimeUpdateSuccessful { get; private set; } = false;
    
    private DateTime lastServerTimeUpdateTimestamp = default;
    
    public DateTime currentDeviceTimeUniversal
    {
        get
        {
            if (lastServerTimeUpdateSuccessful)
                return lastServerTimeUniversal.Add(DateTime.UtcNow - lastServerTimeUpdateTimestamp);

            var fallbackTime = DateTime.UtcNow;
            _ApplyMockOffsetIfEnabled(ref fallbackTime);
            
            return fallbackTime;
        }
    }
    
    public DateTime currentDeviceTimeLocal => currentDeviceTimeUniversal.ToLocalTime();
    
    private int consecutiveFailures = 0;
    private Coroutine retryCoroutine;

    private void Start()
    {
        RetrieveServerTime();
        InvokeRepeating(nameof(RetrieveServerTime), timeUpdateInterval, timeUpdateInterval);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if(!pauseStatus)
            RetrieveServerTime();
    }

    //time update logic
    public void RetrieveServerTime()
    {
        // Cancel any existing retry coroutine when a new retrieval is requested
        if (retryCoroutine != null)
        {
            StopCoroutine(retryCoroutine);
            retryCoroutine = null;
        }

        ServerTime.GetNetworkTimeAsync((success, dateTime) =>
        {
            lastServerTimeUpdateSuccessful = success;
            if (success)
            {
                consecutiveFailures = 0;

                _ApplyMockOffsetIfEnabled(ref dateTime);

                lastServerTimeUniversal = dateTime;
                lastServerTimeUpdateTimestamp = DateTime.UtcNow;
                this.DevLog("Server Time Retrieved: " + dateTime);
                onServerTimeUpdated?.Invoke(dateTime);
                onServerTimeUpdatedEvent?.Invoke();
            }
            else
            {
                consecutiveFailures++;
                this.DevLogWarning($"Failed to retrieve server time. Consecutive failures: {consecutiveFailures}");

                // Start retry coroutine if we haven't exceeded max failures
                if (consecutiveFailures <= maxConsecutiveFailures)
                {
                    this.DevLog($"Will retry server time retrieval in {retryInterval} seconds...");
                    
                    this.DevLog("Retrying server time retrieval...");
                    retryCoroutine = this.Delay(retryInterval, () =>
                    {
                        this.DevLog("Retrying server time retrieval...");
                        RetrieveServerTime();
                        retryCoroutine = null;
                    });
                }
            }
        });
    }

#region MockTimeOffset
    bool _DEBUG_useMockTimeOffset = false;
    float _DEBUG_daysOffsetFromRetrievedTime = 0F;

    private void _ApplyMockOffsetIfEnabled(ref DateTime dateTime)
    {
        if (_DEBUG_useMockTimeOffset)
            dateTime = dateTime.AddDays(_DEBUG_daysOffsetFromRetrievedTime);
    }
    public void DEBUG_SetMockTimeOffset(float daysOffset, bool relativeToCurrentOffset = true)
    {
        _DEBUG_useMockTimeOffset = true;
        if (relativeToCurrentOffset)
            _DEBUG_daysOffsetFromRetrievedTime += daysOffset;
        else
            _DEBUG_daysOffsetFromRetrievedTime = daysOffset;
        RetrieveServerTime();
    }
    public void DEBUG_ResetMockTimeOffset()
    {
        _DEBUG_useMockTimeOffset = false;
        _DEBUG_daysOffsetFromRetrievedTime = 0F;
        RetrieveServerTime();
    }
#endregion

    public string ToDebugString()
    {
        string result = "Last Server Time Retrieve Successful: " + lastServerTimeUpdateSuccessful;
        if (lastServerTimeUpdateSuccessful)
        {
            result += "\nLast Server Time Universal: " + lastServerTimeUniversal;
            result += "\nLast Server Time Local: " + lastServerTimeLocal;
            result += "\nCurrent Device Time Universal: " + currentDeviceTimeUniversal;
            result += "\nCurrent Device Time Local: " + currentDeviceTimeLocal;
            result += "\nDEBUG: Use Mock Time Offset: " + _DEBUG_useMockTimeOffset.ToBoolString();
            if (_DEBUG_useMockTimeOffset)
            {
                result += "\nDEBUG: Days Offset From Retrieved Time: " + _DEBUG_daysOffsetFromRetrievedTime;    
            }
        }
        return result;
    }
}
