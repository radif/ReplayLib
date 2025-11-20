namespace Replay.Utils
{
    public static class DoubleExtensions
    {
        public static bool Approximately(this double a, double b, double epsilon = 1E-06d) =>
            MathExtensions.Approximately(a, b, epsilon);
        public static double Clamp(this double value, double min, double max) =>
            MathExtensions.Clamp(value, min, max);
    }
    
}