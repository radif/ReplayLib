using UnityEngine;
using UnityEngine.SceneManagement;

namespace Replay.Utils
{
    public static class GameObjectExtensions
    {
        public static bool isDontDestroyOnLoadActivated(this GameObject gameObject)
        {
            bool retVal = false;
            if (gameObject != null)
                retVal = gameObject.scene.buildIndex == -1;//dontdestroy activated

            //Dev.Log("isDontDestroyOnLoadActivated on \"" + gameObject.name + "\": " + retVal);

            return retVal;
        }

        public static void MoveToMainScene(this GameObject gameObject) => gameObject.MoveToScene(0);
        public static void MoveToActiveScene(this GameObject gameObject) => SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        public static void MoveToScene(this GameObject gameObject, int sceneIndex) => SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneAt(sceneIndex));
        
        
        public static GameObject GetRootGameObject(this GameObject gameObject)
        {
            GameObject rootGameObject = null;
            if (gameObject != null)
            {
                Transform currentTransform = gameObject.transform;
                while (currentTransform.parent != null)
                    currentTransform = currentTransform.parent;
                rootGameObject = currentTransform.gameObject;
            }
            return rootGameObject;
        }
        public static GameObject[] GetRootGameObjectsInActiveScene() =>
            SceneManager.GetActiveScene().GetRootGameObjects();

        public static void SetActiveRecursively(this GameObject gameObject, bool value)
        {
            gameObject.SetActive(value);
            foreach (Transform child in gameObject.transform)
                child.gameObject.SetActiveRecursively(value);
        }
        
        public static void BroadcastMessageToRootObjectsInActiveScene(string methodName, object parameter = null)
        {
            GameObject[] rootGameObjects = GetRootGameObjectsInActiveScene();
            foreach (GameObject rootGameObject in rootGameObjects)
                rootGameObject.BroadcastMessage(methodName, parameter, SendMessageOptions.DontRequireReceiver);
        }
        public static void BroadcastMessageToRoot(this GameObject gameObject, string methodName, object parameter = null)
        {
            GameObject rootGameObject = gameObject.GetRootGameObject();
            if (rootGameObject != null)
                rootGameObject.BroadcastMessage(methodName, parameter, SendMessageOptions.DontRequireReceiver);
        }

        public static bool HasChildObject(this GameObject gameObject, GameObject other, bool recursive = true)
        {
            if (gameObject == null || other == null)
                return false;

            if (recursive)
            {
                foreach (Transform child in gameObject.transform)
                {
                    if (child.gameObject == other)
                        return true;
                
                    if (HasChildObject(child.gameObject, other, true))
                        return true;
                }
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                {
                    if (child.gameObject == other)
                        return true;
                }
            }
    
            return false;
        }
    }

}

