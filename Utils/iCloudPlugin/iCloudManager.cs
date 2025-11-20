using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Text;

namespace Replay.Utils
{
    public class iCloudManager : Singleton<iCloudManager>, IDebugLoggable
    {
        public string logTag => "iCloud";

        private string deviceId;

        // Default buffer size for string operations
        private const int DEFAULT_BUFFER_SIZE = 4096;

        public string DeviceId => deviceId;

        #if UNITY_IOS
        [DllImport("__Internal")]
        private static extern bool _SaveToiCloud(string key, string value);

        [DllImport("__Internal")]
        private static extern bool _SynchronizeToiCloud();

        [DllImport("__Internal")]
        private static extern bool _LoadFromiCloudToBuffer(string key, [Out] byte[] buffer, int bufferSize);

        [DllImport("__Internal")]
        private static extern int _GetRequiredBufferSizeForKey(string key);

        [DllImport("__Internal")]
        private static extern bool _DeleteFromiCloud(string key);

        [DllImport("__Internal")]
        private static extern bool _GetAllKeysFromiCloudToBuffer([Out] byte[] buffer, int bufferSize);

        [DllImport("__Internal")]
        private static extern int _GetRequiredBufferSizeForAllKeys();
        #endif

        private void Awake()
        {
            InitializeDeviceId();
        }

        private string GetOrCreateKeychainDeviceId()
        {
            #if UNITY_IOS && !UNITY_EDITOR
            // Try to get existing ID from keychain
            string keychainKey = "com.replay.wordskirmish.device_id";
            string existingId = KeychainPlugin.GetString(keychainKey);

            if (string.IsNullOrEmpty(existingId))
            {
                // Create new ID if none exists
                existingId = Guid.NewGuid().ToString();
                KeychainPlugin.SetString(keychainKey, existingId);
            }

            return existingId;
            #else
            return SystemInfo.deviceUniqueIdentifier;
            #endif
        }

        private void InitializeDeviceId()
        {
            string persistentId = GetOrCreateKeychainDeviceId();
            deviceId = persistentId;

            this.DevLog($"Using device ID: {deviceId}");
        }

