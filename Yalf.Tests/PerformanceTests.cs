using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Yalf.Tests.Helpers;

namespace Yalf.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        private static Assembly assembly;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            assembly = WeaverHelper.WeaveAssembly(22);
        }

        [Test]
        public void Test_Disabled()
        {
            Log.Enabled = false;
            Log.MaxEntryCount = 0;
            Log.Level = 0;

            var algo = new Dummy();
            algo.Recursive(10);

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            ilalgo.Recursive(10);

            var count = 1000000;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                algo.Recursive(10);
            }

            sw.Stop();
            var duration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                ilalgo.Recursive(10);
            }

            sw.Stop();
            var ilduration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            Console.WriteLine("{0} repeats {1:0.00}ms vs weaved {2:0.00}ms", count, duration, ilduration);
        }

        [Test]
        public void Test_Enabled()
        {
            Log.Enabled = true;
            Log.MaxEntryCount = 1000;
            Log.Level = LogLevel.Info;

            var algo = new Dummy();
            algo.Recursive(10);

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            ilalgo.Recursive(10);

            var count = 100000;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                algo.Recursive(10);
            }

            sw.Stop();
            var duration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                ilalgo.Recursive(10);
            }

            sw.Stop();
            var ilduration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            Console.WriteLine("{0} repeats {1:0.00}ms vs weaved {2:0.00}ms", count, duration, ilduration);
        }

        [Test]
        public void Test_Manual_Enabled()
        {
            Log.Enabled = true;
            Log.MaxEntryCount = 1000;
            Log.Level = LogLevel.Info;

            var algo = new Dummy();
            algo.Recursive(10);

            var ilalgo = new DummyWith();
            ilalgo.Recursive(10);

            var count = 100000;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                algo.Recursive(10);
            }

            sw.Stop();
            var duration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                ilalgo.Recursive(10);
            }

            sw.Stop();
            var ilduration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            Console.WriteLine("{0} repeats {1:0.00}ms vs manual {2:0.00}ms", count, duration, ilduration);
        }
    }

    internal class Dummy
    {
        public int Recursive(int depth)
        {
            if (depth <= 0)
            {
                Log.Info("That's it, ran out of depth");
                return 0;
            }

            return depth + Recursive(depth - 1);
        }
    }

    internal class DummyWith
    {
        public int Recursive(int depth)
        {
            var context = Log.MethodContext("Yalf.Tests.Dummy.Recursive", depth);
            try
            {
                if (depth <= 0)
                {
                    Log.Info("That's it, ran out of depth");
                    return context.RecordReturn(0);
                }

                return context.RecordReturn(depth + Recursive(depth - 1));
            }
            catch (Exception ex)
            {
                Log.TraceException(ex);
                throw;
            }
            finally
            {
                context.Dispose();
            }
        }
    }
}
