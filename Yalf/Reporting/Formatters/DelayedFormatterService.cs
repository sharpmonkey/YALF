using System;
using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelayedFormatterService
    {
        private Stack<MethodEntry> _lastMethodEntry = new Stack<MethodEntry>(10);
        private Stack<String> _orderedBuffer = new Stack<string>(100);
        private Dictionary<String, List<String>> _nonMethodLogs = new Dictionary<string, List<string>>();

        public readonly string DateTimeFormat;

        public DelayedFormatterService(string dateTimeFormat)
        {
            this.DateTimeFormat = dateTimeFormat;
        }

        public string HandleMethodEntry(MethodEntry logEntry)
        {
            // entry details are merged with exit details
            _lastMethodEntry.Push(logEntry);
            return null;
        }

        public IList<string> HandleMethodExit(MethodExit logEntry, int lineNo, ILogFilters filters, Func<DateTime, string> lineBuilder)
        {
            if ((_lastMethodEntry == null) || (_lastMethodEntry.Count <= 0))
                throw new InvalidOperationException(String.Format("No related Method Entry log has been set for '{0}' at line {1:0000} - there could be a problem with the yalf logs."
                                                                , logEntry.MethodName, lineNo));
            if (_lastMethodEntry.Peek().MethodName != logEntry.MethodName)
                throw new InvalidOperationException(String.Format("The method exit log '{1}' has a different name than the current method entry log '{0}' at line {2:0000} - there could be a problem with the yalf logs."
                                                                , _lastMethodEntry.Peek().MethodName, logEntry.MethodName, lineNo));

            // keep any nested items until we have returned to the top level as we process the exit methods from the lower most level to the top
            if (_nonMethodLogs.ContainsKey(logEntry.MethodName))
            {
                // push any LogEvents or ExceptionLogs into the correct place in the stack so the output is in the correct order
                var logs = _nonMethodLogs[logEntry.MethodName];
                while (logs.Count > 0)
                {
                    _orderedBuffer.Push(logs[logs.Count - 1]);
                    _nonMethodLogs[logEntry.MethodName].RemoveAt(logs.Count - 1);
                }
            }

            var currentMethodEntry = _lastMethodEntry.Pop();
            _orderedBuffer.Push(lineBuilder(currentMethodEntry.Time));

            if (_lastMethodEntry.Count > 0)
                return null;

            var result = _orderedBuffer.ToArray().ToList();
            _orderedBuffer.Clear();
            return result;
        }

        public string HandleException(string formattedLine)
        {
            return this.HandleDelayedNonMethodLogs(formattedLine);
        }

        public string HandleLogEvent(string formattedLine)
        {
            return this.HandleDelayedNonMethodLogs(formattedLine);
        }

        private string HandleDelayedNonMethodLogs(string result)
        {
            if (_lastMethodEntry.Count <= 0)
                return result;

            var key = _lastMethodEntry.Peek().MethodName;
            if (!_nonMethodLogs.ContainsKey(key))
                _nonMethodLogs.Add(key, new List<string>());

            _nonMethodLogs[key].Add(result);
            return null;
        }

    }
}