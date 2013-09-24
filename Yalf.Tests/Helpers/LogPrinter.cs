using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yalf.LogEntries;

namespace Yalf.Tests.Helpers
{
    public static class LogPrinter
    {
        public static string Print(IEnumerable<BaseEntry> entries, int level = 0)
        {
            var sb = new StringBuilder();
            foreach (var entry in entries)
            {
                var type = entry.GetType();

                if (type == typeof (ThreadData))
                {
                    sb.Append(PrintThread((ThreadData) entry));
                }
                else if (type == typeof (MethodEntry))
                {
                    sb.Append(PrintMethodEntry((MethodEntry)entry, level++));
                }
                else if (type == typeof(MethodExit))
                {
                    sb.Append(PrintMethodExit((MethodExit)entry, --level));
                }
                else if (type == typeof(LogEvent))
                {
                    sb.Append(PrintLogEntry((LogEvent)entry, level));
                }
                else if (type == typeof(ExceptionTrace))
                {
                    sb.Append(PrintExceptionTrace((ExceptionTrace)entry, level));
                }
            }

            return sb.ToString();
        }

        private static string PrintExceptionTrace(ExceptionTrace entry, int level)
        {
            var str = string.Format("[Exception] {0:dd/MM/yyyy HH:mm:ss.fff} {1}", entry.Time, entry.Message);

            return Indent(level) + str + Environment.NewLine;
        }

        private static string PrintLogEntry(LogEvent entry, int level)
        {
            var str = string.Format("[Log] [{0}] {1}", entry.Level, entry.Message);

            return Indent(level) + str + Environment.NewLine;
        }

        private static string PrintMethodEntry(MethodEntry entry, int level)
        {
            var args = entry.Arguments == null ? "" : string.Join(", ", entry.Arguments);
            var str = string.Format("[Enter] {0:dd/MM/yyyy HH:mm:ss.fff} {1}({2})", entry.Time, entry.MethodName, args);

            return Indent(level) + str + Environment.NewLine;
        }

        private static string PrintMethodExit(MethodExit entry, int level)
        {
            var returnValue = entry.ReturnRecorded ? "(" + entry.ReturnValue + ")" : "";
            var str = string.Format("[Exit] {0}{1} duration {2:0.####}ms", entry.MethodName, returnValue, entry.ElapsedMs);

            return Indent(level) + str + Environment.NewLine;
        }

        private static string PrintThread(ThreadData entry)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[Thread] {0} '{1}'", entry.ThreadId, entry.ThreadName);
            sb.AppendLine();

            if(entry.Entries != null && entry.Entries.Any())
                sb.Append(Print(entry.Entries));

            return sb.ToString();
        }

        private static string Indent(int level)
        {
            var lvl = Math.Max(0, level);
            return "".PadLeft(lvl * 2, ' ');
        }
    }
}
