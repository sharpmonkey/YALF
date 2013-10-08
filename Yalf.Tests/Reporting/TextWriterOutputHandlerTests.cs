using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Yalf.LogEntries;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;
using Yalf.Reporting.OutputHandlers;

namespace Yalf.Tests.Reporting
{
    public class TextWriterOutputHandlerTests
    {
        [Test]
        public void DelimitedValuesFormatter_NestedMethodCalls_ReportHasCorrectFormatting()
        {
            // Arrange
            var formatter = new DelimitedValuesFormatter();
            var startDateTime = DateTime.Now;
            var entries = new BaseEntry[]
                {
                    new ThreadData(22, null, null), 
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
                                        string.Format("Yalf,Method,TopLevelMethod,blackSheep,{0:HH:mm:ss.fff},233,0,22", startDateTime),
                                        string.Format("Yalf,Method,FirstMethod,,{0:HH:mm:ss.fff},200,0,22", startDateTime.AddSeconds(12)),
                                        string.Format("Yalf,Method,SecondMethod,,{0:HH:mm:ss.fff},178,1,22", startDateTime.AddSeconds(45)),
                                        string.Format("Yalf,Log,Information log message here,,{0:HH:mm:ss.fff},0,2,22", startDateTime.AddSeconds(47)),
                                        string.Format("Yalf,Exception,Test the log Parameter name: lineNo,,{0:HH:mm:ss.fff},0,2,22", startDateTime.AddSeconds(53)),
                                        string.Format("Yalf,Method,ThirdMethod,,{0:HH:mm:ss.fff},100,2,22", startDateTime.AddSeconds(75)),
                                        string.Format("Yalf,Method,FourthMethod,,{0:HH:mm:ss.fff},57,2,22", startDateTime.AddSeconds(80)),
                                        string.Format("Yalf,Method,TopLevelMethod2,whiteSheep,{0:HH:mm:ss.fff},488,0,22", startDateTime.AddSeconds(99))
                                    }
                   ).ToList();

            var filters = this.GetDefaultFilters();
            var indentLevel = 0;
            var writer = new StringWriter();

            // Act
            var outputter = new TextWriterOutputHandler(writer, filters, formatter);
            outputter.Initialise();

            foreach (var entry in entries)
            {
                if (entry is ThreadData)
                {
                    outputter.HandleThread((ThreadData)entry);
                }
                else if (entry is MethodEntry)
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
            var reportText = writer.ToString();

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

        private ILogFilters GetDefaultFilters()
        {
            var builder = new LogFiltersBuilder();
            return builder.Build();
        }

    }
}
