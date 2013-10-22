using System;
using System.Collections.Generic;
using System.Linq;
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
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleMethodEntry_LogNotEnabled_NoTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new MethodEntry(1, "Yalf.TestMethod", new[] { "param1", "param2" }, DateTime.Parse("2022-10-22 22:22:31.678"));
            output.Initialise();

            // Act
            output.HandleMethodEntry(entry, 1, false);
            output.Complete();

            // Assert
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Empty, "Expected no text in the report output");
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
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleMethodExit_LogNotEnabled_NoTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");
            output.Initialise();

            // Act
            output.HandleMethodExit(entry, 1, false);
            output.Complete();

            // Assert
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Empty, "Expected no text in the report output");
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
            var outputText = output.GetReport();
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
            var outputText = output.GetReport();
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
            var outputText = output.GetReport();
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
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void HandleLogEntry_LogNotEnabled_NoTextIsReturned()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var output = new DefaultOutputHandler(filters);
            var entry = new LogEvent(LogLevel.Info, DateTime.Parse("2022-10-22 22:22:31.678"), "This is a log entry");

            output.Initialise();

            // Act
            output.HandleLogEvent(entry, 1, false);
            output.Complete();

            // Assert
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Empty, "Expected no text in the report output");
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
            var outputText = output.GetReport();
            Assert.That(outputText, Is.Not.Empty, "Expected a string to be returned");
            Assert.That(outputText, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
        }

        [Test]
        public void SingleLineFormatter_NestedMethodCalls_ReportHasCorrectIndenting()
        {
            // Arrange
            var formatter = new SingleLineFormatter();
            var startDateTime = DateTime.Now;
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
                    new MethodEntry(3, "FourthMethod", null, startDateTime.AddSeconds(80)),
                    new MethodExit(3, "FourthMethod", 57, false, null),
                    new MethodExit(2, "SecondMethod", 178, false, null),
                    new MethodExit(1, "FirstMethod", 200, false, null),
                    new MethodEntry(1, "TopLevelMethod2", null, startDateTime.AddSeconds(99)),
                    new MethodExit(1, "TopLevelMethod2", 488, true, "whiteSheep"),
                };

            var expectedText = (new string[]
                                    {
                                        string.Format("TopLevelMethod(blackSheep) started {0:HH:mm:ss.fff} duration 233ms", startDateTime),
                                        string.Format("FirstMethod() started {0:HH:mm:ss.fff} duration 200ms", startDateTime.AddSeconds(12)),
                                        string.Format("  SecondMethod() started {0:HH:mm:ss.fff} duration 178ms", startDateTime.AddSeconds(45)),
                                        string.Format("    [Log] [Info] Information log message here"),
                                        string.Format("    [Exception] {0:HH:mm:ss.fff} Test the log", startDateTime.AddSeconds(53)),
                                        string.Format("Parameter name: lineNo"),
                                        string.Format("    ThirdMethod() started {0:HH:mm:ss.fff} duration 100ms", startDateTime.AddSeconds(75)),
                                        string.Format("    FourthMethod() started {0:HH:mm:ss.fff} duration 57ms", startDateTime.AddSeconds(80)),
                                        string.Format("TopLevelMethod2(whiteSheep) started {0:HH:mm:ss.fff} duration 488ms", startDateTime.AddSeconds(99))
                                    }
                               ).ToList();

            var filters = this.GetDefaultFilters();
            var indentLevel = 0;

            // Act
            var outputter = new DefaultOutputHandler(filters, formatter);
            outputter.Initialise();

            foreach (var entry in entries)
            {
                if (entry is MethodEntry)
                {
                    outputter.HandleMethodEntry((entry as MethodEntry), indentLevel, true);
                    ++indentLevel;
                }
                else if (entry is MethodExit)
                {
                    --indentLevel;
                    outputter.HandleMethodExit((entry as MethodExit), indentLevel, true);
                }
                else if (entry is LogEvent)
                {
                    outputter.HandleLogEvent((entry as LogEvent), indentLevel, true);
                }
                else if (entry is ExceptionTrace)
                {
                    outputter.HandleException((entry as ExceptionTrace), indentLevel);
                }
            }

            outputter.Complete();
            var reportText = outputter.GetReport();

            // Assert
            Console.WriteLine(reportText);

            List<String> output = reportText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Assert.That(output.Count, Is.EqualTo(expectedText.Count), "Expected {0} output lines, but have {1}", expectedText.Count, output.Count);
            Assert.That(reportText, Is.Not.Empty, "Expected report text to be returned.");
            for (int index = 0; index < expectedText.Count; index++)
            {
                Console.WriteLine("Checking actual\r\n\"{0}\" with expected\r\n\"{1}\"", output[index], expectedText[index]);
                Assert.That(output[index], Is.EqualTo(expectedText[index]), "Not the expected text for line {0}", index + 1);
            }
        }

        [Test]
        public void SingleLineFormatter_NestedMethodCallsWithSomeMethodsDisabled_ReportDoesNotHaveDisabledLogs()
        {
            // Arrange
            var formatter = new SingleLineFormatter();
            var startDateTime = DateTime.Now;
            var entries = new Tuple<bool, BaseEntry>[]
                {
                    new Tuple<bool, BaseEntry>(true, new MethodEntry(1, "TopLevelMethod", null, startDateTime)),
                    new Tuple<bool, BaseEntry>(true, new MethodExit(1, "TopLevelMethod", 233, true, "blackSheep")),
                    new Tuple<bool, BaseEntry>(true, new MethodEntry(1, "FirstMethod", null, startDateTime.AddSeconds(12))),
                    new Tuple<bool, BaseEntry>(false, new MethodEntry(2, "SecondMethod", null, startDateTime.AddSeconds(45))),
                    new Tuple<bool, BaseEntry>(true, new LogEvent(LogLevel.Info, startDateTime.AddSeconds(47), "Information log message here")), 
                    new Tuple<bool, BaseEntry>(true, new ExceptionTrace(new ArgumentNullException("lineNo", "Test the log"), startDateTime.AddSeconds(53))), 
                    new Tuple<bool, BaseEntry>(true, new MethodEntry(3, "ThirdMethod", null, startDateTime.AddSeconds(75))),
                    new Tuple<bool, BaseEntry>(true, new MethodExit(3, "ThirdMethod", 100, false, null)),
                    new Tuple<bool, BaseEntry>(false, new MethodEntry(3, "FourthMethod", null, startDateTime.AddSeconds(80))),
                    new Tuple<bool, BaseEntry>(false, new MethodExit(3, "FourthMethod", 57, false, null)),
                    new Tuple<bool, BaseEntry>(false, new MethodExit(2, "SecondMethod", 178, false, null)),
                    new Tuple<bool, BaseEntry>(true, new MethodExit(1, "FirstMethod", 200, false, null)),
                    new Tuple<bool, BaseEntry>(true, new MethodEntry(1, "TopLevelMethod2", null, startDateTime.AddSeconds(99))),
                    new Tuple<bool, BaseEntry>(true, new MethodExit(1, "TopLevelMethod2", 488, true, "whiteSheep")),
                };

            var expectedText = (new string[]
                                    {
                                        string.Format("TopLevelMethod(blackSheep) started {0:HH:mm:ss.fff} duration 233ms", startDateTime),
                                        string.Format("FirstMethod() started {0:HH:mm:ss.fff} duration 200ms", startDateTime.AddSeconds(12)),
                                        string.Format("    [Log] [Info] Information log message here"),
                                        string.Format("    [Exception] {0:HH:mm:ss.fff} Test the log", startDateTime.AddSeconds(53)),
                                        string.Format("Parameter name: lineNo"),
                                        string.Format("    ThirdMethod() started {0:HH:mm:ss.fff} duration 100ms", startDateTime.AddSeconds(75)),
                                        string.Format("TopLevelMethod2(whiteSheep) started {0:HH:mm:ss.fff} duration 488ms", startDateTime.AddSeconds(99))
                                    }
                               ).ToList();

            var filters = this.GetDefaultFilters();
            var indentLevel = 0;

            // Act
            var outputter = new DefaultOutputHandler(filters, formatter);
            outputter.Initialise();

            foreach (var entry in entries)
            {
                if (entry.Item2 is MethodEntry)
                {
                    outputter.HandleMethodEntry((entry.Item2 as MethodEntry), indentLevel, entry.Item1);
                    ++indentLevel;
                }
                else if (entry.Item2 is MethodExit)
                {
                    --indentLevel;
                    outputter.HandleMethodExit((entry.Item2 as MethodExit), indentLevel, entry.Item1);
                }
                else if (entry.Item2 is LogEvent)
                {
                    outputter.HandleLogEvent((entry.Item2 as LogEvent), indentLevel, entry.Item1);
                }
                else if (entry.Item2 is ExceptionTrace)
                {
                    outputter.HandleException((entry.Item2 as ExceptionTrace), indentLevel);
                }
            }

            outputter.Complete();
            var reportText = outputter.GetReport();

            // Assert
            Console.WriteLine(reportText);

            List<String> output = reportText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Assert.That(output.Count, Is.EqualTo(expectedText.Count), "Expected {0} output lines, but have {1}", expectedText.Count, output.Count);
            Assert.That(reportText, Is.Not.Empty, "Expected report text to be returned.");
            for (int index = 0; index < expectedText.Count; index++)
            {
                Console.WriteLine("Checking actual\r\n\"{0}\" with expected\r\n\"{1}\"", output[index], expectedText[index]);
                Assert.That(output[index], Is.EqualTo(expectedText[index]), "Not the expected text for line {0}", index + 1);
            }
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

