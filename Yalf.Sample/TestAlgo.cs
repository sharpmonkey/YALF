using System;
using System.Threading;

namespace Yalf.Sample
{
    public class TestAlgo
    {
        public TestAlgo()
        {
            var context = Log.MethodContext(".ctor");
            context.Dispose();
        }

        private int x;
        public TestAlgo(int value = 0)
        {
            var context = Log.MethodContext(".ctor", value);
            try
            {
                int v = value - 1;

                x = v + 1;
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

        public int Recursive(int depth)
        {
            var context = Log.MethodContext("Recursive", depth);
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

        public void LotsOfLocalVars()
        {
            var context = Log.MethodContext("LotsOfLocalVars");
            try
            {
                var i = 1;
                var b = i == 0;
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
                    }
                }
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

        public void EvenOdd(int depth)
        {
            var context = Log.MethodContext("EvenOdd", depth);
            try
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

        public void SpinAThreadForWithException()
        {
            var context = Log.MethodContext("SpinAThreadForWithException");
            try
            {
                var t = new Thread(WithException);

                t.Name = "ExceptionThread";

                t.Start();

                t.Join();
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

        public void WithException()
        {
            var context = Log.MethodContext("WithException");
            try
            {
                for (int i = 5; i >= 0; i--)
                {
                    DoSomethingExceptional(i);
                }
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

        private int DoSomethingExceptional(int value)
        {
            var context = Log.MethodContext("DoSomethingExceptional", value);
            try
            {
                var x = 10 / value;

                return context.RecordReturn(x);
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

        public void SampleWith()
        {
            var context = Log.MethodContext("SampleWith");
            try
            {
                return;
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

        public void SampleWithout()
        {
            return;
        }
    }
}