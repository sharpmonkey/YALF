using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Yalf.LogEntries;
using Yalf.Reporting.Formatters;

namespace Yalf.Reporting.OutputHandlers
{
    public class ThreadCollectionOutputHandler : DefaultOutputHandler
    {
        private readonly Dictionary<int, List<String>> _threadEntries;

        public ThreadCollectionOutputHandler(ILogFilters filters) : this(filters, new DefaultFormatter()) { }
        public ThreadCollectionOutputHandler(ILogFilters filters, ILogFormatter formatter) : base(filters, formatter)
        {
            _threadEntries = new Dictionary<int, List<String>>(20);
        }

        public void HandleThread(ThreadData entry)
        {
            this.CurrentThreadId = entry.ThreadId;
            _threadEntries.Add(entry.ThreadId, new List<String>());
            base.Buffer = _threadEntries[entry.ThreadId];
        }

        public String GetReport()
        {
            throw new NotImplementedException("Use the GetThreadEntries method to get the results for this output handler.");
        }

        public Dictionary<int, ReadOnlyCollection<String>> GetThreadEntries()
        {
            var result = new Dictionary<int, ReadOnlyCollection<String>>();
            foreach (int key in _threadEntries.Keys)
            {
                result.Add(key, new ReadOnlyCollection<String>(_threadEntries[key]));
            }
            return result;
        }

        public void Complete()
        {
        }
    }
}