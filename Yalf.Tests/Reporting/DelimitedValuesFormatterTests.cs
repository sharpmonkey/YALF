using System;
using System.Collections.Generic;
using System.Linq;
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
            var orderedOutput = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(orderedOutput.Count, Is.Not.EqualTo(0), "Expected one string to be returned");
            Assert.That(orderedOutput[0].FormattedLine, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            var orderedOutput = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);

            // Assert
            Assert.That(orderedOutput.Count, Is.Not.EqualTo(0), "Expected one string to be returned");
            Assert.That(orderedOutput[0].FormattedLine, Is.EqualTo(expectedText), "Not the expected output text, you may need to adjust the test if the formatter has been changed.");
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
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void FormatMethodExit_SuppliedEntryMethodNameDoesNotMatch_ExceptionIsThrown()
        {
            // Arrange
            var filters = this.GetDefaultFilters();
            var formatter = new DelimitedValuesFormatter();
            var entry = new MethodExit(1, "Yalf.TestMethod", 345, true, "returnVal");

            // Act, Assert
            var outputText = formatter.FormatMethodExitDelayed(1, 2, 33, entry, filters);
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

        [Test]
        public void FormatMethodExit_MixedNestting_ExpectedTextIsReturned()
        {
            // Arrange
            var filters = LogFiltersBuilder.Create().Build();
            var formatter = new DelimitedValuesFormatter();
            var startDateTime = DateTime.Now;
            int threadId = 22;
            int lineNo = 0;
            int indentLevel = 0;

            var expectedText = (new string[]
                                    {
                                        string.Format("Yalf,Method,TopLevelMethod,blackSheep,{0:HH:mm:ss.fff},233,1,22", startDateTime),
                                        string.Format("Yalf,Method,FirstMethod,,{0:HH:mm:ss.fff},200,1,22", startDateTime.AddSeconds(12)),
                                        string.Format("Yalf,Method,SecondMethod,,{0:HH:mm:ss.fff},178,2,22", startDateTime.AddSeconds(45)),
                                        string.Format("Yalf,Log,Information log message here,,{0:HH:mm:ss.fff},0,2,22", startDateTime.AddSeconds(47)),
                                        string.Format("Yalf,Exception,Test the log Parameter name: lineNo,,{0:HH:mm:ss.fff},0,2,22", startDateTime.AddSeconds(53)),
                                        string.Format("Yalf,Method,ThirdMethod,,{0:HH:mm:ss.fff},100,3,22", startDateTime.AddSeconds(75)),
                                        string.Format("Yalf,Method,TopLevelMethod2,whiteSheep,{0:HH:mm:ss.fff},488,1,22", startDateTime.AddSeconds(99))
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
                        actualText.AddRange(result.Select(oo => oo.FormattedLine).ToList());
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
            Console.WriteLine(String.Join(Environment.NewLine, actualText.ToArray()));

            Assert.That(actualText.Count, Is.EqualTo(expectedText.Count), "Expected {0} lines to be returned overall, but have {1}.", expectedText.Count, actualText.Count);
            for (int logLine = 0; logLine < actualText.Count-1; logLine++)
            {
                Assert.That(actualText[logLine], Is.EqualTo(expectedText[logLine]), "Text does not match on line {0}", logLine);
            }
            
            //var misMatchedResults = actualText.Where((at, i) => (at != expectedText[i]));
            //Assert.That(misMatchedResults.Count(), Is.EqualTo(0), "{0} of the lines do not match.\nExpected:\n{1}\n\nActual:\n{2}"
            //                                                    , misMatchedResults.Count()
            //                                                    , String.Join(Environment.NewLine, expectedText.ToArray())
            //                                                    , String.Join(Environment.NewLine, actualText.ToArray())
            //            );
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