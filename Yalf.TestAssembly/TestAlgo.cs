using System;
using System.Threading;

namespace Yalf.TestAssembly
{
    public class TestAlgo
    {
        // here just for activator
        public TestAlgo()
        {
        }

        private int x;
        public TestAlgo(int value = 0)
        {
            int v = value - 1;

            x = v + 1;
        }

        public int Recursive(int depth)
        {
                if (depth <= 0)
                {
                    Log.Info("That's it, ran out of depth");
                    return 0;
                }

                return depth + Recursive(depth - 1);
        }

        public void EvenOdd(int depth)
        {
                if (depth <= 0)
                    return;

                if (depth % 2 == 0)
                {
                    Log.Info("Yeah it's even, so what");
                    EvenOdd(depth - 2);
                    return;
                }
                else
                {
                    EvenOdd(depth - 1);
                    EvenOdd(depth - 2);
                    return;
                }
        }

        public void LotsOfLocalVars()
        {
            var i = 1;
            var b = i == 0;
            TestAlgo someObject1;
            String someObject2;
            var someObject3 = new TestSignatures();
            var cont = new object();
            var r = new Random();
            var t = this.GetType();

            if (r.NextDouble() > 0.5)
            {
                i++;
                b = false;
                cont = null;
                if (i == 1)
                {
                    var s = t.Name;
                    someObject1 = null;
                    someObject2 = null;
                    someObject3 = null;
                }
            }
        }

        public void SpinAThreadForWithException()
        {
                var t = new Thread(WithException);

                t.Name = "ExceptionThread";

                t.Start();

                t.Join();
        }

        public void WithException()
        {
                for (int i = 5; i >= 0; i--)
                {
                    DoSomethingExceptional(i);
                }
        }
        
        private int DoSomethingExceptional(int value)
        {
                var x = 10 / value;

                return x;
        }

        public void SampleWith()
        {
            SampleWithout();
        }

        public void SampleWithout()
        {
            return;
        }

        public void QueueBackgroundThread()
        {
            ThreadPool.QueueUserWorkItem(DummyThreadWork);

            Thread.Sleep(1200);
        }

        public void DummyThreadWork(object ignore)
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
            }
        }
      
    }
}