using System;

namespace Yalf
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Verbose = 1,
        Debug = 2,
        Info = 4,
        Warning = 8,
        Error = 16,
        Custom1 = 32,
        Custom2 = 64,
        Custom3 = 128,
    }
}