namespace Replay.Utils { 
    public static class BoolExtensions
    {
        public static void Flip(this ref bool b) => b = !b;
        public static bool Fliped(this bool b) => !b;

        public static string ToBoolString(this bool b) => b ? "true" : "false";

        public static bool ToBool(this string boolString)
        {
            if (string.IsNullOrEmpty(boolString))
                return false;
            if (string.IsNullOrWhiteSpace(boolString))
                return false;
            string lowerCased = boolString.ToLower();
            if (lowerCased == "true")
                return true;
            if (lowerCased == "1")
                return true;
            //other rnumbers?
            return false;
        }

        public static bool GetRandom(float chance = 0.5F) => MathExtensions.GetRandomBool(chance);
        public static void SetRandom(this ref bool b, float chance = 0.5F) => b = GetRandom(chance);
    }
}
