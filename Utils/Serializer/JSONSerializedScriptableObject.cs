using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Replay.Utils
{
    public abstract class JSONSerializedScriptableObject : ScriptableObject {
        
        public Action<JSONSerializedScriptableObject> onDataChanged;

        public enum JSONLibrary { Undefined, Unity, Newtonsoft }

        public virtual string dataKey => null;

        public virtual JSONLibrary preferredJSONLibrary => JSONLibrary.Undefined;

        public virtual string ToJSON(bool prettyJSON = true) => ToJSON(preferredJSONLibrary, prettyJSON);
        public virtual string ToJSON(JSONLibrary jsonLibrary, bool prettyJSON = true)
        {
            if(jsonLibrary == JSONLibrary.Unity)
                return JsonUtility.ToJson(this, prettyJSON);
            else if(jsonLibrary == JSONLibrary.Newtonsoft)
                return JsonConvert.SerializeObject(this, prettyJSON ? Formatting.Indented : Formatting.None,
                new JsonSerializerSettings() { ContractResolver = new IgnorePropertiesResolver(new[] { "name", "hideFlags", "onDataChanged", "dataKey",  "preferredJSONLibrary"}) });

            //undefined library
            return null;
        }

        public virtual bool PopulateFromJSON(string jsonString) => PopulateFromJSON(jsonString, preferredJSONLibrary);
        public virtual bool PopulateFromJSON(string jsonString, JSONLibrary jsonLibrary)
        {
            bool retVal = true;
            try
            {
                if (jsonLibrary == JSONLibrary.Unity)
                    JsonUtility.FromJsonOverwrite(jsonString, this);
                else if (jsonLibrary == JSONLibrary.Newtonsoft)
                    JsonConvert.PopulateObject(jsonString, this,
                        new JsonSerializerSettings {
                            ContractResolver = new CollectionClearingContractResolver(),
                        });
                else if (jsonLibrary == JSONLibrary.Undefined)
                    retVal = false;
            }
            catch (Exception)
            {
                retVal = false;
            }

            if (retVal)
                onDataChanged?.Invoke(this);

            return retVal;
        }

        // Returns nested ScriptableObjects that should be marked as dirty when this object is updated
        public virtual ScriptableObject[] GetNestedScriptableObjects() => null;

    }
}
