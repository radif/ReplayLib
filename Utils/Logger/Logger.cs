using System;
using UnityEngine;
using Replay.Utils;
public class Logger : Singleton<Logger>
{
    public static bool isDebugEnabled = true;
    
    const int kLogQueueSize = 1000;
    const int kMaxLogFileEntryCount = 10000;
    
    const int kDEBUGLogQueueSize = 5000;
    const int kDEBUGMaxLogFileEntryCount = 30000;
    
    public string exceptionMessage { get; private set; } = "";
    private LogQueue _logQueue = new (isDebugEnabled ? kDEBUGLogQueueSize : kLogQueueSize, isDebugEnabled ? kDEBUGMaxLogFileEntryCount : kMaxLogFileEntryCount, isDebugEnabled);

    public Action<string, string> onExceptionHandler;
    
    protected override void Init()
    {
        base.Init();
        Application.logMessageReceivedThreaded += OnLogThreaded;
        //Dev.Log("Logger Initialized");
    }
    protected override void Deinit()
    {
        base.Deinit();
        Application.logMessageReceivedThreaded -= OnLogThreaded;
        //Dev.Log("Logger Deinitialized");
    }

    public void FlushExceptionMessage()
    {
        exceptionMessage = "";
    }
    public void FlushLogQueue()
    {
        _logQueue.Clear();
    }

    void OnLogThreaded(string condition, string stackTrace, LogType type)
    {
        var dateTime = DateTime.Now.ToLoggerString();
        string[] lines = stackTrace.Split('\n');
        
        
        if (type == LogType.Exception)
        {
            
            onExceptionHandler?.Invoke(condition, stackTrace);

            if (string.IsNullOrEmpty(exceptionMessage))
            {
                string partialStackTrace = "";
                for (int i = 0; i < 5 && i < lines.Length; i++)
                    partialStackTrace += (i + 1) + ". " + lines[i] + "\n\n";


                string logs = GetLogs();
                
                exceptionMessage = "Exception:\n\n" +
                                   condition +
                                   "\n\n\n\nStack Trace:\n\n"
                                   + partialStackTrace +
                                   "\n\n\nLOGS:\n\n" +
                                   logs;

                DiagnosticUtils.exceptionsCount++;
            }
        }
        
        string functionName = lines.Length < 2 ? "..." : lines[1];

        //level up
        if (functionName.Contains("Replay.Utils.Dev:") && lines.Length > 2)
            functionName = lines[2];
        else if (functionName.Contains("IDebugLoggableExtensions:") && lines.Length > 2)
            functionName = lines[2];
        
        //level up
        if (functionName.Contains("IDebugLoggableExtensions:") && lines.Length > 3)
            functionName = lines[3];
        
        bool shouldFlushToFile = type != LogType.Log;

        string logType = "";
        
        switch (type)
        {
            case LogType.Log:
                break;
            case LogType.Warning:
                logType = "WARN: ";
                break;
            case LogType.Error:
                logType = "ERROR: ";
                break;
            case LogType.Exception:
                logType = "EXCEPTION: ";
                break;
        }

        _logQueue.Enqueue(dateTime + " " + logType + condition, functionName,
            shouldFlushToFile);
    }

    public string GetLogs(string tag = null, bool reversed = true) 
    => _logQueue.GetLogs(tag, reversed);
    
    //log file support
    public void FlushLogsToFile() => _logQueue.FlushCacheToFile();
    public void TrimLogFile() =>  _logQueue.TrimLogFile();
    
    //before sending it to support, it's best to close, then attach and re-open
    public void OpenLogFile() =>  _logQueue.OpenFile();
    public string logFilePath => _logQueue.filePath;
    public void CloseLogFile() =>  _logQueue.CloseFile();
    
    //probably not needed
    public void DeleteLogFile() => _logQueue.DeleteFile();
    
}
