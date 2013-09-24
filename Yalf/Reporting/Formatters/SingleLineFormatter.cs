using System;
using System.Collections.Generic;
using System.Text;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class SingleLineFormatter : ILogFormatter
    {
        private readonly DefaultFormatter _default;
        private Stack<MethodEntry> _lastMethodEntry = new Stack<MethodEntry>(10);
        private Stack<String> _orderedBuffer = new Stack<string>(100);

        public SingleLineFormatter()
            : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat)
        {
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

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
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
            _orderedBuffer.Push(String.Concat(logEntry.MethodName, returnValue, timestamp, duration));

            if (_lastMethodEntry.Count > 0)
                return null;

            if (_orderedBuffer.Count == 1)
                return _orderedBuffer.Pop();

            StringBuilder builder = new StringBuilder(_orderedBuffer.Count * 255);
            while (_orderedBuffer.Count > 0)
            {
                builder.AppendLine(_orderedBuffer.Pop());
            }

            return builder.ToString();
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            return _default.FormatException(threadId, level, lineNo, logEntry, filters);
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            return _default.FormatLogEvent(threadId, level, lineNo, logEntry, filters);
        }
    }
}