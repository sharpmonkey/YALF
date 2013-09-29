using System;
using NUnit.Framework;
using Yalf.LogEntries;

namespace Yalf.Tests.LogEntries
{
    [TestFixture]
    public class BaseEntryTest : ProtobufTest<BaseEntry>
    {      
        [Test]
        public void LogEvent_Rountrip()
        {
            // arrange
            var entry = new LogEvent(LogLevel.Info, DateTime.Now, "message");

            // act
            var deserialized = SerializationRoundtrip(entry);

            // assert
            Assert.IsNotNull(deserialized);
        }

        [Test]
        public void MethodEntry_Rountrip()
        {
            // arrange
            var entry = new MethodEntry(1, "method", null, DateTime.Now);

            // act
            var deserialized = SerializationRoundtrip(entry);

            // assert
            Assert.IsNotNull(deserialized);
        }

        [Test]
        public void MethodExit_Rountrip()
        {
            // arrange
            var entry = new MethodExit(2, "method", 11, true, "value");

            // act
            var deserialized = SerializationRoundtrip(entry);

            // assert
            Assert.IsNotNull(deserialized);
        }

        [Test]
        public void ThreadData_Rountrip()
        {
            // arrange
            var entry = new ThreadData(1, "tread name", new BaseEntry[] { new LogEvent(LogLevel.Info, DateTime.Now, "message") });

            // act
            var deserialized = SerializationRoundtrip(entry);

            // assert
            Assert.IsNotNull(deserialized);
        }

        [Test]
        public void ExceptionTrace_Rountrip()
        {
            // arrange
            var entry = new ExceptionTrace(new Exception("test"), DateTime.Now);

            // act
            var deserialized = SerializationRoundtrip(entry);

            // assert
            Assert.IsNotNull(deserialized);
        }
    }
}
