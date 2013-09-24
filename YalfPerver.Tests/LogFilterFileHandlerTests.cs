using System;
using NUnit.Framework;
using Yalf.Reporting;
using YalfPerver.Utilities;

namespace YalfPerver.Tests
{
    public class LogFilterFileHandlerTests
    {

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Load_NullPathSupplied_ExceptionThrown()
        {
            LogFilterFileHandler.Load(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Load_BlankPathSupplied_ExceptionThrown()
        {
            LogFilterFileHandler.Load(String.Empty);
        }

        [Test, ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void Load_FileDoesNotExist_ExceptionThrown()
        {
            LogFilterFileHandler.Load(@"C:\invalidpath\really_it_is_invalid\blah$$.glhj");
        }

        [Test]
        public void Load_ValidFile_CorrectEntriesAreLoaded()
        {
            // Arrange
            var builder = LogFiltersBuilder.Create().WithEnterMethodLogsHidden().WithMethodDurationHidden().WithSingleLineFormat();
            builder.ExcludeTheseMethodNames(new string[] { @"\.ctor" });
            builder.IncludeTheseMethodNames(new string[] { "Insuranceline.Ethil" });
            
            var fileName = System.IO.Path.GetTempFileName();
            var expectedFilters = builder.Build();
            LogFilterFileHandler.Save(fileName, expectedFilters);

            // Act
            var loadedFilters = LogFilterFileHandler.Load(fileName);
            System.IO.File.Delete(fileName);
            
            // Assert
            Assert.That(loadedFilters, Is.Not.Null);
            Assert.That(loadedFilters.HideEnterMethodLogs, Is.EqualTo(expectedFilters.HideEnterMethodLogs));
            Assert.That(loadedFilters.HideExitMethodLogs, Is.EqualTo(expectedFilters.HideExitMethodLogs));
            Assert.That(loadedFilters.HideMethodDuration, Is.EqualTo(expectedFilters.HideMethodDuration));
            Assert.That(loadedFilters.HideMethodParameters, Is.EqualTo(expectedFilters.HideMethodParameters));
            Assert.That(loadedFilters.HideMethodReturnValue, Is.EqualTo(expectedFilters.HideMethodReturnValue));
            Assert.That(loadedFilters.HideTimeStampInMethod, Is.EqualTo(expectedFilters.HideTimeStampInMethod));
            Assert.That(loadedFilters.IgnoreCaseInFilter, Is.EqualTo(expectedFilters.IgnoreCaseInFilter));
            Assert.That(loadedFilters.SingleLineFormat, Is.EqualTo(expectedFilters.SingleLineFormat));
            Assert.That(loadedFilters.TimeStampFrom, Is.EqualTo(expectedFilters.TimeStampFrom));
            Assert.That(loadedFilters.TimeStampTo, Is.EqualTo(expectedFilters.TimeStampTo));
            Assert.That(loadedFilters.ExcludedKeysExpressionList, Is.EquivalentTo(expectedFilters.ExcludedKeysExpressionList));
            Assert.That(loadedFilters.IncludedKeysExpressionList, Is.EquivalentTo(expectedFilters.IncludedKeysExpressionList));

        }
    }
}
