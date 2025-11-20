using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace Replay.Utils
{
    public class CoroutineUtils : ComponentSingleton<CoroutineUtils>, IDebugLoggable
    {
        [Preserve]
        public static bool ShouldCreateOwnGameObject() => true;
        [Preserve]
        public static bool ShouldEnableDontDestroyOnLoad() => true;
        //[Preserve]
        //public static bool ShouldNotRenameGameObject() => true;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() =>
            Load();
        
        //Coroutines
        public static IEnumerator DelayCoroutine(float delayTime, Action action) { yield return new WaitForSeconds(delayTime); action?.Invoke(); }
        public static IEnumerator NextFrameCoroutine(Action action) { yield return null; action?.Invoke(); }
        public static IEnumerator SkipFramesCoroutine(int frameCount, Action action)
        {
            for (int i = 0; i < frameCount; i++)
                yield return null; 
            action?.Invoke();
        }
        public static IEnumerator NextFixedUpdateCoroutine(Action action) { yield return new WaitForFixedUpdate(); action?.Invoke(); }
        public static IEnumerator WaitWhileCoroutine(Func<bool> condition, Action action) { yield return new WaitWhile(condition); action?.Invoke(); }
        public static IEnumerator WaitUntilCoroutine(Func<bool> condition, Action action) { yield return new WaitUntil(condition); action?.Invoke(); }

        //Factories (additional factories see in MonoBehaviorExtensions)
        public static Coroutine Delay(float delayTime, Action action) => Instance.StartCoroutine(DelayCoroutine(delayTime, action));
        public static Coroutine NextFrame(Action action) => Instance.StartCoroutine(NextFrameCoroutine(action));
        public static Coroutine SkipFrames(int frameCount, Action action) => Instance.StartCoroutine(SkipFramesCoroutine(frameCount, action));
        public static Coroutine NextFixedUpdate(Action action) =>Instance.StartCoroutine(NextFixedUpdateCoroutine(action));
    }

}
