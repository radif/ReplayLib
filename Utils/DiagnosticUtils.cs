using System;
using System.Globalization;
using UnityEngine;

namespace Replay.Utils
{
    //this class is for utility functions that are used for diagnostics
    //it can serve as a superclass for other diagnostic utils classes
    public class DiagnosticUtils
    {
#region Screen

        public static (int, int) GetScreenResolution() =>
            (UnityEngine.Screen.currentResolution.width, UnityEngine.Screen.currentResolution.height);

        public static string GetScreenResolutionString()
        {
            var resolution = GetScreenResolution();
            return "{" + resolution.Item1 + ", " + resolution.Item2 + "}";
        }
        public static (int, int) GetWindowSize() =>
            (UnityEngine.Screen.width, UnityEngine.Screen.height);
        
        public static string GetWindowSizeString()
        {
            var windowSize = GetWindowSize();
            return "{" + windowSize.Item1 + ", " + windowSize.Item2 + "}";
        }
        public static string GetScreenDPIString() =>
            UnityEngine.Screen.dpi.ToString(CultureInfo.InvariantCulture);
#endregion
        #region Memory

        // Basic memory tracking
        public static long GetTotalMemory() =>
            GC.GetTotalMemory(false);

        public static long GetTotalMemoryMB() =>
            GetTotalMemory() / 1024 / 1024;

        public static string GetTotalMemoryMBString() =>
            GetTotalMemoryMB().ToLocalCultureFormattedNumber() + "MB";

        // Enhanced memory tracking
        private static long _lastMemorySample = 0;
        private static float _lastSampleTime = 0f;

        public static long GetGraphicsMemoryMB()
        {
            return SystemInfo.graphicsMemorySize;
        }

        public static long GetSystemMemoryMB()
        {
            return SystemInfo.systemMemorySize;
        }

        public static string GetGraphicsMemoryString() =>
            GetGraphicsMemoryMB().ToLocalCultureFormattedNumber() + "MB";

        public static string GetSystemMemoryString() =>
            GetSystemMemoryMB().ToLocalCultureFormattedNumber() + "MB";

        public static float GetMemoryPressureRatio()
        {
            long currentMemory = GetTotalMemoryMB();
            long systemMemory = GetSystemMemoryMB();
            if (systemMemory <= 0) return 0f;
            return (float)currentMemory / systemMemory;
        }

        public static string GetMemoryPressureString()
        {
            float pressure = GetMemoryPressureRatio();
            return (pressure * 100f).ToString("F1") + "%";
        }

        public static long GetMemoryAllocationRate()
        {
            float currentTime = Time.realtimeSinceStartup;
            long currentMemory = GetTotalMemory();

            if (_lastSampleTime <= 0f)
            {
                _lastSampleTime = currentTime;
                _lastMemorySample = currentMemory;
                return 0;
            }

            float deltaTime = currentTime - _lastSampleTime;
            if (deltaTime < 0.1f) return 0; // Sample at most every 100ms

            long deltaMemory = currentMemory - _lastMemorySample;
            long allocationRate = (long)(deltaMemory / deltaTime); // bytes per second

            _lastSampleTime = currentTime;
            _lastMemorySample = currentMemory;

            return allocationRate;
        }

        public static string GetMemoryAllocationRateString()
        {
            long rate = GetMemoryAllocationRate();
            if (rate == 0) return "0 B/s";

            long rateMB = rate / (1024 * 1024);
            if (rateMB > 0)
                return rateMB.ToLocalCultureFormattedNumber() + " MB/s";

            long rateKB = rate / 1024;
            if (rateKB > 0)
                return rateKB.ToLocalCultureFormattedNumber() + " KB/s";

            return rate.ToLocalCultureFormattedNumber() + " B/s";
        }

        public static bool IsMemoryPressureHigh(float threshold = 0.85f)
        {
            return GetMemoryPressureRatio() > threshold;
        }

        public static bool IsMemoryPressureCritical(float threshold = 0.95f)
        {
            return GetMemoryPressureRatio() > threshold;
        }

