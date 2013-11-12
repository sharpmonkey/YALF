using System;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public interface ILogFormatter
    {
        String DateTimeFormat { get; }
        Char IndentChar { get; }
        String Indent(int level);

        String FormatThread(ThreadData logEntry, ILogFilters filters);
        string FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters, bool displayEnabled);
        string FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters, bool displayEnabled);
        String FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters);
        string FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters, bool displayEnabled);
    }
}