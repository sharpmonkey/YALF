using System;
using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class LogEntry : BaseEntry
    {
        [ProtoMember(1)]
        public readonly LogLevel Level;
        [ProtoMember(2)]
        public readonly DateTime Time;
        [ProtoMember(3)]
        public readonly string Message;

        private LogEntry() { }

        public LogEntry(LogLevel level, DateTime time, string message)
        {
            Level = level;
            Time = time;
            Message = message;
        }
    }
}