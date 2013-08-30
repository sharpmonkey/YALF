using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class ExceptionTrace : BaseEntry
    {
        [ProtoMember(1)]
        public readonly DateTime Time;
        [ProtoMember(2, DynamicType = true)]
        private readonly byte[] _exception;

        public Exception Exception
        {
            get
            {
                if (_exception == null || _exception.Length <= 0)
                    return null;

                using (var ms = new MemoryStream(_exception))
                {
                    IFormatter decoder = new BinaryFormatter();
                    return decoder.Deserialize(ms) as Exception;
                }
            }
        }

        private ExceptionTrace() { }

        public ExceptionTrace(Exception ex, DateTime time)
        {
            Time = time;
            using (var ms = new MemoryStream())
            {
                IFormatter encoder = new BinaryFormatter();
                encoder.Serialize(ms, ex);
                _exception = ms.ToArray();
            }
        }
    }
}
