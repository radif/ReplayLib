using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public class LocalSerializer : Singleton<LocalSerializer>, ISerializer
    {
        // protected override void Init() {
        //     base.Init();
        // }
        // protected override void Deinit() {
        //     base.Deinit();
        // }

        public void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        
        public void SetLong(string key, long value) => PlayerPrefs.SetString(key, value.ToString());
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public void SetDouble(string key, double value) => PlayerPrefs.SetString(key, value.ToString());

        public void SetString(string key, string value, bool saveToDeviceAccount = false)
        {
            PlayerPrefs.SetString(key, value);
            if (saveToDeviceAccount)
            {
                if(Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    iCloudManager.Instance.SaveToCloud(key, value);
                }
            }
        }


        public bool GetBool(string key) => PlayerPrefs.GetInt(key) == 1;
        public bool GetBool(string key, bool defaultValue) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

        public int GetInt(string key) => PlayerPrefs.GetInt(key);
        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
        
        public long GetLong(string key) => GetLong(key, 0L);
        public long GetLong(string key, long defaultValue)
        {
            string stringValue = PlayerPrefs.GetString(key, defaultValue.ToString());
            if (long.TryParse(stringValue, out long result))
                return result;
            return defaultValue;
        }


        public float GetFloat(string key) => PlayerPrefs.GetFloat(key);
        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);

        public double GetDouble(string key) => StringUtils.GetDouble(PlayerPrefs.GetString(key));
        public double GetDouble(string key, double defaultValue) => StringUtils.GetDouble(PlayerPrefs.GetString(key, defaultValue.ToString()), defaultValue);

        public string GetString(string key) => PlayerPrefs.GetString(key);

        public string GetString(string key, string defaultValue, bool loadFromDeviceAccount = false)
        {
            if(PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key, defaultValue);
            
            string retVal = defaultValue;

            if (loadFromDeviceAccount)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    var (success, value) = iCloudManager.Instance.LoadFromCloud(key);
                    if (success)
                    {
                        retVal = value;
                    }
                }
            }
            return retVal;
        }

        public void DeleteAll(bool deleteFromDeviceAccount = true)
        {
            PlayerPrefs.DeleteAll();
            if (deleteFromDeviceAccount)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    iCloudManager.Instance.ClearAllCloudData();
                }
            }
        }
        
        public void DeleteKey(string key, bool deleteFromDeviceAccount = true)
        {
            PlayerPrefs.DeleteKey(key);
            if (deleteFromDeviceAccount)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    iCloudManager.Instance.DeleteFromCloud(key);
                }
            }
        }

        public void Serialize()
        {
            PlayerPrefs.Save();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                iCloudManager.Instance.Synchronze();
            }
        }
    }
}