using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class ListExtensions
    {
        public static void DestoryContentsAndClear<T>(this List<T> list) where T : MonoBehaviour
        {
            foreach (var o in list)
                GameObject.Destroy(o.gameObject);

            list.Clear();
        }
        //public static bool IsEmpty<T>(this List<T> list) where T : MonoBehaviour => list.Count > 0;


      
        public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                throw new ArgumentException("List count should be at least 2 for a swap.");

            T firstValue = list[firstIndex];

            list[firstIndex] = list[secondIndex];
            list[secondIndex] = firstValue;
        }

        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                Swap(list, randomIndex, i);
            }
        }

        
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            var state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);

            Shuffle(list);

            UnityEngine.Random.state = state;
        }

        
        public static void RotateLeft<T>(this IList<T> list, int count = 1)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            for (int current = 0; current < count; current++)
            {
                T first = list[0];
                list.RemoveAt(0);
                list.Add(first);
            }
        }

       
        public static void RotateRight<T>(this IList<T> list, int count = 1)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            int lastIndex = list.Count - 1;
            for (int current = 0; current < count; current++)
            {
                T last = list[lastIndex];
                list.RemoveAt(lastIndex);
                list.Insert(0, last);
            }
        }

       
        public static void RemoveNullEntries<T>(this IList<T> list) where T : class
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (Equals(list[i], null))
                    list.RemoveAt(i);
        }

       
        public static void RemoveDefaultValues<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (Equals(default(T), list[i]))
                    list.RemoveAt(i);
        }

       
        public static bool HasIndex<T>(this IList<T> list, int index) => index.InRange(0, list.Count - 1);
    }

}

