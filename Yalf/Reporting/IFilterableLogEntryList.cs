using System.Collections.Generic;
using Yalf.LogEntries;

namespace Yalf.Reporting
{
    public interface IFilterableLogEntryList
    {
        void Refresh();

        bool EnabledMethod(string methodName);
        bool EnabledLevel(string level);
        IList<BaseEntry> GetEntries();

        ILogFilters Filters { get; }
    }
}