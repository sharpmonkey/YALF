using System.Collections.Generic;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    interface ISingleLineOutputLogFormatter
    {
        IList<OrderedOutput> FormatMethodExitForSingleLineOutput(int threadId, int level, int lineNo, MethodExit logEntry, ILogFilters filters, bool displayEnabled);
    }
}
