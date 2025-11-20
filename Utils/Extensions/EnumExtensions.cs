using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public static class EnumExtensions
    {
        public static string ConvertToString(this Enum enumValue) => Enum.GetName(enumValue.GetType(), enumValue);
        public static EnumType ConvertToEnum<EnumType>(this string enumValue)  => (EnumType) Enum.Parse(typeof(EnumType), enumValue);

        public static int intValue(this Enum enumValue) => (int)(object)enumValue;

        public static int ValueIndex<T>(this T enumValue) where T : Enum
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            return Array.IndexOf(array, enumValue);
        }
        
        public static bool IsLast<T>(this T enumValue) where T : Enum
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            int i = Array.IndexOf(array, enumValue);
            return i >= array.Length - 1;
        }

        public static bool IsFirst<T>(this T enumValue) where T : Enum
        {
            int i = ValueIndex(enumValue);
            return i <= 0;
        }
        public static T Next<T>(this T enumValue) where T : Enum
        {
            //return (T)(((int)enumValue + 1) % Enum.GetValues(typeof(T)).Length);

            T[] array = (T[])Enum.GetValues(typeof(T));
            int i = Array.IndexOf(array, enumValue) + 1;
            return (i >= array.Length) ? array[0] : array[i];
        }

        public static T Previous<T>(this T enumValue) where T : Enum
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            int i = Array.IndexOf(array, enumValue) - 1;
            return (i < 0) ? array[array.Length - 1] : array[i];
        }
    }
}