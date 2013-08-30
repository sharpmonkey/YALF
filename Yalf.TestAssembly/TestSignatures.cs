using System;
using System.Collections.Generic;
using System.Linq;

namespace Yalf.TestAssembly
{
    public class TestSignatures
    {
        public static List<T> GetAll<T>()
        {
            return new List<T>();
        }

        public static void GenericWithSomeParameters<T>(T item, int index, IEnumerable<byte> someEnumerable)
        {
            var b = index + 1;
            if (someEnumerable.Count() > b)
                return;

            if (Equals(item, default(T)))
                return;
        }
    }

    public class ClassWithFieldsOnlyInitializedInDeclaration
    {
        public int DummyField = 1;
    }

    public class ClassWithFieldsOnlyInitializedInConstructor
    {
        public int DummyField;

        [NoLog]
        public ClassWithFieldsOnlyInitializedInConstructor() : base()
        {
            IContext context = Log.MethodContext("GraphDht.DConfiguration..ctor", new object[0]);
            try
            {
                this.DummyField = 1;
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
