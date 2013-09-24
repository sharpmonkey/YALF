using System;
using NUnit.Framework;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;

namespace Yalf.Tests.Reporting
{
    public class DefaultFormatterTests
    {

        [Test]
        public void FormatMethodEntry_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var expectedText = "[Enter] 22:22:31.678 Yalf.TestMethod(param1, param2)";

            // Act
            var outputText = formatter.FormatMethodEntry(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodEntry_HideMethodParametersIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithMethodParametersHidden().Build();
            var formatter = new DefaultFormatter();
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var expectedText = "[Enter] 22:22:31.678 Yalf.TestMethod()";

            // Act
            var outputText = formatter.FormatMethodEntry(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodEntry_HideTimeStampInMethodIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithTimeStampInMethodHidden().Build();
            var formatter = new DefaultFormatter();
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var expectedText = "[Enter] Yalf.TestMethod(param1, param2)";

            // Act
            var outputText = formatter.FormatMethodEntry(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "[Exit] Yalf.TestMethod(returnVal) duration 345ms";

            // Act
            var outputText = formatter.FormatMethodExit(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_ReturnRecordedIsFalse_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, false, "returnVal");
            var expectedText = "[Exit] Yalf.TestMethod() duration 345ms";

            // Act
            var outputText = formatter.FormatMethodExit(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_HideMethodReturnValueIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithMethodReturnValueHidden().Build();
            var formatter = new DefaultFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "[Exit] Yalf.TestMethod() duration 345ms";

            // Act
            var outputText = formatter.FormatMethodExit(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_HideMethodDurationIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithMethodDurationHidden().Build();
            var formatter = new DefaultFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "[Exit] Yalf.TestMethod(returnVal)";

            // Act
            var outputText = formatter.FormatMethodExit(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }



        [Test]
        public void FormatThread_ValidLogWithThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, "YalfThread", new BaseEntry[] { entry1, entry2 });

            var expectedText = "[Thread 22 'YalfThread']";

            // Act
            var outputText = formatter.FormatThread(entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatThread_ValidLogWithNullThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, null, new BaseEntry[] { entry1, entry2 });

            var expectedText = "[Thread 22]";

            // Act
            var outputText = formatter.FormatThread(entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatThread_ValidLogWithBlankThreadName_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, String.Empty, new BaseEntry[] { entry1, entry2 });

            var expectedText = "[Thread 22]";

            // Act
            var outputText = formatter.FormatThread(entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatLogEvent_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();
            var entry = new LogEvent(LogLevel.Info, DateTime.Parse("2022-10-22 22:22:31.678"), "This is a log entry");

            var expectedText = "[Log] [Info] This is a log entry";

            // Act
            var outputText = formatter.FormatLogEvent(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatException_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DefaultFormatter();
            var ex = this.GenerateExceptionWithStackTrace();
            var entry = new ExceptionTrace(ex, DateTime.Parse("2022-10-22 22:22:31.678"));

            var expectedText = "[Exception] 22:22:31.678 Attempted to divide by zero.";

            // Act
            var outputText = formatter.FormatException(22, 1, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        private Exception GenerateExceptionWithStackTrace()
        {
            try
            {
                int start = 0;
                int end = 32 / start;

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
