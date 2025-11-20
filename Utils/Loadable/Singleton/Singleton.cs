using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _Instance = null;
        public static T Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new T();
                    _Instance._LoadingSequenceImpl();
                    _Instance.Init();
                }
                    
                
                return _Instance;
            }
        }

        public static T WeakInstance { get => IsLoaded ? Instance : null; }

        public static bool IsLoaded
        {
            get => _Instance != null;
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
        
        public static void Unload()
        {
            if (IsLoaded)
            {
                _Instance.Deinit();
                _Instance = null;
            }
                
            
        }
        //protected:
        protected virtual void Init() { }
        protected virtual void Deinit() { }
        
        
        //startup
        private void _LoadingSequenceImpl()
        {
            SingletonUtils.HandleStartupInterfaceSupport(this);
        }
        
        protected static string GetClassName() => ClassExtensions.GetClassName<T>();
        
#if FINALIZER_LOGS_ENABLED
        ~Singleton()
        {
            Debug.LogWarning("Finalizer: Singleton: "+ GetClassName());
        }
#endif
    }
}


