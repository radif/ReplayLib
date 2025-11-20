using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Replay.Utils
{
    [RequireComponent(typeof(SingletonMonitor))]
    public abstract class ComponentSingleton<T> : MonoBehaviour, IReplaySingleton, ILoadable where T : ComponentSingleton<T>
    {
        [SerializeField] bool _dontDestroyOnLoad = false;
        [Tooltip("This might be necessary when the game object is referred by the code or an animation")]
        [SerializeField] bool _dontRenameGameObject = false;
        
        public static string GetResourcePath() => _Instance.GetStringOrNullForStaticMethodName("ResourcePath");
        public static string GetAddressablesIdentifier() => _Instance.GetStringOrNullForStaticMethodName("AddressablesIdentifier");
        
        private static T _Instance = null;
        public static T Instance
        {
            get
            {
                if(_Instance == null)
                {

                    //1. Check if resource path is provided
                    string resourcePath = GetResourcePath();
                    if (!string.IsNullOrEmpty(resourcePath))
                        LoadFromResources(resourcePath);

                    //2. Check if Addressable identifier is provided
                    if (_Instance == null)
                    {
                        string addressablesIdentifier = GetAddressablesIdentifier();
                        if (!string.IsNullOrEmpty(addressablesIdentifier))
                            LoadFromAddressables(addressablesIdentifier);
                    }
                    //3. check if should create own game objec
                    if (_Instance == null && _Instance.GetBoolOrFalseForStaticMethodName("ShouldCreateOwnGameObject"))
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = GetClassName();
                        _Instance = (T)gameObject.AddComponent(typeof(T));
                        _Instance._dontDestroyOnLoad = _Instance.GetBoolOrFalseForStaticMethodName("ShouldEnableDontDestroyOnLoad");
                        _Instance._dontRenameGameObject = _Instance.GetBoolOrFalseForStaticMethodName("ShouldNotRenameGameObject");
                        //gameObject.AddComponent<SingletonMonitor>();
                    }
                    

                    //4. return null - this object must be part of a scene or a prefab
                }
                
                return _Instance;
            }
        }

        /// <summary>
        /// Call this method for preheating the prefab.
        /// This will speed up the instantiation in the future.
        /// Useful for smooth animations.
        /// </summary>
        /// <returns>True if the prefab was preheated, false if the prefab was already preheated or not found.</returns>
        public static bool WarmupPrefab()
        {
            if (IsLoaded)
                return false;
            
            T prefab = null;
            //1. Check if resource path is provided
            string resourcePath = GetResourcePath();
            if (!string.IsNullOrEmpty(resourcePath))
                prefab = GetPrefabFromResources(resourcePath);

            //2. Check if Addressable identifier is provided
            if (prefab == null)
            {
                string addressablesIdentifier = GetAddressablesIdentifier();
                if (!string.IsNullOrEmpty(addressablesIdentifier))
                    prefab = LoadFromAddressables(addressablesIdentifier);
            }
            return prefab != null;
        }
        
        public SingletonMonitor singletonMonitor => this.GetOrAddComponent<SingletonMonitor>();
        public static T WeakInstance { get => IsLoaded ? Instance : null; }

        public void _AssignInstanceValue() => _Instance = (T)this;

        public static void _NullifyInstanceValue() => _Instance = null;

        
        private void _LoadingSequenceImpl()
        {
            _AssignInstanceValue();
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            if(!_dontRenameGameObject)
                _FixGameObjectName();
            SingletonUtils.HandleStartupInterfaceSupport(this);
            OnSingletonInit();
        }

        private bool _teardownSequenceImplCalled = false;
        private void _TeardownSequenceImpl()
        {
            if (!_teardownSequenceImplCalled)
            {
                _teardownSequenceImplCalled = true;
                OnSingletonTeardown();
            }
        }

        private void _FixGameObjectName() => gameObject.FixOrAppendPrefabCloneSuffix("ComponentSingleton");
        

        public static bool IsLoaded {
            get => _Instance != null && _Instance.gameObject != null;
            set
            {
                if (value)
                {
                    if (!IsLoaded)
                        Load();
                }
                else
                {
                    if (IsLoaded)
                        Unload();
                }
            }
        }

        public static T Load()
        {
            return Instance;
        }

        public static T GetPrefabFromResources(string resourcePath)
        {
            var prefab = Resources.Load<T>(resourcePath);
            return prefab;
        }

        public static T LoadFromResources(string resourcePath, Transform parent = null)
        {
            var prefab = GetPrefabFromResources(resourcePath);
            if (prefab == null)
            {
                Debug.LogError("Cannot Load Prefab From Resources: " + resourcePath);
                return null;
            }
            
            T retVal = Instantiate(prefab);
            if (retVal != null)
            {
                retVal.singletonMonitor._singleton = retVal;
                retVal._LoadingSequenceImpl();
                if (parent != null)
                    retVal.transform.parent = parent;
            }
            return retVal;
        }

        // Dictionary to track all loaded asset operation handles
        private static readonly System.Collections.Generic.Dictionary<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject>> _addressableHandles = 
            new System.Collections.Generic.Dictionary<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject>>();
            
        // Release a specific addressable by its identifier
        public static void ReleaseAddressable(string addressablesIdentifier)
        {
            if (string.IsNullOrEmpty(addressablesIdentifier))
                return;
                
            if (_addressableHandles.TryGetValue(addressablesIdentifier, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"Released addressable: {addressablesIdentifier}");
                }
                
                _addressableHandles.Remove(addressablesIdentifier);
            }
        }
        
        // Release all addressables loaded by this singleton
        public static void ReleaseAllAddressables()
        {
            foreach (var kvp in _addressableHandles)
            {
                string id = kvp.Key;
                var handle = kvp.Value;
                
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"Released addressable during cleanup: {id}");
                }
            }
            
            _addressableHandles.Clear();
        }
        
        public static T GetPrefabFromAddressables(string addressablesIdentifier)
        {
            // Release any existing handle for this identifier first
            ReleaseAddressable(addressablesIdentifier);
            
            // Load the asset
            var op = Addressables.LoadAssetAsync<GameObject>(addressablesIdentifier);
            
            // Store the handle for later cleanup
            _addressableHandles[addressablesIdentifier] = op;
            
            if (!op.IsDone)
                op.WaitForCompletion();
            
            if (!op.IsValid())
            {
                Debug.LogError($"Failed to load addressable asset (operation invalid): {addressablesIdentifier}");
                _addressableHandles.Remove(addressablesIdentifier);
                return null;
            }
            
            var resultGO = op.Result;
            if (resultGO == null)
            {
                Debug.LogError($"Failed to load addressable asset's GameObject: {addressablesIdentifier}");
                Addressables.Release(op);
                _addressableHandles.Remove(addressablesIdentifier);
                return null;
            }
            
            var retValPrefab = resultGO.GetComponent<T>();
            if (retValPrefab == null)
            {
                Debug.LogError($"Failed to load addressable asset's Component: {addressablesIdentifier}");
                Addressables.Release(op);
                _addressableHandles.Remove(addressablesIdentifier);
                return null;
            }
            
            // Note: We don't release the handle here as we might need the asset later
            // It will be released when the singleton is unloaded or when ReleaseAddressable is called
            return retValPrefab;
        }
        public static T LoadFromAddressables(string addressablesIdentifier, Transform parent = null)
        {
            var prefab = GetPrefabFromAddressables(addressablesIdentifier);
            
            T retVal = Instantiate(prefab);
            
            if (retVal != null)
            {
                retVal.singletonMonitor._singleton = retVal;
                retVal._LoadingSequenceImpl();
                if (parent != null)
                    retVal.transform.parent = parent;
            }

            return retVal;
        }

        public static void Unload()
        {
            if (IsLoaded)
            {
                try
                {
                    // Teardown lifecycle
                    _Instance._TeardownSequenceImpl();
                    
                    // Destroy the instance
                    Destroy(_Instance.gameObject);
                    
                    // Remove the reference
                    _NullifyInstanceValue();
                    
                    // Release all addressable assets
                    ReleaseAllAddressables();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error during unload of {typeof(T).Name}: {e.Message}");
                    _NullifyInstanceValue();
                }
            }
        }
        
        public static void UnloadImmediately()
        {
            if (IsLoaded)
            {
                try
                {
                    var instance = Instance;
                    
                    // Teardown lifecycle
                    instance._TeardownSequenceImpl();
                    
                    // Destroy the instance immediately
                    DestroyImmediate(instance.gameObject);
                    
                    // Remove the reference
                    _NullifyInstanceValue();
                    
                    // Release all addressable assets
                    ReleaseAllAddressables();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error during immediate unload of {typeof(T).Name}: {e.Message}");
                    _NullifyInstanceValue();
                }
            }
        }

        public static void Reload()
        {
            UnloadImmediately();
            Load();
        }

        //this is not guaranteed if the monitor component is instantiated before this component
        public void _OnSingletonAwake()
        {
            _LoadingSequenceImpl();
        }
        public void _OnSingletonStart()
        {
            //self created objects have properties assigned later, therefore we need to init again
            _LoadingSequenceImpl();
        }
        public void _OnSingletonDestroy()
        {
            _TeardownSequenceImpl();
            _NullifyInstanceValue();
        }

        
        //optional init
        protected virtual void OnSingletonInit(){}
        
        //optional teardown
        protected virtual void OnSingletonTeardown() {}
        
        protected static string GetClassName() => ClassExtensions.GetClassName<T>();

#if FINALIZER_LOGS_ENABLED
        ~ComponentSingleton()
        {
            Debug.LogWarning("Finalizer: ComponentSingleton: "+ GetClassName());
        }
#endif
    }
}


