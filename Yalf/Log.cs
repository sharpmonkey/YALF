using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ProtoBuf;
using Yalf.LogEntries;

namespace Yalf
{
    public sealed class Log : IDisposable
    {
        #region Thread Instance

        private static readonly object _sync = new object();
        private static readonly List<Log> _logs = new List<Log>();

        [ThreadStatic] private static Log _threadInstance;
        private static Log Instance
        {
            get
            {
                if (_threadInstance == null)
                {
                    lock (_sync)
                    {
                        var threadId = Thread.CurrentThread.ManagedThreadId;
                        // TODO: No Thread.Name support in PCL
                        var threadName = string.Empty; //Thread.CurrentThread.Name;
                        _threadInstance = new Log(threadId, threadName);
                        _logs.Add(_threadInstance);
                    }
                }
                return _threadInstance;
            }
        }

        #endregion

        #region Shared settings
        /// <summary>Turns logging on or off.  Also see <see cref="EnableParameterLogging"/> and <see cref="EnableReturnValueLogging"/> properties for full logging options.</summary>
        public static bool Enabled { get; set; }
        /// <summary>Turn parameter logging on (off by default).  If true, the method parameter values are serialised and stored in the yalf log.</summary>
        /// <remarks>Logging of parameters involves serialisation to <see cref="System.String"/> which can cause performance issues for some complex types.</remarks>
        public static bool EnableParameterLogging { get; set; }
        /// <summary>Turn return value logging on (off by default).  If true, the method return value is serialised (if applicable) and stored in the yalf log.</summary>
        /// <remarks>Logging of return values involves serialisation to <see cref="System.String"/> which can cause performance issues for some complex types.</remarks>
        public static bool EnableReturnValueLogging { get; set; }

        //public static bool Enabled { get; set; }
        public static LogLevel Level { get; set; }
        public static int MaxEntryCount { get; set; }

        #endregion

        #region Static methods

        public static void Clear()
        {
            lock (_sync)
            {
                var localLogs = _logs.ToArray();

                foreach (var log in localLogs)
                {
                    log.ClearInternal();
                    log.Dispose();
                }

                _logs.Clear();
            }
        }

        public static BaseEntry[] DumpInMemory()
        {
            lock (_logs)
                return _logs.Select(l => l.DumpInternal()).ToArray();
        }

        public static byte[] DumpToBinary()
        {
            BaseEntry[] dump;
            lock (_logs)
                dump = _logs.Select(l => l.DumpInternal()).ToArray();

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, dump);

