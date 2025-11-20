using UnityEngine;

namespace Replay.Utils
{
    public interface IDebugLoggable
    {
        string ToDebugString()
            => ToString();
        string logTag => GetType().Name;
        string ToDebugStringWithLogs()
            => ToDebugString() + "\n\nLogs:\n" + this.GetLogs();
    }

    //TODO: make Dev a member and call this.Dev.Log()
    public static class IDebugLoggableExtensions
    {
        public static string ToDebugStringWithLogs(this IDebugLoggable loggable)
            => loggable.ToDebugString() + "\n\nLogs:\n" + loggable.GetLogs();
        //get logs
        public static string GetLogs(this IDebugLoggable loggable)
            => Logger.Instance.GetLogs(loggable.logTag);

        //debug logs
        public static void Log(this IDebugLoggable loggable, string message)
            => Debug.Log(Dev.ResolveMessageWithTag(message, loggable.logTag));

        public static void LogWarning(this IDebugLoggable loggable, string message)
            => Debug.LogWarning(Dev.ResolveMessageWithTag(message, loggable.logTag));

        public static void LogError(this IDebugLoggable loggable, string message)
            => Debug.LogError(Dev.ResolveMessageWithTag(message, loggable.logTag));

        //dev logs
        public static void DevLog(this IDebugLoggable loggable, string message)
            => Dev.Log(message, loggable.logTag);

        public static void DevLogWarning(this IDebugLoggable loggable, string message)
            => Dev.LogWarning(message, loggable.logTag);

        public static void DevLogError(this IDebugLoggable loggable, string message)
            => Dev.LogError(message, loggable.logTag);

    }
}