        public static MemoryInfo GetDetailedMemoryInfo()
        {
            return new MemoryInfo
            {
                totalMemoryMB = GetTotalMemoryMB(),
                graphicsMemoryMB = GetGraphicsMemoryMB(),
                systemMemoryMB = GetSystemMemoryMB(),
                memoryPressureRatio = GetMemoryPressureRatio(),
                allocationRateBytes = GetMemoryAllocationRate(),
                isHighPressure = IsMemoryPressureHigh(),
                isCriticalPressure = IsMemoryPressureCritical()
            };
        }

        public struct MemoryInfo
        {
            public long totalMemoryMB;
            public long graphicsMemoryMB;
            public long systemMemoryMB;
            public float memoryPressureRatio;
            public long allocationRateBytes;
            public bool isHighPressure;
            public bool isCriticalPressure;

            public string ToDebugString()
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Memory Usage: {totalMemoryMB.ToLocalCultureFormattedNumber()}MB");
                sb.AppendLine($"Graphics Memory: {graphicsMemoryMB.ToLocalCultureFormattedNumber()}MB");
                sb.AppendLine($"System Memory: {systemMemoryMB.ToLocalCultureFormattedNumber()}MB");
                sb.AppendLine($"Memory Pressure: {(memoryPressureRatio * 100f):F1}%");
                sb.AppendLine($"Allocation Rate: {GetMemoryAllocationRateString()}");
                sb.AppendLine($"High Pressure: {isHighPressure.ToBoolString()}");
                sb.AppendLine($"Critical Pressure: {isCriticalPressure.ToBoolString()}");
                return sb.ToString();
            }
        }

        #endregion

        #region Uptime
        
        public static double GetUptimeSeconds() => 
            UnityEngine.Time.realtimeSinceStartup;
        

        public static string GetUptimeString()
        {
            double uptime = GetUptimeSeconds();
            TimeSpan uptimeSpan = TimeSpan.FromSeconds(uptime);
            return uptimeSpan.ToString(@"hh\:mm\:ss");
        }

        #endregion

        #region Battery Level

        public static float GetBatteryLevel() =>
            UnityEngine.SystemInfo.batteryLevel;

        #endregion

        #region Temperature

        //public static float GetDeviceTemperature()=>
        //...

        #endregion

        #region Audio

        public static bool IsAudioPlaying() =>
            AudioSettings.dspTime > 0;

        //public static bool IsMuteOn() =>
        //...

        #endregion
        
        #region Exceptions
        public static int exceptionsCount = 0;
        #endregion
        
        public static string GetDiagnosticsString()
        {
            var stringBuilder = new System.Text.StringBuilder();

            // Enhanced memory information
            var memoryInfo = GetDetailedMemoryInfo();
            stringBuilder.AppendLine("=== Memory Information ===");
            stringBuilder.AppendLine("Total Memory: " + GetTotalMemoryMBString());
            stringBuilder.AppendLine("Graphics Memory: " + GetGraphicsMemoryString());
            stringBuilder.AppendLine("System Memory: " + GetSystemMemoryString());
            stringBuilder.AppendLine("Memory Pressure: " + GetMemoryPressureString());
            stringBuilder.AppendLine("Allocation Rate: " + GetMemoryAllocationRateString());
            stringBuilder.AppendLine("High Pressure: " + memoryInfo.isHighPressure.ToBoolString());
            stringBuilder.AppendLine("Critical Pressure: " + memoryInfo.isCriticalPressure.ToBoolString());

            stringBuilder.AppendLine("=== System Information ===");
            stringBuilder.AppendLine("Uptime: " + GetUptimeString());
            stringBuilder.AppendLine("Battery Level: " + GetBatteryLevel());
            stringBuilder.AppendLine("Is Audio Playing: " + IsAudioPlaying());
            stringBuilder.AppendLine("Screen Resolution: " + GetScreenResolutionString());
            stringBuilder.AppendLine("Screen DPI: " + GetScreenDPIString());
            stringBuilder.AppendLine("Window Size: " + GetWindowSizeString());
            stringBuilder.AppendLine("Exceptions Count: " + exceptionsCount);
            return stringBuilder.ToString();
        }
    }

}

