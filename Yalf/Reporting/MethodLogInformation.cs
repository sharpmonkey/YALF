using Yalf.LogEntries;

namespace Yalf.Reporting
{
    public class MethodLogInformation
    {
        public MethodEntry LogEntry { get; private set; }
        public bool Enabled { get; private set; }

        public MethodLogInformation(MethodEntry logEntry, bool enabled)
        {
            this.LogEntry = logEntry;
            this.Enabled = enabled;
        }

    }
}
