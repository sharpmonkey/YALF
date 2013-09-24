using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    [ProtoInclude(101, typeof(ExceptionTrace))]
    [ProtoInclude(102, typeof(LogEvent))]
    [ProtoInclude(103, typeof(MethodEntry))]
    [ProtoInclude(104, typeof(MethodExit))]
    [ProtoInclude(105, typeof(ThreadData))]
    public class BaseEntry { }
    // silly protobuf-net, doesn't work well with interfaces
}