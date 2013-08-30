using System;

namespace Yalf
{
    public interface IContext : IDisposable
    {
        T RecordReturn<T>(T value);
        void Dispose();
    }
}