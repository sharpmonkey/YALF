using System;
using System.Collections.Generic;
using System.Linq;

namespace Yalf.TestAssembly
{
    public class TestSignatures
    {
        public static List<T> GetAll<T>()
        {
            var context = Log.MethodContext("GetAll");
            try
            {
                return new List<T>();
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

        public static void GenericWithSomeParameters<T>(T item, int index, IEnumerable<byte> someEnumerable)
        {
            var context = Log.MethodContext("GenericWithSomeParameters", item, index, someEnumerable);
            try
            {
                var b = index + 1;
                if (someEnumerable.Count() > b)
                    return;

                if (Equals(item, default(T)))
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
    }
}
