using System;
using System.Collections.Generic;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DefaultFormatter : ILogFormatter
    {
        public const String DefaultDateTimeFormat = "HH:mm:ss.fff";
        public const Char DefaultIndentChar = ' ';

        public DefaultFormatter() : this(DefaultIndentChar, DefaultDateTimeFormat)
        {
        }

        public DefaultFormatter(Char indentChar, String dateTimeFormatText)
        {
            this.IndentChar = indentChar;
            this.DateTimeFormat = dateTimeFormatText;
        }

        public string DateTimeFormat { get; private set; }
        public char IndentChar { get; private set; }
        public string Indent(int level)
        {
            var lvl = Math.Max(0, level);
            return "".PadLeft(lvl * 2, ' ');
        }

        public string FormatThread(ThreadData logEntry, ILogFilters filters)
        {
            var outputFormat = "[Thread {0} '{1}']";
            if (String.IsNullOrEmpty(logEntry.ThreadName))
                outputFormat = "[Thread {0}]";
            
            return String.Format(outputFormat, logEntry.ThreadId, logEntry.ThreadName);
        }

        public string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters)
        {
            var args = ((logEntry.Arguments == null) || filters.HideMethodParameters) ? "" : string.Join(", ", logEntry.Arguments);
            var timeText = filters.HideTimeStampInMethod ? "" : string.Concat(this.FormatTime(logEntry.Time), " ");
            return string.Format("[Enter] {0}{1}({2})", timeText, logEntry.MethodName, args);
        }

        public string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            var returnValue = (logEntry.ReturnRecorded && !filters.HideMethodReturnValue) ? "(" + logEntry.ReturnValue + ")" : "()";
            if (filters.HideMethodDuration)
                return string.Format("[Exit] {0}{1}", logEntry.MethodName, returnValue);

            return string.Format("[Exit] {0}{1} duration {2:0.####}ms", logEntry.MethodName, returnValue, logEntry.ElapsedMs);
        }

        public IList<string> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters)
        {
            throw new NotImplementedException(String.Format("{0} does not need to immplement this method, use the regular FormatMethodExit method.", this.GetType().Name));
        }

        public string FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters)
        {
            return string.Format("[Exception] {0} {1}", this.FormatTime(logEntry.Time), logEntry.Message);
        }

        public string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters)
        {
            return string.Format("[Log] [{0}] {1}", logEntry.Level, logEntry.Message);
        }

        protected string FormatTime(DateTime time)
        {
            return time.ToString(DateTimeFormat);
        }
    }
}