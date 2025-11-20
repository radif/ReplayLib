using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Replay.Utils
{
    public static class MathExtensions
    {
        public static int Inverse(this int value) => value * -1;
        public static double Inverse(this double value) => value *= -1d;
        public static float Inverse(this float value) => value * -1f;
        public static float Complement(this float value)
        {
            if (value < 0.0f || value > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(value), "Expects value between in range 0 to 1.");

            return 1.0f - value;
        }
        public static double Complement(this double value)
        {
            if (value < 0.0d || value > 1.0d)
                throw new ArgumentOutOfRangeException(nameof(value), "Expects value between in range 0 to 1.");

            return 1.0d - value;
        }
        public static bool InRange(this int value, int min, int max) => value >= min && value <= max;
        public static float Normalize(this float value, float min, float max) => (value - min) / (max - min);
        public static float Map(this float value, float min, float max, float targetMin, float targetMax)
                        => (value - min) * ((targetMax - targetMin) / (max - min)) + targetMin;

        public static bool ToBool(this int value) => value == 0 ? false : true;
        public static int ToInt(this bool value) => value ? 1 : 0;
        
        
        // public static bool Approximately(float a, float b)
        // {
        //     return (double) Mathf.Abs(b - a) < (double) Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
        // }
        public static bool Approximately(float a, float b, float epsilon = 1E-06f)
        {
            return (double)Mathf.Abs(b - a) < (double)Mathf.Max(epsilon * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
        }
        // public static bool Approximately(double a, double b)
        // {
        //     return Math.Abs(b - a) < Math.Max(1E-06d * Math.Max(Math.Abs(a), Math.Abs(b)), double.Epsilon * 8d);
        // }
        public static double Clamp(float value, float min, float max)
        {
            if(value < min)
                return min;
            if(value > max)
                return max;
            return value;
        }
        public static bool Approximately(double a, double b, double epsilon = 1E-06d)
        {
            return Math.Abs(b - a) < Math.Max(epsilon * Math.Max(Math.Abs(a), Math.Abs(b)), double.Epsilon * 8d);
        }
        public static double Clamp(double value, double min, double max)
        {
            if(value < min)
                return min;
            if(value > max)
                return max;
            return value;
        }
        //random bool
        public static bool GetRandomBool(float chance = 0.5F) => Random.Range(0F, 1F) <= chance;
    }
}

