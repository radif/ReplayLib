using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Replay.Utils
{
    public static class LocalSerializerExtensions
    {
        public static bool SetList<T>(this LocalSerializer serializer, string key, List<T> values)
        {
            bool retVal = false;
            try
            {
                string jsonString = JsonConvert.SerializeObject(values, Formatting.None);
                serializer.SetString(key, jsonString);
                retVal = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                retVal = false;
            }

            return retVal;
        }
        
        public static bool SetHashSet<T>(this LocalSerializer serializer, string key, HashSet<T> values)
        {
            bool retVal = false;
            try
            {
                string jsonString = JsonConvert.SerializeObject(values, Formatting.None);
                serializer.SetString(key, jsonString);
                retVal = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                retVal = false;
            }

            return retVal;
        }

        public static HashSet<T> GetHashSet<T>(this LocalSerializer serializer, string key, HashSet<T> defaultValue = null)
        {
            string jsonString = serializer.GetString(key, null);
            if (!string.IsNullOrEmpty(jsonString))
            {
                HashSet<T> retVal = new();
                try
                {
                    JsonConvert.PopulateObject(jsonString, retVal);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                return retVal;
            }
            return defaultValue;
        }
        
    }
}