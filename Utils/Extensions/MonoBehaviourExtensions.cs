using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class MonoBehaviourExtensions
    {

        //call "this.Delay(...)" in order to use this extension
        public static Coroutine Delay(this MonoBehaviour monoBehavior, float delayTime, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.DelayCoroutine(delayTime, action));
        public static Coroutine NextFrame(this MonoBehaviour monoBehavior, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.NextFrameCoroutine(action));
        public static Coroutine SkipFrames(this MonoBehaviour monoBehavior, int frameCount, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.SkipFramesCoroutine(frameCount, action));
        public static Coroutine NextFixedUpdate(this MonoBehaviour monoBehavior, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.NextFixedUpdateCoroutine(action));
        public static Coroutine WaitWhile(this MonoBehaviour monoBehavior, Func<bool> condition, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.WaitWhileCoroutine(condition, action));
        public static Coroutine WaitUntil(this MonoBehaviour monoBehavior, Func<bool> condition, Action action) => monoBehavior.StartCoroutine(CoroutineUtils.WaitUntilCoroutine(condition, action));

        //Stop And null Coroutine
        public static void StopAndNullCoroutine(this MonoBehaviour monoBehavior, ref Coroutine coroutine)
        {
            if (monoBehavior.IsLoaded() && coroutine != null)
            {
                try
                {
                    monoBehavior.StopCoroutine(coroutine);
                }
                catch (Exception e)
                {
                    Debug.LogError("StopAndNullCoroutine threw an exception: " + e.Message);
                }
            }
                
            coroutine = null;
        }

        //Canvas
        public static RectTransform GetRectTransform(this MonoBehaviour monoBehavior) => monoBehavior.GetComponent<RectTransform>();
    }
}

