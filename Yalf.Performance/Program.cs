using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yalf.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Enabled = true;
            Log.MaxEntryCount = 1000;
            Log.Level = LogLevel.Info;


            var count = 100000;

            var duration = MeasureDummy(count);
            var ilduration = MeasureDummyWith(count);

            Console.WriteLine("{0} repeats {1:0.00}ms vs manual {2:0.00}ms", count, duration, ilduration);
        }

        private static double MeasureDummy(int count)
        {
            var algo = new Dummy();
            algo.Recursive(10);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                algo.Recursive(10);
            }

            sw.Stop();
            var duration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            return duration;
        }

        private static double MeasureDummyWith(int count)
        {
            var algo = new DummyWith();
            algo.Recursive(10);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                algo.Recursive(10);
            }

            sw.Stop();
            var duration = 1000.0 * sw.ElapsedTicks / Stopwatch.Frequency;

            return duration;
        }
    }
}
