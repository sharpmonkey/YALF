using System;
using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Reporting.Formatters
{
    public class SingleLineFormatter : ILogFormatter, IIndentableSingleLineMethodFormatter
    {
        private readonly DefaultFormatter _default;
        private DelayedFormatterService _delayedService;

        public SingleLineFormatter()
            : this(DefaultFormatter.DefaultIndentChar, DefaultFormatter.DefaultDateTimeFormat)
        {
        }

        public SingleLineFormatter(Char indentChar, String dateTimeFormatText)
        {
            _default = new DefaultFormatter(indentChar, dateTimeFormatText);
            _delayedService = new DelayedFormatterService(dateTimeFormatText);
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
            return _delayedService.HandleMethodEntry(logEntry);
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
            var returnValue = (logEntry.ReturnRecorded && !filters.HideMethodReturnValue) ? "(" + logEntry.ReturnValue + ")" : "()";
            var duration = (filters.HideMethodDuration) ? "" : string.Format(" duration {0:0.####}ms", logEntry.ElapsedMs);
            Func<DateTime, String> lineBuilder = startTime =>
                        {
                            var timestamp = (filters.HideTimeStampInMethod) ? "" : string.Concat(" started ", (startTime.ToString(DateTimeFormat)));
                            return String.Concat(logEntry.MethodName, returnValue, timestamp, duration);
                        };

            return _delayedService.HandleMethodExit(logEntry, lineNo, filters, lineBuilder);
        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            throw new NotImplementedException(String.Format("{0} does not immplement this method as nested logs are delivered in the wrong order, use the FormatMethodExitDelayed method and apply the indent to the returned array of logs."
                , this.GetType().Name));
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            return _delayedService.HandleException(_default.FormatException(threadId, level, lineNo, logEntry, filters));
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            return _delayedService.HandleLogEvent(_default.FormatLogEvent(threadId, level, lineNo, logEntry, filters));
        }

        public bool IsLogEventLine(string formattedLine)
        {
            return (formattedLine.IndexOf("[Log]", StringComparison.Ordinal) == 0);
        }

        public bool IsExceptionTraceLine(string formattedLine)
        {
            return (formattedLine.IndexOf("[Exception]", StringComparison.Ordinal) == 0);
        }

        public bool IndentIncreaseRequired(string formattedLine)
        {
            if (this.IsLogEventLine(formattedLine)) return false;
            if (this.IsExceptionTraceLine(formattedLine)) return false;

            return true;
        }
    }
}