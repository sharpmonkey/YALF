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

        /// <summary>Indicates that the formatter compresses MethodEntry and MthodExit calls into one output line.</summary>
        /// <remarks><para>This gives the <see cref="ILogOutputHandler"/> an indication to use the <see cref="FormatMethodExitDelayed"/> method for single line outputs.</para></remarks>
        bool ProducesSingleLineMethodOutput { get; }

        String FormatThread(ThreadData logEntry, ILogFilters filters);
        String FormatMethodEntry(int threadId, int level, int lineNo, MethodEntry logEntry, ILogFilters filters);
        String FormatMethodExit(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters);
        IList<string> FormatMethodExitDelayed(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters);
        String FormatException(int threadId, int level, int lineNo, ExceptionTrace logEntry, ILogFilters filters);
        String FormatLogEvent(int threadId, int level, int lineNo, LogEvent logEntry, ILogFilters filters);
    }
}
