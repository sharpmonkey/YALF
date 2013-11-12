using System;
using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelayedFormatterService
    {
        private Stack<MethodEntry> _lastMethodEntry = new Stack<MethodEntry>(10);
        private List<LogEntryTracker> _orderedBuffer = new List<LogEntryTracker>(100);
        private int _currentNestedLevel = 0;

        public readonly string DateTimeFormat;

        public DelayedFormatterService(string dateTimeFormat)
        {
            this.DateTimeFormat = dateTimeFormat;
        }

        public string HandleMethodEntry(MethodEntry logEntry, bool displayEnabled)
        {
            // entry details are merged with exit details
            if (displayEnabled)
            {
                _lastMethodEntry.Push(logEntry);
                _orderedBuffer.Add(new LogEntryTracker(_currentNestedLevel, logEntry, null));
            }

            _currentNestedLevel++;
            return null;
        }

        public IList<OrderedOutput> HandleMethodExit(MethodExit logEntry, int lineNo, ILogFilters filters, Func<DateTime, string> lineBuilder, bool displayEnabled)
        {
            if (!displayEnabled)
            {
                // method is not actually displayed, we are tracking the nesting level for other items displayed such as logs, exceptions, etc
                _currentNestedLevel--;
                return null;
            }


            if ((_lastMethodEntry == null) || (_lastMethodEntry.Count <= 0))
                throw new InvalidOperationException(String.Format("No related Method Entry log has been set for '{0}' at line {1:0000} - there could be a problem with the yalf logs."
                                                                , logEntry.MethodName, lineNo));
            if (_lastMethodEntry.Peek().MethodName != logEntry.MethodName)
                throw new InvalidOperationException(String.Format("The method exit log '{1}' has a different name than the current method entry log '{0}' at line {2:0000} - there could be a problem with the yalf logs."
                                                                , _lastMethodEntry.Peek().MethodName, logEntry.MethodName, lineNo));

            var currentMethodEntry = _lastMethodEntry.Pop();
            _currentNestedLevel--;

            int indexOfEntryToUpdate = FindLastIndexOf(currentMethodEntry);
            if (indexOfEntryToUpdate < 0)
                throw new InvalidOperationException(String.Format("Could not find the method [{0}] in the current ordered buffer.  This probably means there is an error in the processing logic, or the yalf file is corrupt.",
                                                                        currentMethodEntry.MethodName));
            
            var currentItem = _orderedBuffer[indexOfEntryToUpdate];
            _orderedBuffer[indexOfEntryToUpdate] = new LogEntryTracker(currentItem.Level, currentItem.RelatedEntry, lineBuilder(currentMethodEntry.Time));

            if (_lastMethodEntry.Count > 0)
                return null;

            _currentNestedLevel = 0;
            return this.PrepareOutputBuffer();
        }

        private int FindLastIndexOf(MethodEntry logEntry)
        {
            for (int index = _orderedBuffer.Count-1; index >= 0; index--)
            {
                if ((_orderedBuffer[index].RelatedEntry != null) && (_orderedBuffer[index].RelatedEntry.MethodName == logEntry.MethodName))
                    return index;
            }

            return -1;
        }

        private IList<OrderedOutput> PrepareOutputBuffer()
        {
            var result = new List<OrderedOutput>(_orderedBuffer.Count);
            result.AddRange(_orderedBuffer.Select(item => new OrderedOutput(item.Level, item.FormattedLine)));

            _orderedBuffer.Clear();
            return result;

        }

        public string HandleException(string formattedLine)
        {
            return this.HandleNonMethodLogs(formattedLine);
        }

        public string HandleLogEvent(string formattedLine)
        {
            return this.HandleNonMethodLogs(formattedLine);
        }

        private string HandleNonMethodLogs(string formattedLine)
        {
            if (_orderedBuffer.Count <= 0)
                return formattedLine;

            _orderedBuffer.Add(new LogEntryTracker(_currentNestedLevel, null, formattedLine));
            return null;
        }

        private class LogEntryTracker
        {
            public readonly int Level;
            public readonly MethodEntry RelatedEntry;
            public readonly String FormattedLine;

            public LogEntryTracker(int level, MethodEntry relatedEntry, String formattedLine)
            {
                this.Level = level;
                this.RelatedEntry = relatedEntry;
                this.FormattedLine = formattedLine;
            }
        }
    }
}