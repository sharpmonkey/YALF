using System;
using System.Collections.Generic;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelimitedValuesFormatter : ILogFormatter, IIndentableSingleLineMethodFormatter
    {
        private const String DefaultDelimiter = ",";
        private readonly DefaultFormatter _default;
        private readonly DelayedFormatterService _delayedService;

        public DelimitedValuesFormatter() : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat, "Yalf", DefaultDelimiter) { }

        public DelimitedValuesFormatter(String logContext, String delimiter) : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat, logContext, delimiter) { }

        public DelimitedValuesFormatter(Char indentChar, String dateTimeFormatText, String logContext, String delimiter)
        {
            _default = new DefaultFormatter(indentChar, dateTimeFormatText);
            _delayedService = new DelayedFormatterService(dateTimeFormatText);
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
            get { throw new NotImplementedException(String.Format("There is no indent for a [{0}]", this.GetType().FullName)); }
        }

        public string Indent(int level)
        {
            throw new NotImplementedException(String.Format("There is no indent for a [{0}]", this.GetType().FullName));
        }

        public string FormatThread(ThreadData logEntry, ILogFilters filters)
        {
            throw new NotImplementedException("There is no specific format for a thread data entry in a delimited list format, the thread id is included in the other log entry lines.");
        }

        public string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters)
        {
            // entry details are merged with exit details
            return _delayedService.HandleMethodEntry(logEntry);
        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            throw new NotImplementedException(String.Format("{0} does not need to immplement this method, use the FormatMethodExitDelayed method so the calls are in the right order.", this.GetType().Name));
        }

        public IList<OrderedOutput> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            var returnValue = (logEntry.ReturnRecorded && !filters.HideMethodReturnValue) ? logEntry.ReturnValue : "";
            Func<DateTime, String> lineBuilder = startTime => BuildOutputLine("Method", logEntry.MethodName, returnValue, startTime, logEntry.ElapsedMs, level, threadId);

            return _delayedService.HandleMethodExit(logEntry, lineNo, filters, lineBuilder);
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            var stackTrace = (logEntry.StackTrace == null) ? "" : logEntry.StackTrace.Replace(Environment.NewLine, " ");
            return _delayedService.HandleException(this.BuildOutputLine("Exception", logEntry.Message.Replace(Environment.NewLine, " "), stackTrace, logEntry.Time, 0, level, threadId));
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            return _delayedService.HandleLogEvent(this.BuildOutputLine("Log", logEntry.Message, "", logEntry.Time, 0, level, threadId));
        }

        private string BuildOutputLine(string LogType, string title, string details, DateTime timeStamp, double duration, int level, int threadId)
        {
            return String.Join(this.Delimiter,
                new[] { this.LogContext, LogType, title, details, timeStamp.ToString(this.DateTimeFormat), duration.ToString("0.####"), level.ToString(), threadId.ToString() }
            );
        }

        public bool IsLogEventLine(string formattedLine)
        {
            return this.matchStartPrefix(formattedLine, String.Concat("Log", this.Delimiter));
        }

        public bool IsExceptionTraceLine(string formattedLine)
        {
            return this.matchStartPrefix(formattedLine, String.Concat("Exception", this.Delimiter));
        }

        private bool matchStartPrefix(string formattedLine, string prefix)
        {
            return (formattedLine.IndexOf(prefix, StringComparison.Ordinal) == 0);
        }

        public bool IndentIncreaseRequired(string formattedLine)
        {
            if (this.IsLogEventLine(formattedLine)) return false;
            if (this.IsExceptionTraceLine(formattedLine)) return false;

            return true;
        }
    }
}