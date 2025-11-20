using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public class NetworkSerializer : Singleton<NetworkSerializer>, ISerializer
    {

        protected override void Init()
        {
            base.Init();
        }
        protected override void Deinit()
        {
            base.Deinit();
        }

        public void SetBool(string key, bool value)
        {
            LocalSerializer.Instance.SetBool(key, value);
            QueueSerialization(new NetworkValue(key, value));
        }
        public void SetInt(string key, int value) {
            LocalSerializer.Instance.SetInt(key, value);
            QueueSerialization(new NetworkValue(key, value));
        }
        public void SetLong(string key, long value) {
            LocalSerializer.Instance.SetLong(key, value);
            QueueSerialization(new NetworkValue(key, value));
        }
        public void SetFloat(string key, float value)
        {
            LocalSerializer.Instance.SetFloat(key, value);
            QueueSerialization(new NetworkValue(key, value));
        }
        public void SetDouble(string key, double value) {
            LocalSerializer.Instance.SetDouble(key, value);
            QueueSerialization(new NetworkValue(key, value));
        }
        public void SetString(string key, string value, bool saveToDeviceAccount = false) {
            LocalSerializer.Instance.SetString(key, value, saveToDeviceAccount);
            QueueSerialization(new NetworkValue(key, value));
        }

        public bool GetBool(string key) => LocalSerializer.Instance.GetBool(key);
        public bool GetBool(string key, bool defaultValue) => LocalSerializer.Instance.GetBool(key, defaultValue);
        
        public int GetInt(string key) => LocalSerializer.Instance.GetInt(key);
        public int GetInt(string key, int defaultValue) => LocalSerializer.Instance.GetInt(key, defaultValue);
        
        public long GetLong(string key) => LocalSerializer.Instance.GetLong(key);
        public long GetLong(string key, long defaultValue) => LocalSerializer.Instance.GetLong(key, defaultValue);


        public float GetFloat(string key) => LocalSerializer.Instance.GetFloat(key);
        public float GetFloat(string key, float defaultValue) => LocalSerializer.Instance.GetFloat(key, defaultValue);

        public double GetDouble(string key) => LocalSerializer.Instance.GetDouble(key);
        public double GetDouble(string key, double defaultValue) => LocalSerializer.Instance.GetDouble(key, defaultValue);

        public string GetString(string key) => LocalSerializer.Instance.GetString(key);
        public string GetString(string key, string defaultValue, bool loadFromDeviceAccount = false) => LocalSerializer.Instance.GetString(key, defaultValue, loadFromDeviceAccount);



        //network

        class NetworkValue{
            public string key;

            public enum ValueType { valueBool, valueInt, valueFloat, valueDouble, valueString }

            public ValueType valueType;

            public bool boolValue = default;
            public int intValue = default;
            public float floatValue = default;
            public double doubleValue = default;
            public string stringValue = default;

            public NetworkValue(string key, bool value) { this.key = key; valueType = ValueType.valueBool; boolValue = value; }
            public NetworkValue(string key, int value) { this.key = key; valueType = ValueType.valueInt; intValue = value; }
            public NetworkValue(string key, float value) { this.key = key; valueType = ValueType.valueFloat; floatValue = value; }
            public NetworkValue(string key, double value) { this.key = key; valueType = ValueType.valueDouble; doubleValue = value; }
            public NetworkValue(string key, string value) { this.key = key; valueType = ValueType.valueString; stringValue = value; }

            static NetworkValue CreateWithJSON(string jsonString)
            {
                return JsonUtility.FromJson<NetworkValue>(jsonString);
            }

            public string ToJSON()
            {
                string jsonString = JsonUtility.ToJson(this);
                return jsonString;

            }
        }
        List<NetworkValue> _networkQueue = new List<NetworkValue>();

        //TODO: queue network
        void QueueSerialization(NetworkValue networkValue)
        {
            _networkQueue.Add(networkValue);
        }

        public void ProcessQueue()
        {
            //command

            _networkQueue.Clear();
        }
        
        public void DeleteAll(bool deleteFromDeviceAccount = true)
        {
            LocalSerializer.Instance.DeleteAll(deleteFromDeviceAccount);
        }

        public void DeleteKey(string key, bool deleteFromDeviceAccount = true)
        {
            LocalSerializer.Instance.DeleteKey(key, deleteFromDeviceAccount);
        }
        
    }
}