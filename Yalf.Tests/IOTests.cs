using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yalf.Sample;
using Yalf.Tests.Helpers;

namespace Yalf.Tests
{
    [TestFixture]
    public class IOTests
    {
        [SetUp]
        public void Setup()
        {
            Log.Clear();
            Log.Level = LogLevel.Info;
            Log.MaxEntryCount = 100;
            Log.Enabled = true;
        }

        [Test]
        public void Test_Record_Write_Read_Display()
        {
            var algo = new TestAlgo();

            try
            {
                algo.WithException();
            }
            catch
            {
            }

            var file = Log.DumpToFile();

            var context = Log.DumpFromFile(file);

            File.Delete(file);

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

    }
}
