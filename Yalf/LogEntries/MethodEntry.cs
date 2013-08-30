using System;
using ProtoBuf;

namespace Yalf.LogEntries
{
    [ProtoContract]
    public class MethodEntry : BaseEntry
    {
        [ProtoMember(1)]
        public readonly int StackLevel;
        [ProtoMember(2)]
        public readonly string MethodName;
        [ProtoMember(3)]
        public readonly string[] Arguments;
        [ProtoMember(4)]
        public readonly DateTime Time;

        private MethodEntry() { }

        public MethodEntry(int stackLevel, string methodName, string[] arguments, DateTime time)
        {
            StackLevel = stackLevel;
            MethodName = methodName;
            Arguments = arguments;
            Time = time;
        }
    }
}