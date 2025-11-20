using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Replay.Utils
{
    public static class CollectionUtils
    {
        //is empty
        public static bool IsEmpty<T>(this List<T> list) => list == null || list.Count == 0;
        public static bool IsEmpty<T>(this T[] array) => array == null || array.Length == 0;
        public static bool IsEmpty<T>(this HashSet<T> set) => set == null || set.Count == 0;

        //random element
        static System.Random _systemRandom = new ();
        public static T PickAny<T>(this List<T> list) => list[Random.Range(0, list.Count)];

        public static T PickAnyExcept<T>(this List<T> list, params T[] exceptions)
        {
            var retVal = list.PickAny();
            while (exceptions.Contains(retVal) && list.Count > exceptions.Length)
                retVal = list.PickAny();
            return retVal;
        }
        public static T PickAnyThreadSafe<T>(this List<T> list) => list[_systemRandom.Next(list.Count)];  

        public static T PickAny<T>(this T[] array) => array[Random.Range(0, array.Length)];
        public static T PickAnyExcept<T>(this T[] array, params T[] exceptions)
        {
            var retVal = array.PickAny();
            while (exceptions.Contains(retVal) && array.Length > exceptions.Length)
                retVal = array.PickAny();
            return retVal;
        }
        public static T PickAnyThreadSafe<T>(this T[] array) => array[_systemRandom.Next(array.Length)];

        public static T PickAny<T>(this HashSet<T> set)
        {
            return set.ElementAt(Random.Range(0, set.Count));
            //int targetIndex = Random.Range(0, set.Count);
            //int index = 0;

            //foreach (var word in set)
            //{
            //    if (index == targetIndex)
            //        return word;
            //    index++;
            //}
            //return set.FirstOrDefault();
        }
        public static T PickAnyExcept<T>(this HashSet<T> set, params T[] exceptions)
        {
            var retVal = set.PickAny();
            while (exceptions.Contains(retVal) && set.Count > exceptions.Length)
                retVal = set.PickAny();
            return retVal;
        }

        //firstIndex
        public static int FirstIndex<T>(this List<T> list) => 0;  
        public static int FirstIndex<T>(this T[] array) => 0;

        
        //lastIndex
        public static int LastIndex<T>(this List<T> list) => list.Count - 1;  
        public static int LastIndex<T>(this T[] array) => array.Length - 1;
        
        //clampIndex
        public static int ClampIndex<T>(this List<T> list, int index) => Mathf.Clamp(index, list.FirstIndex(), list.LastIndex());
        public static int ClampIndex<T>(this T[] array, int index) => Mathf.Clamp(index, array.FirstIndex(), array.LastIndex());

        //flipIndex
        public static int FlipIndex<T>(this List<T> list, int index) => list.LastIndex() - index;
        public static int FlipIndex<T>(this T[] array, int index) => array.LastIndex() - index;

        //first and last elements
        public static T GetFirst<T>(this List<T> list) => list[0];
        public static T GetFirst<T>(this T[] array) => array[0];

        public static T GetLast<T>(this List<T> list) => list[^1];
        public static T GetLast<T>(this T[] array) => array[^1];

        //removeLast
        public static void RemoveLast<T>(this List<T> list) => list.RemoveAt(list.Count - 1);
        public static void RemoveLast<T>(this T[] array) => Array.Resize(ref array, array.Length - 1);
        
        //safe first and last elements
        public static T GetFirstOrNull<T>(this List<T> list) where T : class
        {
            if(!list.IsEmpty())
                return list.GetFirst();
            return null;
        }
        public static T GetFirstOrNull<T>(this T[] array) where T : class
        {
            if (!array.IsEmpty())
                return array.GetFirst();
            return null;
        }

        public static T GetLastOrNull<T>(this List<T> list) where T : class
        {
            if (!list.IsEmpty())
                return list.GetLast();
            return null;
        }
        public static T GetLastOrNull<T>(this T[] array) where T : class
        {
            if (!array.IsEmpty())
                return array.GetLast();
            return null;
        }

        //safe access at index
        public static T AtOrNull<T>(this List<T> list, int index) where T : class
        {
            if (index >= 0 && index < list.Count)
                return list[index];
            return null;
        }
        public static T AtOrNull<T>(this T[] array, int index) where T : class
        {
            if (index >= 0 && index < array.Length)
                return array[index];
            return null;
        }

        public static T AtOrDefault<T>(this List<T> list, int index, T defaultValue)
        {
            if (index >= 0 && index < list.Count)
                return list[index];
            return defaultValue;
        }
        public static T AtOrDefault<T>(this T[] array, int index, T defaultValue)
        {
            if (index >= 0 && index < array.Length)
                return array[index];
            return defaultValue;
        }

        //join an Array or a List
        public static string Join<T>(this T[] array, string delimiter)
        {
            string retVal = "";
            int count = array.Length;
            for (int i = 0; i < count; ++i)
                retVal += array[i].ToString() + (i == count - 1 ? "" : delimiter);

            return retVal;
        }
        public static string Join<T>(this List<T> list, string delimiter)
        {
            string retVal = "";
            int count = list.Count;
            for (int i = 0; i < count; ++i)
                retVal += list[i].ToString() + (i == count - 1 ? "" : delimiter);

            return retVal;
        }

        //conversion
        public static List<T> ToList<T>(this T[] array)
        {
            List<T> retVal = new();
            retVal.AddRange(array);
            return retVal;
        }

        //bulk adding to hash
        public static void AddRange<T>(this HashSet<T> hashSet, List<T> list)
        {
            foreach (var element in list)
                hashSet.Add(element);
        }
        public static void AddRange<T>(this HashSet<T> hashSet, T[] array)
        {
            foreach (var element in array)
                hashSet.Add(element);
        }
        
        public static void AddRange<T>(this HashSet<T> hashSet, HashSet<T> h)
        {
            foreach (var element in h)
                hashSet.Add(element);
        }
        public static bool ContainsAllRange<T>(this HashSet<T> hashSet, T[] array)
        {
            foreach (var element in array)
                if (!hashSet.Contains(element))
                    return false;
            return true;
        }
        public static bool ContainsAllElementsInRange(this HashSet<int> hashSet, Range range)
        {
            int start = range.Start.Value;
            int end = range.End.Value;
            
            for (int i = start; i <= end; ++i)
                if (!hashSet.Contains(i))
                    return false;
            return true;
        }
        public static void AddRange<T>(this T[] array, HashSet<T> h)
        {
            var list = new List<T>();
            list.AddRange(array);
            list.AddRange(h);
            Array.Resize(ref array, list.Count);
            
            for (int i = 0; i< list.Count; ++i)
                array[i] = list[i];
        }
        
        
        
        public static void RemoveDuplicates<T>(this List<T> list)
        {
            var hashSet = new HashSet<T>();
            hashSet.AddRange(list);
            list.Clear();
            list.AddRange(hashSet);
        }
        
        public static void RemoveDuplicates<T>(this T[] array)
        {
            var hashSet = new HashSet<T>();
            hashSet.AddRange(array);
            Array.Resize(ref array, hashSet.Count);

            int i = 0;
            foreach (var element in hashSet)
            {
                array[i] = element;
                ++i;
            }
        }
    }
}
