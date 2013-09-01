using System;
using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class ExceptionTrace : BaseEntry
    {
        [ProtoMember(1)]
        public readonly DateTime Time;
        [ProtoMember(2, DynamicType = true)]
        private readonly object _exception;

        public Exception Exception
        {
            get
            {
                if (_exception == null)
                    return null;

                return _exception as Exception;
            }
        }

        private ExceptionTrace() { }

        public ExceptionTrace(Exception ex, DateTime time)
        {
            Time = time;
            _exception = ex;
        }
    }
}
