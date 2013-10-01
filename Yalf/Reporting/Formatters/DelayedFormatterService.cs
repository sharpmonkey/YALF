using System;
using System.Collections.Generic;
using Yalf.LogEntries;

namespace Yalf.Reporting.Formatters
{
    public class DelayedFormatterService
    {
        private Stack<MethodEntry> _lastMethodEntry = new Stack<MethodEntry>(10);
        private Stack<String> _orderedBuffer = new Stack<string>(100);
        private Dictionary<String, List<String>> _nonMethodLogs = new Dictionary<string, List<string>>();

        public string HandleMethodEntry(MethodEntry logEntry)
        {
            // entry details are merged with exit details
            _lastMethodEntry.Push(logEntry);
            return null;
        }

    }
}
