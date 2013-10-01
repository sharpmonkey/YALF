using System;
using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Reporting.Formatters
{
    public class SingleLineFormatter : ILogFormatter
    {
        private readonly DefaultFormatter _default;
        private Stack<MethodEntry> _lastMethodEntry;
        private Stack<String> _orderedBuffer;
        private Dictionary<String, List<String>> _nonMethodLogs;
        private DelayedFormatterService _delayedService;

        public SingleLineFormatter() : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat)
        {
            _delayedService = new DelayedFormatterService();
            _nonMethodLogs = new Dictionary<string, List<string>>();
            _orderedBuffer = new Stack<string>(100);
            _delayedService = new DelayedFormatterService();
            _lastMethodEntry = new Stack<MethodEntry>(10);
        }

        public SingleLineFormatter(Char indentChar, String dateTimeFormatText)
        {
            _default = new DefaultFormatter(indentChar, dateTimeFormatText);
        }

        public string DateTimeFormat
        {
            get { return _default.DateTimeFormat; }
        }

        public char IndentChar
        {
            get { return _default.IndentChar; }
        }

        public string Indent(int level)
        {
            return _default.Indent(level);
        }

        public bool ProducesSingleLineMethodOutput
        {
            get { return true; }
        }

        public string FormatThread(ThreadData logEntry, ILogFilters filters)
        {
            return _default.FormatThread(logEntry, filters);
        }

        public string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters)
        {
            // entry details are merged with exit details
            _lastMethodEntry.Push(logEntry);
            return null;
        }

        /// <summary>
        /// Collates nested log calls and only returns an list of formatted log strings when a top level method exit log entry is envcountered
        /// </summary>
        /// <remarks>
        /// <para>This is required as, due to the nature of the single line formatter, nested logs are returned in the wrong order with the normal <see cref="FormatMethodExit"/> method.</para>
        /// <para>The indent must still be applied to the strings in the returned list in the <see cref="ILogOutputHandler"/>.  The first item will be the top level method call.</para>
        /// </remarks>
        public IList<String> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            if ((_lastMethodEntry == null) || (_lastMethodEntry.Count <= 0))
                throw new InvalidOperationException(String.Format("No related Method Entry log has been set for '{0}' at line {1:0000} - there could be a problem with the yalf logs."
                                                                , logEntry.MethodName, lineNo));
            if (_lastMethodEntry.Peek().MethodName != logEntry.MethodName)
                throw new InvalidOperationException(String.Format("The method exit log '{1}' has a different name than the current method entry log '{0}' at line {2:0000} - there could be a problem with the yalf logs."
                                                                , _lastMethodEntry.Peek().MethodName, logEntry.MethodName, lineNo));

            var currentMethodEntry = _lastMethodEntry.Pop();
            var returnValue = (logEntry.ReturnRecorded && !filters.HideMethodReturnValue) ? "(" + logEntry.ReturnValue + ")" : "()";
            var duration = (filters.HideMethodDuration) ? "" : string.Format(" duration {0:0.####}ms", logEntry.ElapsedMs);
            var timestamp = (filters.HideTimeStampInMethod) ? "" : string.Concat(" started ", (currentMethodEntry.Time.ToString(DateTimeFormat)));

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
            _orderedBuffer.Push(String.Concat(logEntry.MethodName, returnValue, timestamp, duration));

            if (_lastMethodEntry.Count > 0)
                return null;

            var result = _orderedBuffer.ToArray().ToList();
            _orderedBuffer.Clear();
            return result;

        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            throw new NotImplementedException(String.Format("{0} does not immplement this method as nested logs are delivered in the wrong order, use the FormatMethodExitDelayed method and apply the indent to the returned array of logs."
                , this.GetType().Name));
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            var result = _default.FormatException(threadId, level, lineNo, logEntry, filters);
            return this.HandleDelayedNonMethodLogs(result);
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            var result = _default.FormatLogEvent(threadId, level, lineNo, logEntry, filters);
            return this.HandleDelayedNonMethodLogs(result);
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