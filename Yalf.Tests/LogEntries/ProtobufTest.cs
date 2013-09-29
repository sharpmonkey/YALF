using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using ProtoBuf;

namespace Yalf.Tests.LogEntries
{
    [TestFixture]
    public abstract class ProtobufTest<T> where T : class
    {
        private HashSet<string> _descendants = null;

        [TestFixtureSetUp]
        public void Setup()
        {
            var baseType = typeof(T);

            _descendants = new HashSet<string>();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(t => t.IsSubclassOf(baseType));
            foreach (var type in types)
            {
                var typeName = type.Name;

                var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (constructors.All(c => c.GetParameters().Length != 0))
                    throw new InvalidProgramException(string.Format("Type '{0}' missing default (empty) constructor", typeName));

                _descendants.Add(typeName);
            }

            if (!_descendants.Any())
                throw new InvalidOperationException(string.Format("Could not find any {0} descendants", baseType.Name));
        }

        [TestFixtureTearDown]
        public void TearDownAndCheckForMissingTests()
        {
            if (_descendants.Any())
            {
                throw new MissingMethodException(string.Format("Missing tests for: {0}", string.Join(", ", _descendants)));
            }
        }

        protected U SerializationRoundtrip<U>(U value) where U : T
        {
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                bytes = ms.ToArray();
            }

            T deserialized = null;
            using (var ms = new MemoryStream(bytes))
            {
                deserialized = Serializer.Deserialize<T>(ms);
            }

            _descendants.Remove(typeof(U).Name);

            return (U)deserialized;
        }
    }
}
