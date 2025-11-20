using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Replay.Utils
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject, ILoadable where T : ScriptableObjectSingleton<T>
    {
        private static T _Instance = null;
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    LoadFromResources();
                }

                return _Instance;
            }
        }

        public static T WeakInstance { get => IsLoaded ? Instance : null; }

        public static bool IsLoaded { get => _Instance != null; }

        public static void SaveAssetInEditor(bool shouldUnload = true)
        {
#if UNITY_EDITOR
            if (IsLoaded)
            {
                EditorUtility.SetDirty(_Instance);
                AssetDatabase.SaveAssets();
                
                //unload and let it reload
                if(shouldUnload)
                    Unload();
            }
#else
            Dev.LogError("Cannot Save Asset \"" + GetClassName() + "\" when not in Unity Editor");
#endif
        }

        public static void Unload()
        {
            if (_Instance != null)
            {
                Resources.UnloadAsset(_Instance);
                _Instance = null;
                Resources.UnloadUnusedAssets();
            }
        }
        protected static string GetClassName() => ClassExtensions.GetClassName<T>();

        public void _AssignInstanceValue() => _Instance = (T)this;

        //protected virtual void OnEnable() => _LoadingSequenceImpl();

        private void _LoadingSequenceImpl()
        {
            _AssignInstanceValue();
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
        public static T LoadFromResources()
        {
            
            T[] assets = Resources.LoadAll<T>("");
            if (assets == null || assets.Length < 1)
            {
                throw new Exception("Could not find any scriptable object<" + GetClassName() +
                                    "> instances in the resources");
            }else if (assets.Length > 1)
            {
                Debug.LogWarning("Multiple Instances of the scriptable object<" + GetClassName() +
                                 "> found in the resources. The first one will be used, others will be unloaded");
            }
            T retVal = assets[0];
            
            // Unload unused assets
            for(int i = 1; i < assets.Length; i++)
            {
                Resources.UnloadAsset(assets[i]);
            }
            
            if (retVal != null)
            {
                retVal._LoadingSequenceImpl();
            }
            return retVal;
        }
        
#if FINALIZER_LOGS_ENABLED
        ~ScriptableObjectSingleton()
        {
            Debug.LogWarning("Finalizer: ScriptableObjectSingleton: "+ GetClassName());
        }
#endif
    }

}
