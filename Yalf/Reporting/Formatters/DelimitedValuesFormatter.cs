using System;
using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelimitedValuesFormatter : ILogFormatter
    {
        private const String DefaultDelimiter = ",";
        private readonly DefaultFormatter _default;

        private Stack<MethodEntry> _lastMethodEntry = new Stack<MethodEntry>(10);
        private Stack<String> _orderedBuffer = new Stack<string>(100);

        public DelimitedValuesFormatter()
            : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat, "Yalf", DefaultDelimiter)
        { }


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

        public bool ProducesSingleLineMethodOutput
        {
            get { return true; }
        }

        public string FormatThread(ThreadData logEntry, ILogFilters filters)
        {
            throw new NotImplementedException("There is no specific format for a thread data entry in a delimited list format, the thread id is included in the other log entry lines.");
        }

        public string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters)
        {
            // entry details are merged with exit details
            _lastMethodEntry.Push(logEntry);
            return null;
        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            throw new NotImplementedException(String.Format("{0} does not need to immplement this method, use the FormatMethodExitDelayed method so the calls are in the right order.", this.GetType().Name));
        }

        public IList<string> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
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

            _orderedBuffer.Push(String.Concat(logEntry.MethodName, returnValue, timestamp, duration));

            if (_lastMethodEntry.Count > 0)
                return null;

            var result = _orderedBuffer.ToArray().ToList();
            _orderedBuffer.Clear();
            return result;
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
            return String.Join(this.Delimiter,
                new[] { this.LogContext, LogType, title, details, timeStamp.ToString(this.DateTimeFormat), duration.ToString("0.####"), level.ToString(), threadId.ToString() }
            );
        }
    }
}