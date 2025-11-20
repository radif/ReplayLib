using System;
using System.Globalization;
using UnityEngine.Networking;

namespace Replay.Utils
{
    public static class StringExtensions
    {
        public static string ShuffleCharacters(this string str)
        {
            char[] array = str.ToCharArray();
            System.Random rng = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        public static bool Empty(this string str) => str.Length == 0;
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        public static bool IsEmptyOrNullOrWhiteSpace(this string str) => string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        public static string IsInterned(this string str) => string.IsInterned(str);
        public static string GetNAOrString(this string str) => string.IsNullOrEmpty(str) ? "N/A" : str;


        //parse
        public static int IntValue(this string s, int defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null) => StringUtils.GetInt(s, defaultValue, numberStyles, cultureInfo);
        public static long LongValue(this string s, long defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null) => StringUtils.GetLong(s, defaultValue, numberStyles, cultureInfo);
        public static float FloatValue(this string s, float defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null) => StringUtils.GetFloat(s, defaultValue, numberStyles, cultureInfo);
        public static double DoubleValue(this string s, double defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null) => StringUtils.GetDouble(s, defaultValue, numberStyles, cultureInfo);
        public static bool BoolValue(this string s, bool defaultValue = false) => StringUtils.GetBool(s, defaultValue);

        /// <summary>
        /// Safely converts a string to a DateTime object. Returns null if the string is null, empty, or invalid.
        /// Uses InvariantCulture for ISO 8601 date parsing with fallback to default parsing.
        /// </summary>
        public static DateTime? ToDateTime(this string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return null;

            try
            {
                if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
                    return result;

                if (DateTime.TryParse(dateTimeString, out result))
                    return result;

                Dev.LogWarning($"Failed to parse datetime string: {dateTimeString}");
                return null;
            }
            catch (Exception e)
            {
                Dev.LogError($"Exception parsing datetime string '{dateTimeString}': {e.Message}");
                return null;
            }
        }


        //char support
        public static char ToUpper(this char c) => c.ToString().ToUpper().ToCharArray()[0];
        
        //title case
        public static string ToTitleCase(this string str, string clutureName = "en-US")
        {
            TextInfo textInfo = new CultureInfo(clutureName, false).TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }
        
        //WWW support
        public static string ToEscapeURL(this string URL) => UnityWebRequest.EscapeURL(URL).Replace("+", "%20");
        public static string ToEscapeDataString(this string s) => 
            StringUtils.EscapeDataString(s);
        public static string ToUnescapeDataString(this string s) => 
            StringUtils.UnescapeDataString(s);

        public static string ToBracketedString(this string s) 
            => "[" + s + "]";
    }

}

