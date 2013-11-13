using System.Collections.Generic;
using System.Linq;
using Yalf.LogEntries;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Reporting
{
    public class LogReporter
    {
        public static ILogOutputHandler Report(IFilterableLogEntryList logEntryList, ILogOutputHandler outputHandler)
        {
            var entries = logEntryList.GetEntries();
            LogReporter printer = new LogReporter(logEntryList, outputHandler);

            printer.OutputHandler.Initialise();
            printer.GenerateOutput(entries, 0);
            printer.OutputHandler.Complete();

            return printer.OutputHandler;
        }

        protected LogReporter(IFilterableLogEntryList logEntryList, ILogOutputHandler outputHandler)
        {
            this.LogEntries = logEntryList;
            this.OutputHandler = outputHandler;
        }

        public ILogOutputHandler OutputHandler { get; protected set; }
        protected IFilterableLogEntryList LogEntries { get; private set; }

        protected void GenerateOutput(IEnumerable<BaseEntry> entries, int indentLevel)
        {
            var stack = new Stack<bool>();

            foreach (var entry in entries)
            {
                var type = entry.GetType();

                if (type == typeof(ThreadData))
                {
                    this.HandleThreadChange((ThreadData)entry);
                }
                else if (type == typeof(MethodEntry))
                {
                    var methodEntry = (MethodEntry)entry;
                    var enabled = this.LogEntries.EnabledMethod(methodEntry.MethodName);
                    stack.Push(enabled);

                    this.OutputHandler.HandleMethodEntry(methodEntry, indentLevel, enabled);
                    ++indentLevel;

                }
                else if (type == typeof(MethodExit))
                {
                    var enabled = false;
                    if (stack.Any())
                        enabled = stack.Pop();

                    if (indentLevel > 0) indentLevel--;
                    this.OutputHandler.HandleMethodExit((MethodExit)entry, indentLevel, enabled);

                }
                else if (type == typeof(LogEvent))
                {
                    var logEntry = (LogEvent)entry;
                    this.OutputHandler.HandleLogEvent(logEntry, indentLevel, this.LogEntries.EnabledLevel(logEntry.Level.ToString()));
                }
                else if (type == typeof(ExceptionTrace))
                {
                    this.OutputHandler.HandleException((ExceptionTrace)entry, indentLevel);
                }
            }
        }

        private void HandleThreadChange(ThreadData entry)
        {
            if ((this.LogEntries.Filters.ThreadId > 0) && (entry.ThreadId != this.LogEntries.Filters.ThreadId))
                return;

            this.OutputHandler.HandleThread(entry);

            if (entry.Entries != null && entry.Entries.Any())
                this.GenerateOutput(entry.Entries, 0);
        }
    }
}