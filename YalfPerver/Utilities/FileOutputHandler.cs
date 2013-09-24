using System;
using System.IO;
using System.Text;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;
using Yalf.Reporting.OutputHandlers;

namespace YalfPerver.Utilities
{
    public class FileOutputHandler : ILogOutputHandler
    {
        public FileOutputHandler(ILogFilters filters, ILogFormatter formatter, String filePath)
        {
            if (filters == null)
                throw new ArgumentNullException("filters", "A valid set of log filters is required for proper operation.");

            this.Filters = filters;
            this.Formatter = formatter;
            this.FilePath = filePath;
        }

        public string FilePath { get; private set; }
        public ILogFormatter Formatter { get; private set; }
        public int CurrentThreadId { get; private set; }
        public ILogFilters Filters { get; private set; }

        private StringBuilder _buffer;

        // This is good enough to generate the text for output.  Later we may want to use a file stream which means this won't be good enough.
        private DefaultOutputHandler _outputHandler;

        public void Initialise()
        {
            this.SetupFilePath();

            _buffer = new StringBuilder(4096);
            _outputHandler = new DefaultOutputHandler(this.Filters, this.Formatter);
            _outputHandler.Initialise();
        }

        private void SetupFilePath()
        {
            if (File.Exists(this.FilePath))
            {
                File.Delete(this.FilePath);
                return;
            }

            var directory = Path.GetDirectoryName(this.FilePath);
            if (Directory.Exists(directory))
                return;

            Directory.CreateDirectory(directory);
        }

        public void HandleThread(ThreadData entry)
        {
            _outputHandler.HandleThread(entry);
        }

        public void HandleMethodEntry(MethodEntry entry, int indentLevel, bool displayEnabled)
        {
            _outputHandler.HandleMethodEntry(entry, indentLevel, displayEnabled);
        }

        public void HandleMethodExit(MethodExit entry, int indentLevel, bool displayEnabled)
        {
            _outputHandler.HandleMethodExit(entry, indentLevel, displayEnabled);
        }

        public void HandleException(ExceptionTrace entry, int indentLevel)
        {
            _outputHandler.HandleException(entry, indentLevel);
        }

        public void HandleLogEvent(LogEvent entry, int indentLevel, bool displayEnabled)
        {
            _outputHandler.HandleLogEvent(entry, indentLevel, displayEnabled);
        }

        public void Complete()
        {
            _outputHandler.Complete();

            File.WriteAllText(this.FilePath, _outputHandler.GetResults());
        }
    }
}