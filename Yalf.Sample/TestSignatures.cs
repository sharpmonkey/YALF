using System;
using System.Collections.Generic;
using System.Linq;

namespace Yalf.Sample
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

        public bool ReferenceParams(int intValue, ref int intRefValue, ref double dblRefValue, ref string strRefValue, ref object objRefValue)
        {
            var context = Log.MethodContext("ReferenceParams", intValue, intRefValue, dblRefValue, strRefValue, objRefValue);
            try
            {
                return intValue == intRefValue && dblRefValue > 0 && !string.IsNullOrEmpty(strRefValue) && objRefValue != null;
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

    public class TestGenericClass<T>
    {
        public TU GenericMethod<TU>(TU value)
        {
            var context = Log.MethodContext("Yalf.TestAssembly.TestGenericClass<T>.GenericMethod<TU>", value);
            try
            {
                return context.RecordReturn(value);
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
