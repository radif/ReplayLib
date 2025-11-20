using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    interface ISerializer
    {
        void SetBool(string key, bool value);
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetDouble(string key, double value);
        void SetString(string key, string value, bool saveToDeviceAccount = false);

        bool GetBool(string key);
        bool GetBool(string key, bool defaultValue);

        int GetInt(string key);
        int GetInt(string key, int defaultValue);

        float GetFloat(string key);
        float GetFloat(string key, float defaultValue);

        double GetDouble(string key);
        double GetDouble(string key, double defaultValue);

        string GetString(string key);
        string GetString(string key, string defaultValue, bool loadFromDeviceAccount = false);
        
        void DeleteAll(bool deleteFromDeviceAccount = true);
        void DeleteKey(string key, bool deleteFromDeviceAccount = true);
    }
}
