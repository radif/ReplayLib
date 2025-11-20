using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Replay.Utils
{
    public static class StringUtils
    {
        public const string kUppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string kLowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
        public const string kAlphanumbericSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        //Time Formatting
        // public static string TimerFriendlyStringFromSeconds(double timeInSeconds)
        // {
        //     TimeSpan currentTime = TimeSpan.FromSeconds(timeInSeconds);
        //     //var text = string.Format("{0:D2}:{1:D2}", currentTime.Minutes, currentTime.Seconds);
        //     var text = string.Format("{0}:{1:D2}", currentTime.Minutes, currentTime.Seconds);
        //     return text;
        // }
        public static string FormatTimeSpan(TimeSpan timeSpan, bool includeHours = false, bool includeMilliseconds = false)
        {
            if (includeHours)
                return string.Format("{0:D2}:{1:D2}:{2:D2}{3}", 
                    timeSpan.Hours, 
                    timeSpan.Minutes, 
                    timeSpan.Seconds,
                    includeMilliseconds ? $".{timeSpan.Milliseconds:D3}" : "");
            
            return string.Format("{0}:{1:D2}{2}", 
                timeSpan.Minutes, 
                timeSpan.Seconds,
                includeMilliseconds ? $".{timeSpan.Milliseconds:D3}" : "");
        }
        public static string TimerFriendlyStringFromSeconds(double timeInSeconds, bool includeHours = false, bool includeMilliseconds = false)
        {
            var timeSpan = TimeSpan.FromSeconds(timeInSeconds);
            return FormatTimeSpan(timeSpan, includeHours, includeMilliseconds);
        }
        public static string TimerFriendlyStringFromMilliSeconds(long timeInMillisecondsSeconds)
        {
            var timeSpan = TimeSpan.FromMilliseconds(timeInMillisecondsSeconds);
            
            bool includeHours = timeSpan.TotalHours >= 1;
            
            return FormatTimeSpan(timeSpan, includeHours, true);
        }
        
        public static string TimerFriendlySecondsStringFromMilliSeconds(long timeInMillisecondsSeconds)
        {
            var timeSpan = TimeSpan.FromMilliseconds(timeInMillisecondsSeconds);
            string retVal;

            if (timeSpan.Hours > 0)
            {
                retVal = string.Format("{0}:{1:D2}:{2:D2}.{3:D3}",
                    timeSpan.Hours,
                    timeSpan.Minutes,
                    timeSpan.Seconds,
                    timeSpan.Milliseconds);
            }
            else
            {
                if (timeSpan.Minutes == 0)
                    retVal = $"{timeSpan.Seconds}.{timeSpan.Milliseconds:D3}s";
                else
                    retVal = $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
            }
            
            return retVal;
        }

        //local numbers
        public static string LocalCultureFormattedNumber(double number) => string.Format("{0:N0}", number, CultureInfo.CurrentCulture);
        public static string LocalCultureFormattedNumber(float number) => string.Format("{0:N0}", number, CultureInfo.CurrentCulture);
        public static string LocalCultureFormattedNumber(int number) => string.Format("{0:N0}", number, CultureInfo.CurrentCulture);
        public static string LocalCultureFormattedNumber(long number) => string.Format("{0:N0}", number, CultureInfo.CurrentCulture);
        
        //parse

        public static void SetInvariantCultureGlobally()
        {
            var culture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;   
        }

        public static int GetInt(string s, int defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;

            int retVal = defaultValue;
            if (int.TryParse(s, numberStyles, cultureInfo, out var res))
                retVal = res;
            
            return retVal;
        }
        public static long GetLong(string s, long defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;

            long retVal = defaultValue;
            if (long.TryParse(s, numberStyles, cultureInfo, out var res))
                retVal = res;
            
            return retVal;
        }
        public static float GetFloat(string s, float defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;

            float retVal = defaultValue;
            if (float.TryParse(s, numberStyles, cultureInfo, out var res))
                retVal = res;

            return retVal;
        }

        public static double GetDouble(string s, double defaultValue = 0, NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            if (cultureInfo == null)
                cultureInfo = CultureInfo.InvariantCulture;

            double retVal = defaultValue;
            if (double.TryParse(s, numberStyles, cultureInfo, out var res))
                retVal = res;

            return retVal;
        }

        public static bool GetBool(string s, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            bool retVal = defaultValue;
            if (bool.TryParse(s, out var res))
                retVal = res;

            return retVal;
        }
        public static decimal GetDecimal(string s, decimal defaultValue = 0, 
            NumberStyles numberStyles = NumberStyles.None, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;

            cultureInfo ??= CultureInfo.InvariantCulture;

            return decimal.TryParse(s, numberStyles, cultureInfo, out var result) 
                ? result 
                : defaultValue;
        }

        public static T GetEnum<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return Enum.TryParse<T>(value, true, out var result) 
                ? result 
                : defaultValue;
        }
        public static string NewGUID() => Guid.NewGuid().ToString(); //new Guid().ToString();
        
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool isAlphanumeric(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            foreach (char c in input)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }
            return true;
        }
        public static bool ContainsOnlyLetters(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            foreach (char c in input)
            {
                if (!char.IsLetter(c))
                    return false;
            }
            return true;
        }

        public static bool ContainsOnlyDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
        public static string TruncateWithEllipsis(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength - 3) + "...";
        }

        public static string RemoveSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return System.Text.RegularExpressions.Regex.Replace(input, "[^a-zA-Z0-9]+", "", 
                System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        public static string GenerateRandomString(int length)
        {
            if (length <= 0) return string.Empty;
        
            var random = new System.Random();
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = kAlphanumbericSymbols[random.Next(kAlphanumbericSymbols.Length)];
            }
            return new string(chars);
        }
        
        public static string EscapeDataString(string value) =>
            Uri.EscapeDataString(value);

        public static string UnescapeDataString(string value) =>
            Uri.UnescapeDataString(value);
        
        
        //Replay String Notation
        public static string ConvertReplayNotationToHTMLTaggedString(this string replayNotationString)
        {
            if (string.IsNullOrEmpty(replayNotationString)) return replayNotationString;
            string htmlTaggedString = Regex.Replace(replayNotationString, @"\*(\w+)", "<b>$1</b>", RegexOptions.Compiled);
            return htmlTaggedString;
        }
        
        public static string ToNullableString(this object obj) => obj == null ? "null" : obj.ToString();
    }
}