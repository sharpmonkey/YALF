using System;
using System.IO;
using Yalf.LogEntries;
using Yalf.Reporting.Formatters;

namespace Yalf.Reporting.OutputHandlers
{
    public class TextWriterOutputHandler : ILogOutputHandler
    {
        public readonly TextWriter Writer;
        private ILogFormatter _formatter;
        private int _lineNumber = 0;
        public TextWriterOutputHandler(TextWriter writer, ILogFilters filters) : this(writer, filters, new DefaultFormatter()) { }


        public TextWriterOutputHandler(TextWriter writer, ILogFilters filters, ILogFormatter formatter)
        {
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid stream writer is required for proper operation.");
            if (filters == null)
                throw new ArgumentNullException("filters", "A valid set of log filters is required for proper operation.");

            this.Filters = filters;
            this.Formatter = formatter;
            this.Writer = writer;
        }

        public ILogFormatter Formatter { get; protected set; }

        public int CurrentThreadId { get; private set; }
        public ILogFilters Filters { get; private set; }

        public void Initialise()
        {
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
            return null;
        }

        public void Complete()
        {
            this.Writer.Flush();
        }

        private void AddLine(string text, int level)
        {
            if (text == null)
                return;

            this.Writer.Write(this.Formatter.Indent(level));
            this.Writer.WriteLine(text);
        }
    }
}