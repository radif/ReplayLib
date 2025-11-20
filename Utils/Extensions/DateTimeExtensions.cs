using System;

namespace Replay.Utils { 
    public static class DateTimeExtensions
    {
        public static int GetDaysBetween(this DateTime startDateTimeLocal, DateTime endDateTimeLocal, bool absolute = false)
        {
            // Normalize to midnight (remove time component)
            DateTime startDateLocal = startDateTimeLocal.Date;
            DateTime endDateLocal = endDateTimeLocal.Date;
    
            // Calculate days between the two dates
            TimeSpan span = endDateLocal - startDateLocal;
            int retVal = absolute ? Math.Abs(span.Days) : span.Days;
            return retVal;
        }
        
        public static string ToDisplayString(this DateTime dateTime)
        {
            //convert to format: SEPTEMBER 9, 2025
            return dateTime.ToString("MMMM d, yyyy");
        }
        public static string ToLoggerString(this DateTime dateTime, bool hasBrackets = true)
        {
            string retVal = dateTime.ToString("MMM-dd HH:mm:ss");
            if (hasBrackets)
                retVal = retVal.ToBracketedString();
            return retVal;
        }
        public static string ToDateString(this DateTime dateTime, bool hasBrackets = true)
        {
            string retVal = dateTime.ToString("MMM-dd");
            if (hasBrackets)
                retVal = retVal.ToBracketedString();
            return retVal;
        }
        public static string ToLoggerStringFull(this DateTime dateTime, bool hasYear = false, bool hasBrackets = true)
        {
            string timeZoneAbbr = "UTC";
            if (dateTime.Kind == DateTimeKind.Local)
            {
                TimeZoneInfo timeZone = TimeZoneInfo.Local;
                timeZoneAbbr = timeZone.IsDaylightSavingTime(dateTime) ? 
                    timeZone.DaylightName.Split(' ')[0] : 
                    timeZone.StandardName.Split(' ')[0];
            }
            
            string retVal = dateTime.ToString(hasYear ? "MMM-dd-yyyy HH:mm:ss" : "MMM-dd HH:mm:ss") + " " + timeZoneAbbr;
            if (hasBrackets)
                retVal = retVal.ToBracketedString();
            return retVal;
        }
    }
}
