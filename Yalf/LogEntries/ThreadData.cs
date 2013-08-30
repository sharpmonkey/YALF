using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class ThreadData : BaseEntry
    {
        [ProtoMember(1)]
        public readonly int ThreadId;
        [ProtoMember(2)]
        public readonly string ThreadName;
        [ProtoMember(3)]
        public readonly BaseEntry[] Entries;

        private ThreadData() { }

        public ThreadData(int threadId, string threadName, BaseEntry[] entries)
        {
            ThreadId = threadId;
            ThreadName = threadName;
            Entries = entries;
        }
    }
}