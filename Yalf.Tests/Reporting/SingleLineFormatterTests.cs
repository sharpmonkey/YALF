using System;
using System.Collections.Generic;
using System.Linq;
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
            var outputText = formatter.FormatMethodEntry(1, 2, 33, entry, filters);

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
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText.Count, Is.EqualTo(1), "Expected one string to be returned");
            Assert.That(outputText[0], Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText.Count, Is.EqualTo(1), "Expected one string to be returned");
            Assert.That(outputText[0], Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText.Count, Is.EqualTo(1), "Expected one string to be returned");
            Assert.That(outputText[0], Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText.Count, Is.EqualTo(1), "Expected one string to be returned");
            Assert.That(outputText[0], Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(outputText.Count, Is.EqualTo(1), "Expected one string to be returned");
            Assert.That(outputText[0], Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }


        [Test]
        public void FormatMethodExit_MixedNestting_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().Build();
            var formatter = new SingleLineFormatter();
            var startDateTime = DateTime.Now;
            int threadId = 22;
            int lineNo = 0;
            int indentLevel = 0;

            var expectedText = (new string[]
                                    {
                                        string.Format("TopLevelMethod(blackSheep) started {0:HH:mm:ss.fff} duration 233ms", startDateTime),
                                        string.Format("FirstMethod() started {0:HH:mm:ss.fff} duration 200ms", startDateTime.AddSeconds(12)),
                                        string.Format("SecondMethod() started {0:HH:mm:ss.fff} duration 178ms", startDateTime.AddSeconds(45)),
                                        string.Format("[Log] [Info] Information log message here"),
                                        string.Format("[Exception] {0:HH:mm:ss.fff} Test the log\r\nParameter name: lineNo", startDateTime.AddSeconds(53)),
                                        string.Format("ThirdMethod() started {0:HH:mm:ss.fff} duration 100ms", startDateTime.AddSeconds(75)),
                                        string.Format("TopLevelMethod2(whiteSheep) started {0:HH:mm:ss.fff} duration 488ms", startDateTime.AddSeconds(99))
                                    }
                               ).ToList();

            var entries = new BaseEntry[]
                {
                    new MethodEntry(1, "TopLevelMethod", null, startDateTime),
                    new MethodExit(1, "TopLevelMethod", 233, true, "blackSheep"),
                    new MethodEntry(1, "FirstMethod", null, startDateTime.AddSeconds(12)),
                    new MethodEntry(2, "SecondMethod", null, startDateTime.AddSeconds(45)),
                    new LogEvent(LogLevel.Info, startDateTime.AddSeconds(47), "Information log message here"), 
                    new ExceptionTrace(new ArgumentNullException("lineNo", "Test the log"), startDateTime.AddSeconds(53)), 
                    new MethodEntry(3, "ThirdMethod", null, startDateTime.AddSeconds(75)),
                    new MethodExit(3, "ThirdMethod", 100, false, null),
                    new MethodExit(2, "SecondMethod", 178, false, null),
                    new MethodExit(1, "FirstMethod", 200, false, null),
                    new MethodEntry(1, "TopLevelMethod2", null, startDateTime.AddSeconds(99)),
                    new MethodExit(1, "TopLevelMethod2", 488, true, "whiteSheep"),
                };

            var actualText = new List<String>();

            // Act
            foreach (var entry in entries)
            {
                if (entry is MethodEntry)
                    formatter.FormatMethodEntry(threadId, indentLevel++, ++lineNo, (entry as MethodEntry), filters);
                else if (entry is MethodExit)
                {
                    var result = formatter.FormatMethodExitDelayed(threadId, indentLevel--, lineNo++, (entry as MethodExit), filters);
                    if (result != null)
                        actualText.AddRange(result);
                }
                else if (entry is LogEvent)
                {
                    var result = formatter.FormatLogEvent(threadId, indentLevel, lineNo++, (entry as LogEvent), filters);
                    if (result != null)
                        actualText.Add(result);
                }
                else if (entry is ExceptionTrace)
                {
                    var result = formatter.FormatException(threadId, indentLevel, lineNo++, (entry as ExceptionTrace), filters);
                    if (result != null)
                        actualText.Add(result);
                }
            }


            // Assert
            Assert.That(actualText.Count, Is.EqualTo(expectedText.Count), "Expected {0} lines to be returned overall, but have {1}.", expectedText.Count, actualText.Count);
            var misMatchedResults = actualText.Where((at, i) => (at != expectedText[i]));
            Assert.That(misMatchedResults.Count(), Is.EqualTo(0), "{0} of the lines do not match.\nExpected:\n{1}\n\nActual:\n{2}"
                                                                , misMatchedResults.Count()
                                                                , String.Join(Environment.NewLine, expectedText.ToArray())
                                                                , String.Join(Environment.NewLine, actualText.ToArray())
                        );
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_NoEntryMethodSupplied_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            var relatedEntry = new MethodEntry(1, "Yalf.DifferentMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            formatter.FormatMethodEntry(1, 2, 33, relatedEntry, filters);

            // Act, Assert
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_SuppliedEntryMethodNameDoesNotMatch_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            // Act, Assert
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);
        }

        [Test, ExpectedException(typeof(NotImplementedException))]
        public void FormatMethodExit_CallMade_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new SingleLineFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            // Act, Assert
            var outputText = formatter.FormatMethodExit(1, 2, 33, entry, filters);
        }


        [Test]
        public void FormatMethodExitDelayed_NestedMethodCalls_CallsAreInCorrectOrder()
        {
            // Arrange
            var formatter = new SingleLineFormatter();
            var startDateTime = DateTime.Now;
            var entries = new BaseEntry[]
                {
                    new MethodEntry(1, "FirstMethod", null, startDateTime),
                    new MethodEntry(2, "SecondMethod", null, startDateTime.AddSeconds(45)),
                    new MethodEntry(3, "ThirdMethod", null, startDateTime.AddSeconds(75)),
                    new MethodExit(3, "ThirdMethod", 100, false, null),
                    new MethodExit(2, "SecondMethod", 178, false, null),
                    new MethodExit(1, "FirstMethod", 200, false, null)
                };

            var expectedText = (new string[]
                                    {
                                        string.Format("FirstMethod() started {0:HH:mm:ss.fff} duration 200ms", startDateTime),
                                        string.Format("SecondMethod() started {0:HH:mm:ss.fff} duration 178ms", startDateTime.AddSeconds(45)),
                                        string.Format("ThirdMethod() started {0:HH:mm:ss.fff} duration 100ms", startDateTime.AddSeconds(75))
                                    }
                               ).ToList();

            List<String> output = new List<string>();
            var filters = this.GetDefaultFilters();
            var lineNo = 0;

            // Act
            foreach (var entry in entries)
            {
                if (entry is MethodEntry)
                    formatter.FormatMethodEntry(22, (entry as MethodEntry).StackLevel, ++lineNo, (MethodEntry)entry, filters);
                else if (entry is MethodExit)
                {
                    var result = formatter.FormatMethodExitDelayed(22, (entry as MethodExit).StackLevel, lineNo, (MethodExit)entry, filters);
                    if (result != null)
                        output.AddRange(result);

                    lineNo--;
                }
            }

            // Assert
            Assert.That(output.Count, Is.EqualTo(3), "Expected only three lines to be returned");
            for (int index = 0; index < 3; index++)
            {
                Assert.That(output[index], Is.EqualTo(expectedText[index]), "Not the expected text for line {0}", index + 1);
            }
        }

        private ILogFilters GetDefaultFilters()
        {
            var builder = new LogFiltersBuilder();
            return builder.Build();
        }
    }
}