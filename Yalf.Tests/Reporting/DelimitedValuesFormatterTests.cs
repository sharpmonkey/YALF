using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;

namespace Yalf.Tests.Reporting
{
    public class DelimitedValuesFormatterTests
    {
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void FormatThread_ValidLog_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();

            var entry1 = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            var entry2 = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var entry = new ThreadData(22, String.Empty, new BaseEntry[] { entry1, entry2 });

            // Act. Assert
            var outputText = formatter.FormatThread(entry, filters);
        }


        [Test]
        public void FormatMethodEntry_ValidLog_NullIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));

            // Act
            var outputText = formatter.FormatMethodEntry(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Null, "Expected nothing to be returned for a method entry, everything is printed in the methodExit handling.");
        }

        [Test]
        public void FormatMethodExit_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "Yalf,Method,Yalf.TestMethod,returnVal,22:22:31.678,345,2,1";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_ReturnRecordedIsFalse_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, false, "returnVal");
            var expectedText = "Yalf,Method,Yalf.TestMethod,,22:22:31.678,345,2,1";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }


        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_NoEntryMethodSupplied_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            var relatedEntry = new MethodEntry(1, "Yalf.DifferentMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act, Assert
            var outputText = formatter.FormatMethodExit(1, 2, 33, entry, filters);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_SuppliedEntryMethodNameDoesNotMatch_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            // Act, Assert
            var outputText = formatter.FormatMethodExit(1, 2, 33, entry, filters);
        }


        [Test]
        public void FormatLogEvent_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new LogEvent(LogLevel.Info, DateTime.Parse("2022-10-22 22:22:31.678"), "This is a log entry");

            var expectedText = "Yalf,Log,This is a log entry,,22:22:31.678,0,1,22";

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
            var formatter = new DelimitedValuesFormatter();
            var ex = this.GenerateExceptionWithStackTrace();
            var entry = new ExceptionTrace(ex, DateTime.Parse("2022-10-22 22:22:31.678"));

            var expectedText = @"Yalf,Exception,Attempted to divide by zero.,   at Yalf.Tests.Reporting.DelimitedValuesFormatterTests.GenerateExceptionWithStackTrace() in d:\repositories\yalf\Yalf.Tests\Reporting\DelimitedValuesFormatterTests.cs:line xxx,22:22:31.678,0,1,22";
            expectedText = expectedText.Replace(@"d:\repositories\yalf", this.GetRootFolder());

            // Act
            var outputText = formatter.FormatException(22, 1, 33, entry, filters);

            // Assert
            outputText = Regex.Replace(outputText, @"DelimitedValuesFormatterTests\.cs\:line [0-9]+,", "DelimitedValuesFormatterTests.cs:line xxx,");

            Console.WriteLine(outputText);
            Console.WriteLine(System.Environment.CurrentDirectory);
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText).IgnoreCase, "Not the expected output text, you may need to adjust the test if the formatter has been changed - it could be the line number of the mock exception has changed.");
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

        private string GetRootFolder()
        {
            var folder = System.Environment.CurrentDirectory;
            return folder.Remove(folder.IndexOf(@"\Yalf.Tests", StringComparison.InvariantCultureIgnoreCase));
        }

        private ILogFilters GetDefaultFilters()
        {
            var builder = new LogFiltersBuilder();
            return builder.Build();
        }
    }
}