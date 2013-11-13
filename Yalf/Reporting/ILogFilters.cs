using System;
using System.Collections.Generic;

namespace Yalf.Reporting
{
    public interface ILogFilters
    {
        IList<string> IncludedKeysExpressionList { get; }
        IList<string> ExcludedKeysExpressionList { get; }
        bool IgnoreCaseInFilter { get; }
        bool HideEnterMethodLogs { get; }
        bool HideExitMethodLogs { get; }
        bool HideTimeStampInMethod { get; }
        bool HideMethodParameters { get; }
        bool HideMethodDuration { get; }
        bool HideMethodReturnValue { get; }
        bool SingleLineFormat { get; }
        DateTime TimeStampFrom { get; }
        DateTime TimeStampTo { get; }
        int ThreadId { get; }
    }
}