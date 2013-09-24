using System;
using NUnit.Framework;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Tests.Reporting
{
    public class DefaultOutputHandlerTests
    {

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ctor_NoFiltersSupplied_ExceptionIsThrown()
        {
// ReSharper disable UnusedVariable
            var dummy = new DefaultOutputHandler(null);
// ReSharper restore UnusedVariable
        }

        [Test]
        public void ctor_Default_HasDefaultFormatterAsFormatter()
        {
            // Arrange
            var output = new DefaultOutputHandler(this.GetDefaultFilters());

            // Act
            var formatter = output.Formatter;

            // Assert
            Assert.That(formatter, Is.Not.Null, "Expected a default formatter to be created if one is not supplied.");
            Assert.That(formatter, Is.TypeOf<DefaultFormatter>(), "Expected default formatter to be a DefaultFormatter formatter.  Be careful changing this as code may depend on the default.");
        }

        [Test]
        public void HandleMethodEntry_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var expectedText = String.Concat("".PadLeft(2, output.Formatter.IndentChar), "[Enter] 22:22:31.678 Yalf.TestMethod(param1, param2)", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleMethodEntry(entry, 1, true);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleMethodExit_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = String.Concat("".PadLeft(2, output.Formatter.IndentChar), "[Exit] Yalf.TestMethod(returnVal) duration 345ms", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleMethodExit(entry, 1, true);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleThread_ValidLogWithThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, "YalfThread", new BaseEntry[] { entry1, entry2 });

            var expectedText = String.Concat("[Thread 22 'YalfThread']", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleThread(entry);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(output.CurrentThreadId, Is.EqualTo(22), "Not the expected thread id for the current thread.");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleThread_ValidLogWithNullThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, null, new BaseEntry[] { entry1, entry2 });

            var expectedText = String.Concat("[Thread 22]", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleThread(entry);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(output.CurrentThreadId, Is.EqualTo(22), "Not the expected thread id for the current thread.");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleThread_ValidLogWithBlankThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, String.Empty, new BaseEntry[] { entry1, entry2 });

            var expectedText = String.Concat("[Thread 22]", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleThread(entry);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(output.CurrentThreadId, Is.EqualTo(22), "Not the expected thread id for the current thread.");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleLogEntry_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new LogEvent(LogLevel.Info, DateTime.Parse("2022-10-22 22:22:31.678"), "This is a log entry");

            var expectedText = String.Concat("".PadLeft(2, output.Formatter.IndentChar), "[Log] [Info] This is a log entry", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleLogEvent(entry, 1, true);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleException_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var ex = this.GenerateExceptionWithStackTrace();
            var entry = new ExceptionTrace(ex, DateTime.Parse("2022-10-22 22:22:31.678"));

            var expectedText = String.Concat("".PadLeft(2, output.Formatter.IndentChar), "[Exception] 22:22:31.678 Attempted to divide by zero.", Environment.NewLine);
            output.Initialise();

            // Act
            output.HandleException(entry, 1);
            output.Complete();

            // Assert
            var outputText = output.GetResults();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        private Exception GenerateExceptionWithStackTrace()
        {
            try
            {
                int start = 0;
                int end = 32/start;

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }


        private ILogFilters GetDefaultFilters()
        {
            var builder = new LogFiltersBuilder();
            return builder.Build();
        }
    }
}

