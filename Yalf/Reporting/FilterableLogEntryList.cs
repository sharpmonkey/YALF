using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Yalf.LogEntries;

namespace Yalf.Reporting
{
    /// <summary>
    /// List of Yalf log entries that is used in reporting
    /// </summary>
    public class FilterableLogEntryList : IFilterableLogEntryList
    {
        private readonly Dictionary<string, bool> _flags;
        private readonly IEnumerable<BaseEntry> _logEntries;

        public FilterableLogEntryList CopyWithUpdatedLogEntries(IEnumerable<BaseEntry> entries)
        {
            FilterableLogEntryList newLogEntryList = new FilterableLogEntryList(entries, this.Filters);

            newLogEntryList.SetupFilterList();

            // preserve selected state of filters from previous instance
            foreach (var flagItem in this._flags)
            {
                if (newLogEntryList._flags.ContainsKey(flagItem.Key))
                    newLogEntryList._flags[flagItem.Key] = this._flags[flagItem.Key];
            }

            return newLogEntryList;
        }

        public FilterableLogEntryList(IEnumerable<BaseEntry> entries, ILogFilters filters)
        {
            _logEntries = entries;
            _flags = new Dictionary<string, bool>();
            this.Filters = filters;

            this.CalculateStartAndEndTimes();

            this.SetupFilterList();
        }

        private void CalculateStartAndEndTimes()
        {
            DateTime startTime = DateTime.MaxValue;
            DateTime endTime = DateTime.MinValue;

            this.FindTimeRangeInEntries(_logEntries, ref startTime, ref endTime);

            this.FirstLogTime = (startTime == DateTime.MaxValue) ? DateTime.MinValue : startTime;
            this.LastLogTime = (endTime == DateTime.MinValue) ? DateTime.MaxValue : endTime;
        }

        private void FindTimeRangeInEntries(IEnumerable<BaseEntry> logEntries, ref DateTime startTime, ref DateTime endTime)
        {
            foreach (var entry in logEntries)
            {
                if (entry is MethodExit)
                    continue;

                if (entry is ThreadData)
                {
                    this.FindTimeRangeInEntries((entry as ThreadData).Entries, ref startTime, ref endTime);
                    continue;
                }

                if (entry is MethodEntry)
                {
                    if ((entry as MethodEntry).Time < startTime) startTime = (entry as MethodEntry).Time;
                    if ((entry as MethodEntry).Time > endTime) endTime = (entry as MethodEntry).Time;
                }
                if (entry is LogEvent)
                {
                    if ((entry as LogEvent).Time < startTime) startTime = (entry as LogEvent).Time;
                    if ((entry as LogEvent).Time > endTime) endTime = (entry as LogEvent).Time;
                }
                if (entry is ExceptionTrace)
                {
                    if ((entry as ExceptionTrace).Time < startTime) startTime = (entry as ExceptionTrace).Time;
                    if ((entry as ExceptionTrace).Time > endTime) endTime = (entry as ExceptionTrace).Time;
                }
            }
        }


        public DateTime FirstLogTime { get; private set; }
        public DateTime LastLogTime { get; private set; }

        public ILogFilters Filters { get; set; }


        public IList<BaseEntry> GetEntries()
        {
            return new ReadOnlyCollection<BaseEntry>(_logEntries.ToList());
        }

        public IList<ThreadData> GetThreadDataItems()
        {
            return new ReadOnlyCollection<ThreadData>(_logEntries.OfType<ThreadData>().ToList());
        }

        public IList<KeyValuePair<string, bool>> GetItemCheckedStateList()
        {
            return _flags.ToList();
        }

        public bool GetAnyItemChecked()
        {
            return _flags.Any(f => f.Value);
        }

        public Dictionary<string, bool> GetItemCheckedStateDictionary()
        {
            return _flags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void SetSelectedState(string key, bool state)
        {
            if (_flags.ContainsKey(key))
                _flags[key] = state;
        }

        public bool EnabledMethod(string key)
        {
            return _flags.ContainsKey(key) && _flags[key];
        }

        public bool EnabledLevel(string level)
        {
            var key = FormatLevel(level);

            return _flags.ContainsKey(key) && _flags[key];
        }

        public bool TimeIsValid(DateTime time)
        {
            if ((this.Filters.TimeStampFrom > DateTime.MinValue) && (time < this.Filters.TimeStampFrom)) return false;
            if ((this.Filters.TimeStampTo < DateTime.MaxValue) && (time > this.Filters.TimeStampTo)) return false;

            return true;
        }

        private void SetupFilterList()
        {
            IList<String> values = this.GetFilteredEntryList(this._logEntries);

            foreach (var value in values.Where(value => !_flags.ContainsKey(value)))
                _flags.Add(value, true);

            var keys = _flags.Keys.ToList();
            foreach (var key in keys)
            {
                if (values.Contains(key) && (_flags[key] == false))
                    _flags[key] = true;
                else if (!values.Contains(key) && (_flags[key] == true))
                    _flags[key] = false;
            }
        }

        private static string FormatLevel(string level)
        {
            return string.Format(" LogLevel.{0}", level);
        }

        protected IList<string> GetFilteredEntryList(IEnumerable<BaseEntry> entries)
        {
            var filterSet = new HashSet<string>();
            var toInspect = entries.ToList();

            RegexOptions options = (this.Filters.IgnoreCaseInFilter) ? RegexOptions.IgnoreCase : RegexOptions.None;

            for (int i = 0; i < toInspect.Count; i++)
            {
                var item = toInspect[i];

                var threadItem = item as ThreadData;
                if (threadItem != null)
                {
                    toInspect.AddRange(threadItem.Entries);
                    toInspect.RemoveAt(i);
                    i--;
                    continue;
                }

                var logItem = item as LogEvent;
                if (logItem != null)
                {
                    filterSet.Add(FormatLevel(logItem.Level.ToString()));
                    continue;
                }

                var methodEntryItem = item as MethodEntry;
                if (methodEntryItem != null)
                {
                    if (!this.Filters.IncludedKeysExpressionList.Any() || this.Filters.IncludedKeysExpressionList.Any(ike => Regex.IsMatch(methodEntryItem.MethodName, ike, options)))
                        if (!this.Filters.ExcludedKeysExpressionList.Any() || this.Filters.ExcludedKeysExpressionList.All(eke => !Regex.IsMatch(methodEntryItem.MethodName, eke, options)))
                            filterSet.Add(methodEntryItem.MethodName);
                }
            }

            return new ReadOnlyCollection<string>(filterSet.ToList());
        }


        /// <summary>
        /// Reapply set filters to current list of log entries
        /// </summary>
        public void Refresh()
        {
            this.SetupFilterList();
        }
    }
}