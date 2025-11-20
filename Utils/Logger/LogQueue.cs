using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Replay.Utils
{
    public class LogQueue : FixedSizedQueue<string>
    {
        private string _getLogsCached = null;
        private string _reversedGetLogsCached = null;

        const string _fileName = "logs.log";
        private readonly string _filePath = null;
        public string filePath => _filePath;
        
        private StreamWriter _fileWriter;
        private StringBuilder _logFileCache = new ();
        readonly int maxLogFileCacheLength;
        readonly int maxLogFileEntryCount;
        private readonly bool isDebugEnabled;
        public static string GetLogFilePath() => Path.Combine(Application.persistentDataPath, _fileName);
        public LogQueue(int size, int maxLogFileEntryCount, bool isDebugEnabled) : base(size)
        {
            this.isDebugEnabled = isDebugEnabled;
            maxLogFileCacheLength = size;
            this.maxLogFileEntryCount = maxLogFileEntryCount;
            _filePath = GetLogFilePath();
            Dev.Log("[LogQueue]: file path: " + _filePath);


            _DEBUG_PreloadOldLogsFromFile();
            
            OpenFile();
        }
        ~LogQueue()
        {
            CloseFile();
        }
        
        public void Enqueue(string message, string function, bool forceFlushToFile = false)
        {
            base.Enqueue("<b>" + message + "</b>\n" + function + "\n");
            string fileString = message.Replace("\n", " | ") + " | " + function + "\n";
            AppendToFile(fileString, forceFlushToFile);
            ClearGetLogsCache();
        }

        public new void Clear()
        {
            base.Clear();
            ClearGetLogsCache();
        }
        
        
        public void ClearGetLogsCache()
        {
            _getLogsCached = null;
            _reversedGetLogsCached = null;
        }
        public string GetLogs(string tag = null, bool reversed = true)
        {
            bool hasTag = !string.IsNullOrEmpty(tag);

            if (!hasTag)
            {
                if (reversed)
                {
                    if (!string.IsNullOrEmpty(_reversedGetLogsCached))
                        return _reversedGetLogsCached;
                }
                else
                {
                    if (!string.IsNullOrEmpty(_getLogsCached))
                        return _getLogsCached;    
                }
                
            }

            var logs = reversed ? this.Reverse() : this;
            var debugOldLines = reversed ? _DEBUG_fileLines?.Reverse() : _DEBUG_fileLines;
            StringBuilder retVal = new ();

            void AddSeparator()
            {
                if (_DEBUG_fileLines != null && _DEBUG_fileLines.Length > 0)
                {
                    retVal.AppendLine();
                    retVal.AppendLine("------------------------");
                    retVal.AppendLine();
                    retVal.AppendLine("<b>Logs Before App Launch:</b>");
                    retVal.AppendLine();
                    retVal.AppendLine("------------------------");
                    retVal.AppendLine();
                }
            }

            if (!hasTag)
            {
                foreach (var log in logs)
                    retVal.AppendLine(log);

                AddSeparator();
                
                retVal.AppendLine("<i>");
                foreach (var log in debugOldLines)
                    retVal.AppendLine(log + "\n");
                retVal.AppendLine("</i>");
            }
            else
            {
                string tagToSearch = tag.ToBracketedString();
                foreach (var log in logs)
                    if(log.Contains(tagToSearch))
                        retVal.AppendLine(log);

                AddSeparator();
                
                retVal.AppendLine("<i>");
                foreach (var log in debugOldLines)
                    if(log.Contains(tagToSearch))
                        retVal.AppendLine(log + "\n");
                retVal.AppendLine("</i>");
            }

            if (!hasTag)
            {
                if (reversed)
                {
                    _reversedGetLogsCached = retVal.ToString();
                    return _reversedGetLogsCached;   
                }
                else
                {
                    _getLogsCached = retVal.ToString();
                    return _getLogsCached;    
                }
                
            }
            return retVal.ToString();
        }
        
        //file management

        public void DeleteFile()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    File.Delete(_filePath);
                    //Dev.Log("[LogQueue]: Log File Deleted Successfully!");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[LogQueue]: Cannot delete File Log - Exception: " + e);
                }
            }
        }

        public void TrimLogFile()
        {
            OpenFile();
        }

        void TrimLogFileImpl()
        {

            var lines = ReadAllLinesFromLogFile();
            File.WriteAllLines(_filePath, lines);
        }
        public void OpenFile()
        {
            if(isFileOpen)
                CloseFile();
            
            TrimLogFileImpl();
            
            _fileWriter = new(_filePath, true);
        }

        string[] _DEBUG_fileLines = new string[0];
        void _DEBUG_PreloadOldLogsFromFile()
        {
            if (isDebugEnabled)    
                _DEBUG_fileLines = ReadAllLinesFromLogFile();
        }
        string[] ReadAllLinesFromLogFile()
        {
            if (File.Exists(_filePath))
            {
                var lines = File.ReadAllLines(_filePath).Reverse().Take(maxLogFileEntryCount).Reverse().ToArray();
                return lines;
            }
            return new string[0];
        }
        void AppendToFile(string log, bool forceFlushToFile = false)
        {
            _logFileCache.Append(log);
            if(forceFlushToFile || _logFileCache.Length > maxLogFileCacheLength)
                FlushCacheToFile();
        }

        public void FlushCacheToFile()
        {
            try
            {
                _fileWriter?.Write(_logFileCache.ToString());
                _fileWriter?.Flush();    
            }catch(System.ObjectDisposedException)
            {
                //object disposed, silencing the issue
            }
            catch(System.Exception e)
            {
                Debug.LogError("[LogQueue]: Cannot write to File Log - Exception: " + e);
            }
            _logFileCache.Clear();
        }
        public void CloseFile()
        {
            FlushCacheToFile();
            try
            {
                _fileWriter?.Close();
            }catch(System.ObjectDisposedException)
            {
                //object disposed, silencing the issue
            }
            catch(System.Exception e)
            {
                Debug.LogError("[LogQueue]: Cannot close _fileWriter - Exception: " + e);
            }
            _fileWriter = null;
        }
        
        public bool isFileOpen => _fileWriter != null;
    }
}
