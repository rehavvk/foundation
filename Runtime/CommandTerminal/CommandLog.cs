using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Foundation.CommandTerminal
{
    public enum TerminalLogType
    {
        Error = LogType.Error,
        Assert = LogType.Assert,
        Warning = LogType.Warning,
        Message = LogType.Log,
        Exception = LogType.Exception,
        Input,
        ShellMessage
    }

    public struct LogItem
    {
        public TerminalLogType type;
        public string message;
        public string stackTrace;
    }

    public class CommandLog
    {
        private List<LogItem> logs = new List<LogItem>();
        private int maxItems;

        public List<LogItem> Logs
        {
            get { return logs; }
        }

        public CommandLog(int maxItems)
        {
            this.maxItems = maxItems;
        }

        public void HandleLog(string message, TerminalLogType type)
        {
            HandleLog(message, "", type);
        }

        public void HandleLog(string message, string stackTrace, TerminalLogType type)
        {
            LogItem log = new LogItem()
            {
                message = message,
                stackTrace = stackTrace,
                type = type
            };

            logs.Add(log);

            if (logs.Count > maxItems)
            {
                logs.RemoveAt(0);
            }
        }

        public void Clear()
        {
            logs.Clear();
        }
    }
}