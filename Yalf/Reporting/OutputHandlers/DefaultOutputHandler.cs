﻿using System;
using System.Text;
using Yalf.LogEntries;
using Yalf.Reporting.Formatters;

namespace Yalf.Reporting.OutputHandlers
{
    public class DefaultOutputHandler : ILogOutputHandler
    {
        private StringBuilder _buffer;
        private ILogFormatter _formatter;

        public DefaultOutputHandler(ILogFilters filters) : this(filters, new DefaultFormatter()) { }


        public DefaultOutputHandler(ILogFilters filters, ILogFormatter formatter)
        {
            if (filters == null)
                throw new ArgumentNullException("filters", "A valid set of log filters is required for proper operation.");

            this.Filters = filters;
            this.Formatter = formatter;
        }

        public ILogFormatter Formatter { get; protected set; }

        public int CurrentThreadId { get; private set; }
        public ILogFilters Filters { get; private set; }

        public void Initialise()
        {
            _buffer = new StringBuilder(4096);
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
            this.AddLine(this.Formatter.FormatMethodEntry(this.CurrentThreadId, indentLevel, entry, this.Filters), indentLevel);
        }

        public void HandleMethodExit(MethodExit entry, int indentLevel, bool displayEnabled)
        {
            this.AddLine(this.Formatter.FormatMethodExit(this.CurrentThreadId, indentLevel, entry, this.Filters), indentLevel);
        }

        public void HandleException(ExceptionTrace entry, int indentLevel)
        {
            this.AddLine(this.Formatter.FormatException(this.CurrentThreadId, indentLevel, entry, this.Filters), indentLevel);
        }

        public void HandleLogEvent(LogEntry entry, int indentLevel, bool displayEnabled)
        {
            this.AddLine(this.Formatter.FormatLogEvent(this.CurrentThreadId, indentLevel, entry, this.Filters), indentLevel);
        }

        public String GetResults()
        {
            return _buffer.ToString();
        }

        public void Complete()
        {
        }

        private void AddLine(string text, int level)
        {
            if (text != null)
                _buffer.Append(this.Formatter.Indent(level)).AppendLine(text);
        }
    }
}