using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yalf.Performance
{
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