                return ms.ToArray();
            }
        }

        public static BaseEntry[] DumpFromBinary(byte[] binary)
        {
            if (binary == null || binary.Length <= 0)
                return null;

            using (var ms = new MemoryStream(binary))
            {
                var entries = Serializer.Deserialize<BaseEntry[]>(ms);
                return entries;
            }
        }

        public static IContext MethodContext(string methodName, params object[] args)
        {
            if (!Enabled)
                return EmptyContext.Empty;

            return Instance.MethodContextInternal(methodName, args);
        }

        public static void TraceException(Exception ex)
        {
            if(Enabled)
                Instance.TraceExceptionInternal(ex);
        }

        #region Logging Levels

        public static bool IsInfoEnabled { get { return Enabled && HasBitFlag(Level, LogLevel.Info); } }

        public static void Info(string format, params object[] args)
        {
            if(IsInfoEnabled)
                Instance.LogInternal(LogLevel.Info, format, args);
        }

        public static bool IsVerboseEnabled { get { return Enabled && HasBitFlag(Level, LogLevel.Verbose); } }

        public static void Verbose(string format, params object[] args)
        {
            if (IsVerboseEnabled)
                Instance.LogInternal(LogLevel.Verbose, format, args);
        }

        public static bool IsDebugEnabled { get { return Enabled && HasBitFlag(Level, LogLevel.Debug); } }

        public static void Debug(string format, params object[] args)
        {
            if (IsDebugEnabled)
                Instance.LogInternal(LogLevel.Debug, format, args);
        }

        public static bool IsWarningEnabled { get { return Enabled && HasBitFlag(Level, LogLevel.Warning); } }

        public static void Warning(string format, params object[] args)
        {
            if (IsWarningEnabled)
                Instance.LogInternal(LogLevel.Warning, format, args);
        }

        public static bool IsErrorEnabled { get { return Enabled && HasBitFlag(Level, LogLevel.Error); } }

        public static void Error(string format, params object[] args)
        {
            if (IsErrorEnabled)
                Instance.LogInternal(LogLevel.Error, format, args);
        }

        public static bool IsCustom1Enabled { get { return Enabled && HasBitFlag(Level, LogLevel.Custom1); } }

        public static void Custom1(string format, params object[] args)
        {
            if (IsCustom1Enabled)
                Instance.LogInternal(LogLevel.Custom1, format, args);
        }

        public static bool IsCustom2Enabled { get { return Enabled && HasBitFlag(Level, LogLevel.Custom2); } }

        public static void Custom2(string format, params object[] args)
        {
            if (IsCustom2Enabled)
                Instance.LogInternal(LogLevel.Custom2, format, args);
        }

        public static bool IsCustom3Enabled { get { return Enabled && HasBitFlag(Level, LogLevel.Custom3); } }

        public static void Custom3(string format, params object[] args)
        {
            if (IsCustom3Enabled)
                Instance.LogInternal(LogLevel.Custom3, format, args);
        }

        private static bool HasBitFlag(LogLevel e, LogLevel other)
        {
            return ((e & other) == other);
        }

        #endregion

        #endregion

        #region Members

        private readonly int _threadId;
        private readonly string _threadName;

        private readonly Stack<StackEntry> _methodStack;
        private readonly Queue<BaseEntry> _queue;
        
        private class StackEntry
        {
            public readonly string MethodName;
            private readonly DateTime _start;

            public StackEntry(string methodName)
            {
                MethodName = methodName;
                _start = DateTime.UtcNow;
            }

            public double StopAndGetDuration()
            {
                var stop = DateTime.UtcNow;
                var duration = stop - _start;
                // guard against clock sync adjustment
                return Math.Max(0, duration.TotalMilliseconds);
            }
        }

        #endregion

        #region Internal methods

        private Log(int threadId, string threadName)
        {
            _threadId = threadId;
            _threadName = threadName;

            _methodStack = new Stack<StackEntry>();
            var initialSize = Math.Min(1024, MaxEntryCount);  // don't make this too big, or yalf clags the app trying to initialise the queue!
            _queue = new Queue<BaseEntry>(initialSize);
        }

        private void Record(BaseEntry entry)
        {
            _queue.Enqueue(entry);

            var maxSize = Math.Max(0, MaxEntryCount);
            
            // TODO: could be more efficient
            while (_queue.Count > maxSize) _queue.Dequeue();
        }

        private BaseEntry DumpInternal()
        {
            var entries = _queue.ToArray();

            var entry = new ThreadData(_threadId, _threadName, entries);

            return entry;
        }

        private void ClearInternal()
        {
            // do not clear the stack!! -> method exit will cause stack to except on pop
            // remember we are using IDisposable action

            _queue.Clear();
        }

        #endregion

        #region Scope/Method tracing

        private static string FormatType(Type type)
        {
            if (type.IsGenericType)
                return string.Format("{0}<{1}>", type.Name.Split('`')[0], string.Join(", ", type.GetGenericArguments().Select(FormatType).ToArray()));
            else
                return type.Name;
        }

        private static string FormatValue(object arg)
        {
            try
            {
                if (arg == null)
                    return "<null>";

                if (arg.GetType().IsArray)
                {
                    var arrayType = FormatType(arg.GetType());
                    return string.Format("{0}[{1}]", arrayType.Substring(0, arrayType.Length - 2), ((Array) arg).Length);
                }

                var argAsString = arg.ToString();
                // TODO: cleanup this check for generic type (faster, prettier?)
                if (argAsString.IndexOf('`') >= 0 && argAsString == arg.GetType().ToString())
                    return FormatType(arg.GetType());

                return argAsString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private IContext MethodContextInternal(string methodName, params object[] args)
        {
            string[] strArgs = null;
            if (EnableParameterLogging && args != null)
                strArgs = GetSerialisedArguments(args);

            var entry = new MethodEntry(_methodStack.Count, methodName, strArgs, DateTime.Now);
            Record(entry);

            _methodStack.Push(new StackEntry(methodName));

            return new ContextScope(MethodExit);
        }

        private static string[] GetSerialisedArguments(object[] args)
        {
            var stringValues = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
                stringValues[i] = FormatValue(args[i]);
            return stringValues;
        }

        private void MethodExit(object value, bool recorded)
        {
            var stackEntry = _methodStack.Pop();
            var duration = stackEntry.StopAndGetDuration();
            
            string returnValue = null;
            if (EnableReturnValueLogging && recorded && value != null)
                returnValue = FormatValue(value);

            var entry = new MethodExit(_methodStack.Count, stackEntry.MethodName, duration, recorded, returnValue);

            Record(entry);
        }

        private class ContextScope : IContext
        {
            private readonly Action<object, bool> _onDispose;
            private bool _returnRecorded = false;
            private object _value;

            public ContextScope(Action<object, bool> onDispose)
            {
                _onDispose = onDispose;
            }

            public T RecordReturn<T>(T value)
            {
                _returnRecorded = true;

                _value = value;

                return value;
            }

            public void Dispose()
            {
                _onDispose(_value, _returnRecorded);
            }

        }

        private class EmptyContext : IContext
        {
            public static readonly EmptyContext Empty = new EmptyContext();

            private EmptyContext() { }

            public void Dispose() { }

            public T RecordReturn<T>(T value) { return value; }
        }

        #endregion

        #region Logging Members

        public void LogInternal(LogLevel level, string format, params object[] args)
        {
            var entry = new LogEntry(level, DateTime.Now, string.Format(format, args));

            Record(entry);
        }

        #endregion

        private void TraceExceptionInternal(Exception exception)
        {
            if (exception == null)
                return;

            var entry = new ExceptionTrace(exception, DateTime.Now);

            Record(entry);
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (_sync)
            {
                _logs.Remove(this);
                _threadInstance = null;
            }
        }

        #endregion

    }
}
