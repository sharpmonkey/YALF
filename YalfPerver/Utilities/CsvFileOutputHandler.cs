using System;
using System.IO;
using System.Text;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;
using Yalf.Reporting.OutputHandlers;

namespace YalfPerver.Utilities
{
    public class CsvFileOutputHandler : ILogOutputHandler
    {
        public CsvFileOutputHandler(ILogFilters filters, ILogFormatter formatter, String filePath)
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

        private TextWriter _writer;
        private TextWriterOutputHandler _outputHandler;

        public void Initialise()
        {
            this.SetupFilePath();

             _writer = new StreamWriter(new FileStream(this.FilePath, FileMode.CreateNew));
            _outputHandler = new TextWriterOutputHandler(_writer, this.Filters, this.Formatter);
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
            _writer.Flush();
            _writer.Dispose();
        }

        public string GetReport()
        {
            //var fs = new System.IO.FileStream("", FileMode.OpenOrCreate);
            //var sw = new StreamWriter(fs)

            throw new NotImplementedException("The FIleOutputHandler writes all report data to the designated file in the Complete() method, you need to read the contents from the file.");
        }
    }
}