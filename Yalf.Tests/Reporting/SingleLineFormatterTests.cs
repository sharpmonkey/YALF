using System;
using NUnit.Framework;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;

namespace Yalf.Tests.Reporting
{
    public class SingleLineFormatterTests
    {

        [Test]
        public void FormatMethodEntry_ValidLog_NullIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
           
            // Act
            var outputText = formatter.FormatMethodEntry(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Null, "Expected nothing to be returned for a method entry, everything is printed in the methodExit handling.");
        }

        [Test]
        public void FormatMethodExit_ValidLog_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "Yalf.TestMethod(returnVal) started 22:22:31.678 duration 345ms";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_ReturnRecordedIsFalse_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, false, "returnVal");
            var expectedText = "Yalf.TestMethod() started 22:22:31.678 duration 345ms";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_HideMethodReturnValueIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithMethodReturnValueHidden().Build();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "Yalf.TestMethod() started 22:22:31.678 duration 345ms";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_HideMethodDurationIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithMethodDurationHidden().Build();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "Yalf.TestMethod(returnVal) started 22:22:31.678";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void FormatMethodExit_HideTimeStampInMethodIsSet_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().WithTimeStampInMethodHidden().Build();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            var expectedText = "Yalf.TestMethod(returnVal) duration 345ms";

            var relatedEntry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);

            // Assert
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }


        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_NoEntryMethodSupplied_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            var relatedEntry = new MethodEntry(1, "Yalf.DifferentMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, relatedEntry, filters);

            // Act, Assert
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_SuppliedEntryMethodNameDoesNotMatch_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            // Act, Assert
            var outputText = formatter.FormatMethodExit(1, 2, entry, filters);
        }

        private ILogFilters GetDefaultFilters()
        {
            var builder = new LogFiltersBuilder();
            return builder.Build();
        }
    }
}