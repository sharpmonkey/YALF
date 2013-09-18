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
        String FormatMethodEntry(int threadId, int level, MethodEntry logEntry, ILogFilters filters);
        String FormatMethodExit(int threadId, int level, MethodExit logEntry, ILogFilters filters);
        String FormatException(int threadId, int level, ExceptionTrace logEntry, ILogFilters filters);
        String FormatLogEvent(int threadId, int level, LogEntry logEntry, ILogFilters filters);
    }
}
