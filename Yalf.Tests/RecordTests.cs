using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Yalf.Sample;
using Yalf.Tests.Helpers;

namespace Yalf.Tests
{
    [TestFixture]
    public class RecordTests
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
        public void Test_Recursive_Enabled()
        {
            var algo = new TestAlgo();

            algo.Recursive(10);

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

        [Test]
        public void Test_EvenOdd()
        {
            var algo = new TestAlgo();

            algo.EvenOdd(5);

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

        [Test]
        public void Test_Recursive_Disabled()
        {
            Log.Enabled = false;

            var algo = new TestAlgo();

            algo.Recursive(10);

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

        [Test]
        public void Test_Recursive_Threaded()
        {
            var t1 = new Thread(RunEvenOdd);
            var t2 = new Thread(RunEvenOdd);
            var t3 = new Thread(RunEvenOdd);

            t1.Name = "FirstThread";
            t2.Name = "[Main]";

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }
        
        private void RunEvenOdd()
        {
            var depth = Thread.CurrentThread.ManagedThreadId;
            var algo = new TestAlgo();
            algo.EvenOdd(depth);
        }

        [Test]
        public void Test_CrossThreadException()
        {
            try
            {
                var algo = new TestAlgo();
                algo.SpinAThreadForWithException();
            } 
            catch 
            { 
            }

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

        private void RunException()
        {
            var algo = new TestAlgo();
            algo.WithException();
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

            Assert.IsTrue(exception);

            var context = Log.DumpInMemory();

            var output = LogPrinter.Print(context);

            Console.WriteLine(output);
        }

    }
}

/*

var context = Log.MethodContext("MethodName");
try
{
    // original code
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
 
 */
