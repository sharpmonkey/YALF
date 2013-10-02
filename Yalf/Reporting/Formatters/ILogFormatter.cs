using System;
using System.Collections.Generic;
using Yalf.LogEntries;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Reporting.Formatters
{
    public interface ILogFormatter
    {
        String DateTimeFormat { get; }
        Char IndentChar { get; }
        String Indent(int level);

        String FormatThread(ThreadData logEntry, ILogFilters filters);
        String FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters);
        String FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters);
        IList<string> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters);
        String FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters);
        String FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters);
    }
}
