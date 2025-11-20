using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public interface IReplaySingleton {
        public void _OnSingletonAwake();
        public void _OnSingletonStart();
        public void _OnSingletonDestroy();
    }

    /// <summary>
    /// Monitors and manages the lifecycle of a singleton component that implements IReplaySingleton.
    /// This class ensures proper initialization and cleanup of singleton objects.
    /// </summary>
    public class SingletonMonitor : MonoBehaviour
    {
        /// <summary>
        /// Reference to the singleton component being monitored.
        /// This component must implement IReplaySingleton interface.
        /// </summary>
        public IReplaySingleton _singleton = null;

        /// <summary>
        /// Finds and returns the first non-null component that implements IReplaySingleton on this GameObject.
        /// </summary>
        /// <returns>The first valid IReplaySingleton component found, or null if none exists.</returns>
        IReplaySingleton GetReplaySingleton()
        {
            var allComponents = GetComponents<IReplaySingleton>();
            foreach(var component in allComponents)
            {
                if (component != null)
                    return component;
            }
            return null;
        }

        /// <summary>
        /// Unity's Awake callback. Initializes the singleton reference and triggers its awake event.
        /// </summary>
        void Awake()
        {
            _singleton = GetReplaySingleton();
            _singleton?._OnSingletonAwake();
        }

        /// <summary>
        /// Unity's Start callback. Ensures singleton is initialized and triggers its start event.
        /// Acts as a fallback if singleton wasn't found during Awake.
        /// </summary>
        private void Start()
        {
            if(_singleton == null)
                _singleton = GetReplaySingleton();
            _singleton?._OnSingletonStart();
        }

        /// <summary>
        /// Unity's OnDestroy callback. Triggers singleton's destroy event if the GameObject
        /// is being destroyed normally (not through DontDestroyOnLoad).
        /// </summary>
        void OnDestroy()
        {
            if(this != null && gameObject != null && !gameObject.isDontDestroyOnLoadActivated())
                _singleton?._OnSingletonDestroy();
            _singleton = null;
        }
    }

}
