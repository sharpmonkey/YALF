using System;
using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class ExceptionTrace : BaseEntry
    {
        [ProtoMember(1)]
        public readonly DateTime Time;
        [ProtoMember(2)]
        public readonly string Message;
        [ProtoMember(3)]
        public readonly string Type;
        [ProtoMember(4)]
        public readonly string StackTrace;

        private ExceptionTrace() { }

        public ExceptionTrace(Exception ex, DateTime time)
        {
            Time = time;
            Message = ex.Message;
            Type = ex.GetType().FullName;
            StackTrace = ex.StackTrace;
        }
    }
}
