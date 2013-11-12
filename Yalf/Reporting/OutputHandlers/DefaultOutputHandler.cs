using System;
using System.Collections.Generic;
using Yalf.LogEntries;
using Yalf.Reporting.Formatters;

namespace Yalf.Reporting.OutputHandlers
{
    public class DefaultOutputHandler : ILogOutputHandler
    {
        protected List<String> Buffer;
        private ILogFormatter _formatter;
        private int _lineNumber = 0;
        public DefaultOutputHandler(ILogFilters filters) : this(filters, new DefaultFormatter()) { }


        public DefaultOutputHandler(ILogFilters filters, ILogFormatter formatter)
        {
            if (filters == null)
                throw new ArgumentNullException("filters", "A valid set of log filters is required for proper operation.");

            this.Filters = filters;
            this.Formatter = formatter;
        }

        public ILogFormatter Formatter { get; protected set; }

        public int CurrentThreadId { get; protected set; }
        public ILogFilters Filters { get; protected set; }

        public void Initialise()
        {
            Buffer = new List<string>(4096);
            if (_formatter == null)
                _formatter = new DefaultFormatter();
        }

        public void HandleThread(ThreadData entry)
        {
            this.CurrentThreadId = entry.ThreadId;
            this.AddLine(this.Formatter.FormatThread(entry, this.Filters), 0);
        }

        public void HandleMethodEntry(MethodEntry entry, int indentLevel, bool displayEnabled)
        {
                this.AddLine(this.Formatter.FormatMethodEntry(this.CurrentThreadId, indentLevel, ++_lineNumber, entry, this.Filters, displayEnabled), indentLevel);
        }

        public void HandleMethodExit(MethodExit entry, int indentLevel, bool displayEnabled)
        {
            if (this.Formatter is ISingleLineOutputLogFormatter)
            {
                this.ManageNestedCallsForSingleLineFormats(entry, indentLevel, displayEnabled);
                return;
            }

            this.AddLine(this.Formatter.FormatMethodExit(this.CurrentThreadId, indentLevel, ++_lineNumber, entry, this.Filters, displayEnabled), indentLevel);
        }

        private void ManageNestedCallsForSingleLineFormats(MethodExit entry, int indentLevel, bool displayEnabled)
        {
            if (!displayEnabled)
                return;

            var output = (this.Formatter as ISingleLineOutputLogFormatter).FormatMethodExitForSingleLineOutput(this.CurrentThreadId, indentLevel, ++_lineNumber, entry, this.Filters, displayEnabled);
            if (output == null)
                return;

            foreach (OrderedOutput outputLine in output)
            {
                this.AddLine(outputLine.FormattedLine, indentLevel + outputLine.Level);
            }
        }

        public void HandleException(ExceptionTrace entry, int indentLevel)
        {
            this.AddLine(this.Formatter.FormatException(this.CurrentThreadId, indentLevel, ++_lineNumber, entry, this.Filters), indentLevel);
        }

        public void HandleLogEvent(LogEvent entry, int indentLevel, bool displayEnabled)
        {
                this.AddLine(this.Formatter.FormatLogEvent(this.CurrentThreadId, indentLevel, ++_lineNumber, entry, this.Filters, displayEnabled), indentLevel);
        }

        public String GetReport()
        {
            return String.Join(Environment.NewLine, Buffer.ToArray());
        }

        public void Complete()
        {
        }

        private void AddLine(string text, int level)
        {
            if (text != null)
                Buffer.Add(String.Concat(this.Formatter.Indent(level), text));
        }
    }
}