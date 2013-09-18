using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Yalf.Reporting
{
    public class LogFilters : ILogFilters
    {
        public IList<String> IncludedKeysExpressionList { get; private set; }
        public IList<String> ExcludedKeysExpressionList { get; private set; }
        public bool IgnoreCaseInFilter { get; private set; }
        public bool HideEnterMethodLogs { get; private set; }
        public bool HideExitMethodLogs { get; private set; }
        public bool HideTimeStampInMethod { get; private set; }
        public bool HideMethodParameters { get; private set; }
        public bool HideMethodDuration { get; private set; }
        public bool HideMethodReturnValue { get; private set; }
        public bool SingleLineFormat { get; private set; }
        public DateTime TimeStampFrom { get; private set; }
        public DateTime TimeStampTo { get; private set; }

        public LogFilters(IEnumerable<string> includedKeysExpressionList,
        IEnumerable<string> excludedKeysExpressionList,
        bool ignoreCaseInFilter,
        bool hideEnterMethodLogs,
        bool hideExitMethodLogs,
        bool hideTimeStampInMethod,
        bool hideMethodParameters,
        bool hideMethodDuration,
        bool hideMethodReturnValue,
        bool singleLineFormat,
        DateTime timeStampFrom,
        DateTime timeStampTo)
        {
            IncludedKeysExpressionList = new ReadOnlyCollection<string>(includedKeysExpressionList.ToList());
            ExcludedKeysExpressionList = new ReadOnlyCollection<string>(excludedKeysExpressionList.ToList());
            IgnoreCaseInFilter = ignoreCaseInFilter;
            HideEnterMethodLogs = hideEnterMethodLogs;
            HideExitMethodLogs = hideExitMethodLogs;
            HideTimeStampInMethod = hideTimeStampInMethod;
            HideMethodParameters = hideMethodParameters;
            HideMethodDuration = hideMethodDuration;
            HideMethodReturnValue = hideMethodReturnValue;
            SingleLineFormat = singleLineFormat;
            TimeStampFrom = timeStampFrom;
            TimeStampTo = timeStampTo;
        }
    }
}