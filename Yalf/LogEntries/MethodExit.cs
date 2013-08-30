using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class MethodExit : BaseEntry
    {
        [ProtoMember(1)]
        public readonly int StackLevel;
        [ProtoMember(2)]
        public readonly string MethodName;
        [ProtoMember(3)]
        public readonly double ElapsedMs;
        [ProtoMember(4)]
        public readonly bool ReturnRecorded;
        [ProtoMember(5)]
        public readonly string ReturnValue;

        private MethodExit() { }

        public MethodExit(int stackLevel, string methodName, double elapsedMs, bool returnRecorded, string returnValue)
        {
            StackLevel = stackLevel;
            MethodName = methodName;
            ElapsedMs = elapsedMs;
            ReturnRecorded = returnRecorded;
            ReturnValue = returnValue;
        }
    }
}