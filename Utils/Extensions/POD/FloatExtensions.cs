namespace Replay.Utils
{
    public static class FloatExtensions
    {
        public static bool Approximately(this float a, float b, float epsilon = 1E-06f) =>
            MathExtensions.Approximately(a, b, epsilon);
        public static double Clamp(this float value, float min, float max) =>
            MathExtensions.Clamp(value, min, max);
    }
    
}
