using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Replay.Utils
{
    public static class ComponentExtensions
    {
        //public static bool IsLoaded(this MonoBehaviour monoBehaviour)
        //{
        //    return monoBehaviour != null && monoBehaviour.gameObject != null;
        //}
        public static T LoadResourceFromPrefabFolder<T>(string folderPath, Transform parent = null) where T : MonoBehaviour
        {
            string className = typeof(T).Name;
            string path = Path.Combine(folderPath, className);

            var retVal = LoadResourceFromPrefab<T>(path, parent);

            return retVal;
        }
        public static T LoadResourceFromPrefab<T>(string path, Transform parent = null) where T : MonoBehaviour
        {
            var resource = Resources.Load<GameObject>(path);
            GameObject go = GameObject.Instantiate(resource, parent);

            var retVal = go.GetOrAddComponent<T>();
            
            return retVal;
        }

        //lifecycle
        public static bool IsLoaded(this Component component) => component != null && component.gameObject != null;
        

        public static void NullifyRefIfNotLoaded(ref Component cRef) { if (!cRef.IsLoaded()) cRef = null; }
        public static void NullifyRefIfNotLoaded(ref MonoBehaviour cRef) { if (!cRef.IsLoaded()) cRef = null; }


        public static void Unload(this Component component) => GameObject.Destroy(component.gameObject);
        

        //getters
        public static T GetOrAddComponent<T>(this Component component) where T : MonoBehaviour
        {
            var retVal = component.GetComponent<T>();
            if (retVal == null)
                retVal = component.gameObject.AddComponent<T>();
            return retVal;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var retVal = gameObject.GetComponent<T>();
            if (retVal == null)
                retVal = gameObject.AddComponent<T>();
            return retVal;
        }


        public static bool HasComponent<T>(this Component component) where T : MonoBehaviour => component.GetComponent<T>() != null;
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() != null;


        //naming
        public static bool FixOrAppendPrefabCloneSuffix(this Component component, string displayName) => FixOrAppendPrefabCloneSuffix(component.gameObject, displayName);
        
        public static bool FixOrAppendPrefabCloneSuffix(this GameObject gameObject, string nameSuffix)
        {
            if (!string.IsNullOrEmpty(nameSuffix))
            {
                const string cloneSuffix = "(Clone)";

                var name = gameObject.name;
                if (name.Contains(cloneSuffix, StringComparison.InvariantCultureIgnoreCase))
                    name = name.Replace(cloneSuffix, "");

                if (!name.Contains(nameSuffix, StringComparison.InvariantCultureIgnoreCase))
                    name += "(" + nameSuffix + ")";

                gameObject.name = name;

                return true;
            }
            return false;
        }

        public static bool AppendNameSuffix(this Component component, string baseName, string displayName) => AppendNameSuffix(component.gameObject, baseName, displayName);

        public static bool AppendNameSuffix(this GameObject gameObject, string baseName, string nameSuffix)
        {
            if (!string.IsNullOrEmpty(nameSuffix))
            {
                gameObject.name = baseName + "(" + nameSuffix + ")";
                return true;
            }
            return false;
        }

        public static void MoveToMainScene(this Component component) => component.gameObject.MoveToMainScene();
        public static void MoveToActiveScene(this Component component) => component.gameObject.MoveToActiveScene();

        public static void MoveToScene(this Component component, int sceneIndex) => component.gameObject.MoveToScene(sceneIndex);


    }
    

}

