using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Replay.Utils
{
    public static class Dev
    {
        public static bool isDebugEnabled = true;
        public static string tag = "Dev";

        public static HashSet<string> customTags = new ();
        
        static string ResolveMessageWithCustomTag(string message, string customTag)
            => ResolveMessageWithTag(message, string.IsNullOrEmpty(customTag) ? tag : customTag);
        public static string ResolveMessageWithTag(string message, string tag)
        {
            customTags.Add(tag);
            return tag.ToBracketedString() + " " + (message ?? "[null]");
        }
        
        public static void Log(string s, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.Log(ResolveMessageWithCustomTag(s, customTag));
        }
        public static void LogMethod(string customTag = null, bool force = false)
        {
            if (!isDebugEnabled || force)
                return;
        
            var stackFrame = new System.Diagnostics.StackFrame(1, true);
            var method = stackFrame.GetMethod();
            var fileName = stackFrame.GetFileName();
            var lineNumber = stackFrame.GetFileLineNumber();
    
            string location = "";
    
            // Add namespace and class if available
            if (method?.DeclaringType != null)
            {
                var ns = method.DeclaringType.Namespace;
                var className = method.DeclaringType.Name;
                location += (!string.IsNullOrEmpty(ns) ? ns + "." : "") + className + ".";
            }
    
            // Add method name
            location += method?.Name ?? "UnknownMethod";
    
            // Add file and line info if available
            if (!string.IsNullOrEmpty(fileName))
            {
                location += $" ({System.IO.Path.GetFileName(fileName)}:{lineNumber})";
            }
    
            Debug.Log(ResolveMessageWithCustomTag(location, customTag));
        }
 
        public static void LogWarning(string s, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.LogWarning(ResolveMessageWithCustomTag(s, customTag));
        }
 
        public static void LogError(string s, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.LogError(ResolveMessageWithCustomTag(s, customTag));
        }
 
        
        public static void Log( string s, Object o, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.Log(ResolveMessageWithCustomTag(s, customTag), o);
        }
 
        public static void LogWarning(string s, Object o, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.LogWarning(ResolveMessageWithCustomTag(s, customTag), o);
        }
 
        public static void LogError(string s, Object o, string customTag = null, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.LogError(ResolveMessageWithCustomTag(s, customTag), o);
        }
 
        public static void LogError(object o, bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.LogError(o);
        }
 
        public static void Break(bool force = false)
        {
            if(isDebugEnabled || force)
                Debug.Break();
        }
        
        public static void ConsoleLog(string output, string customTag = null, bool force = false)
        {
            var s = ResolveMessageWithCustomTag(output, customTag);
            Console.WriteLine(s);
            if(isDebugEnabled || force)
                Debug.Log(s);
        }
    }
}
