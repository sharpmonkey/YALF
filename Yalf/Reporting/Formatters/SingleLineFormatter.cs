using System;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class SingleLineFormatter : ILogFormatter
    {
        private readonly DefaultFormatter _default;
        private MethodEntry _lastMethodEntry = null;

        public SingleLineFormatter() : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat)
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

        public string FormatMethodEntry(int threadId, int level, MethodEntry logEntry, ILogFilters filters)
        {
            // entry details are merged with exit details
            _lastMethodEntry = logEntry;
            return null;
        }

        public string FormatMethodExit(int threadId, int level, MethodExit logEntry, ILogFilters filters)
        {
            if (_lastMethodEntry == null)
                throw new InvalidOperationException(String.Concat("No related Method Entry log has been set for '{0}' - there could be a problem with the yalf logs.", logEntry.MethodName));
            if (_lastMethodEntry.MethodName != logEntry.MethodName)
                throw new InvalidOperationException(String.Concat("The related Method Entry log '{0}' has a different name than the current exit method entry '{1}' - there could be a problem with the yalf logs.", _lastMethodEntry.MethodName, logEntry.MethodName));

            var returnValue = (logEntry.ReturnRecorded && !filters.HideMethodReturnValue) ? "(" + logEntry.ReturnValue + ")" : "()";
            var duration = (filters.HideMethodDuration) ? "" : string.Format(" duration {0:0.####}ms", logEntry.ElapsedMs);
            var timestamp = (filters.HideTimeStampInMethod) ? "" : string.Concat(" started ", (_lastMethodEntry.Time.ToString(DateTimeFormat)));

            return String.Concat(logEntry.MethodName, returnValue, timestamp, duration);

        }

        public string FormatException(int threadId, int level, ExceptionTrace logEntry, ILogFilters filters)
        {
            return _default.FormatException(threadId, level, logEntry, filters);
        }

        public string FormatLogEvent(int threadId, int level, LogEntry logEntry, ILogFilters filters)
        {
            return _default.FormatLogEvent(threadId, level, logEntry, filters);
        }
    }
}