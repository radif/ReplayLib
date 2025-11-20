using UnityEngine;

public static class ScreenUtils
{
    /// <summary>
    /// Controls whether the device's screen can timeout/sleep based on system settings.
    /// </summary>
    /// <value>
    /// <c>true</c> to enable screen timeout using system settings;
    /// <c>false</c> to prevent the screen from timing out (keep screen always on).
    /// </value>
    /// <remarks>
    /// When set to true, the screen will follow the device's system timeout settings.
    /// When set to false, the screen will stay on indefinitely (useful during gameplay).
    /// </remarks>
    /// <example>
    /// <code>
    /// // Keep screen always on
    /// ScreenUtils.screenTimerOn = false;
    /// 
    /// // Allow screen to timeout based on system settings
    /// ScreenUtils.screenTimerOn = true;
    /// </code>
    /// </example>
    public static bool screenTimerOn
    {
        get => Screen.sleepTimeout != SleepTimeout.NeverSleep;
        set => Screen.sleepTimeout = value ? SleepTimeout.SystemSetting : SleepTimeout.NeverSleep;
    }
    
    /// <summary>
    /// Converts millimeters to pixels based on device DPI
    /// </summary>
    /// <param name="millimeters">Distance in millimeters</param>
    /// <returns>Distance in pixels</returns>
    public static float MillimetersToPixels(float millimeters)
    {
        // Get device DPI, fallback to 160 DPI if not available
        float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;

        // Convert mm to inches (1 inch = 25.4 mm), then inches to pixels
        return (millimeters / 25.4f) * dpi;
    }
    
    public static float PixelsToMillimeters(float pixels)
    {
        // Get device DPI, fallback to 160 DPI if not available
        float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;

        // Convert pixels to inches, then inches to millimeters (1 inch = 25.4 mm)
        return (pixels / dpi) * 25.4f;
    }
    public static float PixelsToInches(float pixels)
    {
        // Get device DPI, fallback to 160 DPI if not available
        float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;

        // Convert pixels to inches
        return pixels / dpi;
    }
    
    /// <summary>
    /// Checks if a touch is near screen edges where iOS gestures might interfere
    /// </summary>
    public static bool IsTouchNearScreenEdge(Vector2 screenPosition, float edgeThresholdMM)
    {
        float edgeThreshold = MillimetersToPixels(edgeThresholdMM);
        
        return screenPosition.x < edgeThreshold ||
               screenPosition.x > Screen.width - edgeThreshold ||
               screenPosition.y < edgeThreshold ||
               screenPosition.y > Screen.height - edgeThreshold;
    }
    /// <summary>
    /// Checks if a screen position is within the safe area (iOS notch/home indicator safe)
    /// </summary>
    public static bool IsPositionInSafeArea(Vector2 screenPosition)
    {
        Rect safeArea = Screen.safeArea;
        return safeArea.Contains(screenPosition);
    }
}
