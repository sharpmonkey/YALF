using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Yalf.Sample;
using Yalf.Tests.Helpers;

namespace Yalf.Tests
{
    [TestFixture]
    public class ModuleWeaverTests
    {
        private static Assembly assembly;

        [TestFixtureSetUp]
        public static void ClassInitialize()
        {
            assembly = WeaverHelper.WeaveAssembly();
        }

        [SetUp]
        public void Setup()
        {
            Log.Clear();
            Log.Level = LogLevel.Info;
            Log.MaxEntryCount = 100;
            Log.Enabled = true;
        }

        [Test, Ignore]
        public void Weave_CustomAssembly_WeavesCorrectIL()
        {
            var path = Path.GetFullPath(@"..\..\..\..\GraphDhtDatabase\GraphDht\bin\Debug\GraphDht.dll");
            var customAssembly = WeaverHelper.WeaveAssembly(path);
            var assemblyPath = customAssembly.CodeBase.Remove(0, 8);
            var result = Verifier.Verify(assemblyPath);

            Console.WriteLine(result);

            Assert.IsTrue(result.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)));
        }

        [Test]
        public void ModuleWeaverExecuteWeavesCorrectIL()
        {
            var assemblyPath = assembly.CodeBase.Remove(0, 8);
            var result = Verifier.Verify(assemblyPath);

            Console.WriteLine(result);

            Assert.IsTrue(result.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath)));
        }

        private string Clean(string input)
        {
            var output = input.Replace("Yalf.TestAssembly.TestAlgo.", ""); // strip namespace and class name 
            output = Regex.Replace(output, @"[0-9]+([\.\,][0-9]{1,4})?ms", "_duration_in_ms_"); // strip durations
            output = Regex.Replace(output, @"\d\d/\d\d/\d\d\d\d \d\d:\d\d:\d\d.\d\d\d", "_datetime_"); // strip date/time entries
            return output;
        }

        private string GetOutput()
        {
            var context = Log.DumpInMemory();
            var output = LogPrinter.Print(context);
            output = Clean(output);
            Log.Clear();
            return output;
        }

        [Test]
        public void Test_Constructor()
        {
            var algo = new TestAlgo(1);
            var output = GetOutput();

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly, 1);
            var iloutput = GetOutput();

            var equal = String.Compare(output, iloutput, StringComparison.OrdinalIgnoreCase) == 0;
            Assert.That(equal, Is.True);
        }

        [Test]
        public void Test_Recursive_Enabled()
        {
            var algo = new TestAlgo();
            algo.Recursive(10);
            var output = GetOutput();

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            ilalgo.Recursive(10);
            var iloutput = GetOutput();

            var equal = String.Compare(output, iloutput, StringComparison.OrdinalIgnoreCase) == 0;
            Assert.That(equal, Is.True);
        }


        [Test]
        public void Test_EvenOdd()
        {
            var algo = new TestAlgo();
            algo.EvenOdd(5);
            var output = GetOutput();

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            ilalgo.EvenOdd(5);
            var iloutput = GetOutput();

            var equal = String.Compare(output, iloutput, StringComparison.OrdinalIgnoreCase) == 0;
            Assert.That(equal, Is.True);
        }

        [Test]
        public void Test_LotsOfLocalVars()
        {
            var algo = new TestAlgo();
            algo.LotsOfLocalVars();
            var output = GetOutput();

            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            ilalgo.LotsOfLocalVars();
            var iloutput = GetOutput();

            var equal = String.Compare(output, iloutput, StringComparison.OrdinalIgnoreCase) == 0;
            Assert.That(equal, Is.True);
        }

        [Test]
        public void Test_Exception()
        {
            var algo = new TestAlgo();
            bool exception = false;
            try
            {
                algo.WithException();
            }
            catch
            {
                exception = true;
            }
            var output = GetOutput();


            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);
            bool ilexception = false;
            try
            {
                ilalgo.WithException();
            }
            catch
            {
                ilexception = true;
            }
            var iloutput = GetOutput();

            Assert.That(exception, Is.True);
            Assert.That(ilexception, Is.True);

            var equal = String.Compare(output, iloutput, StringComparison.OrdinalIgnoreCase) == 0;
            Assert.That(equal, Is.True);
        }


        [Test]
        public void Test_BackgroundThread()
        {
            var ilalgo = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestAlgo>(assembly);

            ilalgo.QueueBackgroundThread();

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

        [Test]
        public void Test_Signatures_RefParams()
        {
            var ilsignatures = WeaverHelper.CreateInstance<Yalf.TestAssembly.TestSignatures>(assembly);

            int intValue = 1;
            double dblValue = 0.1;
            string strValue = "value";
            object objValue = new object();

            var compare = ilsignatures.ReferenceParams(1, ref intValue, ref dblValue, ref strValue, ref objValue);

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);

            Assert.IsTrue(compare);
        }
    }
}