        private string GetDeviceSpecificKey(string key)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                InitializeDeviceId();
            }
            return $"{deviceId}_{key}";
        }

        public bool SaveToCloud(string key, string value)
        {
            string deviceKey = GetDeviceSpecificKey(key);
            #if UNITY_IOS && !UNITY_EDITOR
            bool success = _SaveToiCloud(deviceKey, value);
            if  (success)
                this.Log($"Save succeeded: {deviceKey}");
            else
                this.LogError($"Save failed: {deviceKey}");
            return success;
            #else
            this.DevLog($"Would save: {deviceKey} = {value}");
            return true;
            #endif
        }

        public bool Synchronze()
        {
#if UNITY_IOS && !UNITY_EDITOR
            bool success = _SynchronizeToiCloud();
            if (success)
                this.Log("Synchronize succeeded");
            else
                this.LogError("Synchronize failed");

            return success;
#else
            this.DevLog($"Would synchronize");
            return true;
#endif
        }

        public (bool success, string value) LoadFromCloud(string key, bool addDevicePrefix = true)
        {
            string deviceKey = addDevicePrefix ? GetDeviceSpecificKey(key) : key;
            #if UNITY_IOS && !UNITY_EDITOR
            // First get the required buffer size
            int bufferSize = _GetRequiredBufferSizeForKey(deviceKey);
            if (bufferSize <= 0)
            {
                this.Log($"Key not found: {deviceKey}");
                return (false, null);
            }

            // Allocate a buffer of the required size
            byte[] buffer = new byte[bufferSize];

            // Load the value into the buffer
            bool success = _LoadFromiCloudToBuffer(deviceKey, buffer, bufferSize);
            if (!success)
            {
                this.LogError($"Failed to load value for key: {deviceKey}");
                return (false, null);
            }

            // Convert the buffer to a string
            string value = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            if (success)
                this.Log($"Load succeeded: {deviceKey}");
            else
                this.LogError($"Load failed: {deviceKey}");
            
            return (success, value);
            #else
            this.DevLog($"Would load: {deviceKey}");
            return (false, null);
            #endif
        }


        public bool DeleteFromCloud(string key)
        {
            string deviceKey = GetDeviceSpecificKey(key);
            #if UNITY_IOS && !UNITY_EDITOR
            bool success = _DeleteFromiCloud(deviceKey);
            if (success)
                this.Log($"Delete succeeded: {deviceKey}");
            else
                this.LogError($"Delete failed: {deviceKey}");
            return success;
            #else
            this.DevLog($"Would delete: {deviceKey}");
            return true;
            #endif
        }

        public (bool success, string[] keys) GetAllCloudKeys()
        {
            #if UNITY_IOS && !UNITY_EDITOR
            // First get the required buffer size
            int bufferSize = _GetRequiredBufferSizeForAllKeys();
            if (bufferSize <= 1) // Just a null terminator means no keys
            {
                this.Log("No keys found");
                return (true, new string[0]);
            }

            // Allocate a buffer of the required size
            byte[] buffer = new byte[bufferSize];

            // Load the keys into the buffer
            bool success = _GetAllKeysFromiCloudToBuffer(buffer, bufferSize);
            if (!success)
            {
                this.LogError("Failed to get all keys");
                return (false, new string[0]);
            }

            // Convert the buffer to a string and split by commas
            string keysString = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            string[] allKeys = !string.IsNullOrEmpty(keysString) ? keysString.Split(',') : new string[0];
            if (success)
                this.Log($"GetAllCloudKeys succeeded. Found {allKeys.Length} keys");
            else
                this.LogError("GetAllCloudKeys failed");

            return (success, allKeys);
            #else
            this.DevLog("Would get all cloud keys");
            return (false, new string[0]);
            #endif
        }

        public (bool success, string[] keys) GetDeviceCloudKeys(string forDeviceId = null)
        {
            string targetDeviceId = forDeviceId ?? deviceId;

            var (success, allKeys) = GetAllCloudKeys();
            if (!success) 
                return (false, new string[0]);

            // Filter keys for specified device
            string prefix = targetDeviceId + "_";
            string[] deviceKeys = allKeys
                .Where(k => k.StartsWith(prefix))
                .Select(k => k[prefix.Length..])
                .ToArray();

            if (success)
                this.Log($"GetDeviceCloudKeys succeeded for device {targetDeviceId}. Found {deviceKeys.Length} keys");
            else
                this.LogError($"GetDeviceCloudKeys failed for device {targetDeviceId}");
            
            return (success, deviceKeys);
        }

        public bool ClearAllCloudData(bool forThisDeviceOnly = true)
        {
            #if UNITY_IOS && !UNITY_EDITOR
            bool allDeleted = true;

            // Get ALL keys without device filtering
            var (success, allKeys) = GetAllCloudKeys();
            if (!success) return false;

            // Delete iCloud keys based on forThisDeviceOnly parameter
            foreach (string fullKey in allKeys)
            {
                if (forThisDeviceOnly)
                {
                    // Only delete keys that start with current device ID
                    string prefix = deviceId + "_";
                    if (!fullKey.StartsWith(prefix))
                        continue;
                }

                if (!_DeleteFromiCloud(fullKey))
                {
                    allDeleted = false;
                    this.LogError($"Failed to delete key: {fullKey}");
                }
            }

            // Always clear Keychain device ID since it's device-specific
            string keychainKey = "com.replay.wordskirmish.device_id";
            bool keychainDeleted = KeychainPlugin.DeleteKey(keychainKey);
            if (!keychainDeleted)
            {
                allDeleted = false;
                this.LogError("Failed to delete Keychain device ID");
            }
            else
            {
                // clear the local deviceId
                deviceId = null;
            }

            if (allDeleted)
            {
                this.Log($"Clear {(forThisDeviceOnly ? "device" : "all")} iCloud data and Keychain succeeded");
            }
            else
            {
                this.LogError($"Clear {(forThisDeviceOnly ? "device" : "all")} iCloud data and Keychain failed");
            }
            
            return allDeleted;
            #else
            this.DevLog($"Would clear {(forThisDeviceOnly ? "device" : "all")} iCloud data and Keychain");
            return true;
            #endif
        }
    }
}
