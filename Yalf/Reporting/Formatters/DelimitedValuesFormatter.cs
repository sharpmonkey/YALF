using System;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelimitedValuesFormatter : ILogFormatter
    {
        private const String DefaultDelimiter = ",";
        private readonly DefaultFormatter _default;
        private MethodEntry _lastMethodEntry = null;

        public DelimitedValuesFormatter() : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat, "Yalf", DefaultDelimiter)
        {}


        public DelimitedValuesFormatter(String logContext, String delimiter)
            : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat, logContext, delimiter)
        {
        }

        public DelimitedValuesFormatter(Char indentChar, String dateTimeFormatText, String logContext, String delimiter)
        {
            _default = new DefaultFormatter(indentChar, dateTimeFormatText);
            this.LogContext = logContext;
            this.Delimiter = delimiter;
        }

        public String Delimiter { get; set; }
        public string LogContext { get; private set; }

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
            throw new NotImplementedException("There is no specific format for a thread data entry in a delimited list format, the thread id is included in the other log entry lines.");
        }

        public string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters)
        {
            // entry details are merged with exit details
            _lastMethodEntry = logEntry;
            return null;
        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            if (_lastMethodEntry == null)
                throw new InvalidOperationException(String.Concat("No related Method Entry log has been set for '{0}' - there could be a problem with the yalf logs.", logEntry.MethodName));
            if (_lastMethodEntry.MethodName != logEntry.MethodName)
                throw new InvalidOperationException(String.Concat("The related Method Entry log '{0}' has a different name than the current exit method entry '{1}' - there could be a problem with the yalf logs.", _lastMethodEntry.MethodName, logEntry.MethodName));

            var returnValue = logEntry.ReturnRecorded ? logEntry.ReturnValue : "";
            return this.BuildOutputLine("Method", logEntry.MethodName, returnValue, _lastMethodEntry.Time, logEntry.ElapsedMs, level, threadId);
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            return this.BuildOutputLine("Exception", logEntry.Message, logEntry.StackTrace, logEntry.Time, 0, level, threadId);
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            return this.BuildOutputLine("Log", logEntry.Message, "", logEntry.Time, 0, level, threadId);
        }

        private string BuildOutputLine(string LogType, string title, string details, DateTime timeStamp, double duration, int level, int threadId)
        {
            return String.Join(this.Delimiter, this.LogContext, LogType, title, details, timeStamp.ToString(this.DateTimeFormat), duration.ToString("0.####")
                                , level.ToString(), threadId.ToString());
        }
    }
}