namespace Replay.Utils { 
    public static class IntExtensions
    {

        public const int kSentinnelValue = -999999;
        public static string ToExpandedScoreFormat(this int num) => num.ToLocalCultureFormattedNumber();
        public static string ToExpandedScoreFormat(this long num) => num.ToLocalCultureFormattedNumber();
        
        public static int compactedScoreFormatSymbolCountCutOff = 6;

        public static bool GetFormatNeedsCompacting(this string s, int symbolCountCutOff = -1)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            int cutOff = symbolCountCutOff == -1 ?  compactedScoreFormatSymbolCountCutOff : symbolCountCutOff;
            
            return s.Length >= cutOff;
        }

        public static bool GetFormatNeedsCompacting(this int num, int symbolCountCutOff = -1)
        {
            string s = num.ToExpandedScoreFormat();
            return s.GetFormatNeedsCompacting(symbolCountCutOff);
        }
        public static bool GetFormatNeedsCompacting(this long num, int symbolCountCutOff = -1)
        {
            string s = num.ToExpandedScoreFormat();
            return s.GetFormatNeedsCompacting(symbolCountCutOff);
        }
        public static string ToCompactedScoreFormat(this long num, string kSuffix = "K", string mSuffix = "M", string bSuffix = "B", int symbolCountCutOff = -1)
        {
            string retVal = num.ToExpandedScoreFormat();
            if (retVal.GetFormatNeedsCompacting(symbolCountCutOff))
                retVal = num.ToScoreFriendlyKiloFormat(kSuffix, mSuffix, bSuffix);
            return retVal;
        }
        public static string ToLocalCultureFormattedNumber(this double num) => StringUtils.LocalCultureFormattedNumber(num);
        public static string ToLocalCultureFormattedNumber(this float num) => StringUtils.LocalCultureFormattedNumber(num);
        public static string ToLocalCultureFormattedNumber(this int num) => StringUtils.LocalCultureFormattedNumber(num);
        public static string ToLocalCultureFormattedNumber(this long num) => StringUtils.LocalCultureFormattedNumber(num);

        public static string ToStringWithPTSSuffix(this int num, bool hasSpace = false) => num +(hasSpace ? " " : "") + (num == 1 ? "pt" : "pts");
        public static string ToStringWithPTSSuffix(this long num, bool hasSpace = false) => num +(hasSpace ? " " : "") + (num == 1 ? "pt" : "pts");

        
        public static string ToScoreFriendlyKiloFormat(this long num, string kSuffix = "K", string mSuffix = "M", string bSuffix = "B")
        {
            
            if (num >= 1000000000)
            {
                double n = num / 1000000000F;
                //use "0.##" for hundredth
                return n.ToString("0.#", System.Globalization.CultureInfo.CurrentCulture) + bSuffix;
            }
            
            if (num >= 1000000)
            {
                double n = num / 1000000F;
                //use "0.##" for hundredth
                return n.ToString("0.#", System.Globalization.CultureInfo.CurrentCulture) + mSuffix;
            }


           // if (num >= 100000)
           if (num >= 1000)
            {
                double n = num / 1000F;
                //use "0.##" for hundredth
                return n.ToString("0.#", System.Globalization.CultureInfo.CurrentCulture) + kSuffix;
            }
                
            
            return num.ToString("#,0");
        }
        
        public static string ToKiloFormat(this long num)
        {
            
            if (num >= 100000000)
                return (num / 1000000).ToString("#,0M");

            if (num >= 10000000)
                return (num / 1000000).ToString("0.#") + "M";

            if (num >= 100000)
                return (num / 1000).ToString("#,0K");

            if (num >= 10000)
                return (num / 1000).ToString("0.#") + "K";

            return num.ToString("#,0");
        } 
        
        public static string ToOrdinalNumber(this long num)
        {
            if (num <= 0) return num.ToString();

            return num + GetOrdinalSuffix(num);
        }
        
        public static string GetOrdinalSuffix(long num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            switch (num % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        //this function only supports the places 1-12
        public static string ToPlaceOrdinalNumber(this int placeNumber, bool useFinalForThird = false)
        {
            var retVal = placeNumber.ToString();

            switch (placeNumber)
            {
                case 1:
                    retVal = "1st";
                    break;
                case 2:
                    retVal = "2nd";
                    break;
                case 3:
                    retVal = useFinalForThird ? "final" : "3rd";
                    break;
                    //TODO: 21, 22 etc
                default:
                    retVal += "th";
                    break;
            }

            return retVal;
        }
        //this function only supports the places 1-12
        public static string ToPlaceOrdinalString(this int placeNumber, bool useFinalForThird = false)
        {
            var retVal = placeNumber.ToPlaceOrdinalNumber(useFinalForThird);

            switch (placeNumber)
            {
                case 1:
                    retVal = "first";
                    break;
                case 2:
                    retVal = "second";
                    break;
                case 3:
                    retVal = useFinalForThird ? "final" : "third";
                    break;
                case 4:
                    retVal += "fourth";
                    break;
                case 5:
                    retVal = "fifth";
                    break;
                case 6:
                    retVal = "sixth";
                    break;
                case 7:
                    retVal = "seventh";
                    break;
                case 8:
                    retVal = "eighth";
                    break;
                case 9:
                    retVal = "ninth";
                    break;
                case 10:
                    retVal = "tenth";
                    break;
                case 11:
                    retVal = "eleventh";
                    break;
                case 12:
                    retVal = "twelfth";
                    break;
            }

            return retVal;
        }
    }
}
