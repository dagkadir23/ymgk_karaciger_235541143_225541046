using System;
using System.Collections.Generic;
using UnityEngine;

namespace PatientLive.Utilities
{
    /// <summary>
    /// Central logging helper for the PatientLive prototype.
    /// Keeps recent logs in memory for RAMS-style traceability and filters logs by severity.
    /// </summary>
    public static class SimpleLogger
    {
        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            None = 4
        }

        private const int MaxHistorySize = 100;

        private static readonly Queue<string> LogHistory = new Queue<string>();
        private static LogLevel currentLogLevel = LogLevel.Debug;

        public static void SetLogLevel(LogLevel level)
        {
            currentLogLevel = level;
        }

        public static void Log(string message, string context = "")
        {
            Write(LogLevel.Debug, message, context);
        }

        public static void LogInfo(string message, string context = "")
        {
            Write(LogLevel.Info, message, context);
        }

        public static void LogWarning(string message, string context = "")
        {
            Write(LogLevel.Warning, message, context);
        }

        public static void LogError(string message, string context = "")
        {
            Write(LogLevel.Error, message, context);
        }

        public static void LogException(Exception exception, string context = "")
        {
            if (exception == null)
            {
                Write(LogLevel.Error, "Unknown exception.", context);
                return;
            }

            Write(LogLevel.Error, $"Exception: {exception.Message}\n{exception.StackTrace}", context);
        }

        public static string[] GetRecentLogs()
        {
            return LogHistory.ToArray();
        }

        public static void ClearHistory()
        {
            LogHistory.Clear();
        }

        private static void Write(LogLevel level, string message, string context)
        {
            if (level < currentLogLevel || level == LogLevel.None)
            {
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string contextPart = string.IsNullOrWhiteSpace(context) ? string.Empty : $"[{context}] ";
            string formattedMessage = $"[PatientLive][{timestamp}][{level}] {contextPart}{message}";

            LogHistory.Enqueue(formattedMessage);
            while (LogHistory.Count > MaxHistorySize)
            {
                LogHistory.Dequeue();
            }

            switch (level)
            {
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage);
                    break;
                default:
                    Debug.Log(formattedMessage);
                    break;
            }
        }
    }
}
