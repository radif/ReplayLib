using UnityEngine;
using System;

namespace Replay.Utils
{
    public abstract class ComponentPrefab<T> : MonoBehaviour, ILoadable where T : ComponentPrefab<T>
    {
        public static T Create(Transform parent = null, string pathOrIdentifierOverride = null)
        {
            T retVal = null;
            //1. Check if resource path is provided
            string resourcePath = string.IsNullOrEmpty(pathOrIdentifierOverride) ? _GetStringValueIfAny("ResourcePath") : pathOrIdentifierOverride;
            if (!string.IsNullOrEmpty(resourcePath))
                retVal = LoadFromResources(resourcePath, parent);

            //2. Check if Addressable identifier is provided
            if (retVal == null)
            {
                string addressablesIdentifier = string.IsNullOrEmpty(pathOrIdentifierOverride) ? _GetStringValueIfAny("AddressablesIdentifier") : pathOrIdentifierOverride;
                if (!string.IsNullOrEmpty(addressablesIdentifier))
                    retVal = LoadFromAddressables(addressablesIdentifier, parent);
            }
            return retVal;
        }

        public static T LoadFromResources(string resourcePath, Transform parent = null)
        {
            var prefab = Resources.Load<T>(resourcePath);
            T retVal = Instantiate(prefab);
            if (retVal != null)
            {
                if (parent != null)
                    retVal.transform.SetParent(parent, false);
            }
            return retVal;
        }

        public static T LoadFromAddressables(string addressablesIdentifier, Transform parent = null)
        {
            throw new NotImplementedException();
            //return null;
        }
        protected static string _GetStringValueIfAny(string methodName)
        {
            Func<bool, string> f = (bool isBaseType) => {
                string retVal = null;
                Type type = isBaseType ? typeof(T).BaseType : typeof(T);
                var staticPrefabOverrideMethod = type?.GetMethod(methodName);
                if (staticPrefabOverrideMethod != null)
                    retVal = (string)staticPrefabOverrideMethod.Invoke(null, Array.Empty<object>());
                return retVal;
            };


            //checking the base type
            string retVal = f(true);

            //checking the target type
            if (string.IsNullOrEmpty(retVal))
                retVal = f(false);

            return retVal;
        }
        
        protected static string GetClassName() => ClassExtensions.GetClassName<T>();
        
#if FINALIZER_LOGS_ENABLED
        ~ComponentPrefab()
        {
            Debug.LogWarning("Finalizer: ComponentPrefab: "+ GetClassName());
        }
#endif
    }
}

