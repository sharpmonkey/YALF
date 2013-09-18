using System;
using System.Collections.Generic;
using System.Linq;

namespace Yalf.Reporting
{
    public class LogFiltersBuilder
    {
        public LogFiltersBuilder()
        {
            this.IncludedKeysExpressionList = new List<string>();
            this.ExcludedKeysExpressionList = new List<string>();
            this.TimeStampFrom = DateTime.MinValue;
            this.TimeStampTo = DateTime.MaxValue;
        }

        public LogFiltersBuilder(ILogFilters filters)
        {
            this.IncludedKeysExpressionList = filters.IncludedKeysExpressionList.ToList();
            this.ExcludedKeysExpressionList = filters.ExcludedKeysExpressionList.ToList();
            this.TimeStampFrom = filters.TimeStampFrom;
            this.TimeStampTo = filters.TimeStampTo;
            this.IgnoreCaseInFilter = filters.IgnoreCaseInFilter;
            this.HideEnterMethodLogs = filters.HideEnterMethodLogs;
            this.HideExitMethodLogs = filters.HideExitMethodLogs;
            this.HideTimeStampInMethod = filters.HideTimeStampInMethod;
            this.HideMethodParameters = filters.HideMethodParameters;
            this.HideMethodDuration = filters.HideMethodDuration;
            this.HideMethodReturnValue = filters.HideMethodReturnValue;
            this.SingleLineFormat = filters.SingleLineFormat;
        }

        public IList<string> IncludedKeysExpressionList { get; set; }
        public IList<string> ExcludedKeysExpressionList { get; set; }
        public bool IgnoreCaseInFilter { get; set; }

        public bool HideEnterMethodLogs { get; set; }
        public bool HideExitMethodLogs { get; set; }
        public bool HideTimeStampInMethod { get; set; }
        public bool HideMethodParameters { get; set; }
        public bool HideMethodDuration { get; set; }
        public bool HideMethodReturnValue { get; set; }
        public bool SingleLineFormat { get; set; }
        public DateTime TimeStampFrom { get; set; }
        public DateTime TimeStampTo { get; set; }

        public ILogFilters Build()
        {
            return new LogFilters(IncludedKeysExpressionList,
                                  ExcludedKeysExpressionList,
                                  IgnoreCaseInFilter,
                                  HideEnterMethodLogs,
                                  HideExitMethodLogs,
                                  HideTimeStampInMethod,
                                  HideMethodParameters,
                                  HideMethodDuration,
                                  HideMethodReturnValue,
                                  SingleLineFormat,
                                  TimeStampFrom,
                                  TimeStampTo);
            ;
        }

        #region Fluent Interface
        public static LogFiltersBuilder Create()
        {
            return new LogFiltersBuilder();
        }

        public static LogFiltersBuilder Create(ILogFilters copyFilters)
        {
            return new LogFiltersBuilder(copyFilters);
        }

        public LogFiltersBuilder WithEnterMethodLogsHidden()
        {
            this.HideEnterMethodLogs = true;
            return this;
        }

        public LogFiltersBuilder WithExitMethodLogsHidden()
        {
            this.HideExitMethodLogs = true;
            return this;
        }

        public LogFiltersBuilder WithTimeStampInMethodHidden()
        {
            this.HideTimeStampInMethod = true;
            return this;
        }

        public LogFiltersBuilder WithMethodParametersHidden()
        {
            this.HideMethodParameters = true;
            return this;
        }

        public LogFiltersBuilder WithMethodDurationHidden()
        {
            this.HideMethodDuration = true;
            return this;
        }

        public LogFiltersBuilder WithMethodReturnValueHidden()
        {
            this.HideMethodReturnValue = true;
            return this;
        }

        public LogFiltersBuilder WithSingleLineFormat()
        {
            this.SingleLineFormat = true;
            return this;
        }

        public LogFiltersBuilder IncludeLogsFrom(DateTime logTimeStamp)
        {
            this.TimeStampFrom = logTimeStamp;
            return this;
        }

        public LogFiltersBuilder IncludeLogsUpto(DateTime logTimeStamp)
        {
            this.TimeStampTo = logTimeStamp;
            return this;
        }

        public LogFiltersBuilder IncludeTheseMethodNames(IList<string> includeExpressions)
        {
            this.IncludedKeysExpressionList = includeExpressions.ToList();
            return this;
        }

        public LogFiltersBuilder ExcludeTheseMethodNames(IList<string> excludeExpressions)
        {
            this.ExcludedKeysExpressionList = excludeExpressions.ToList();
            return this;
        }

        #endregion
    }
}