using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Yalf.Reporting;

namespace YalfPerver.Utilities
{
    public class LogFilterFileHandler
    {

        public static ILogFilters Load(String settingsFilePath)
        {
            if (String.IsNullOrEmpty(settingsFilePath))
                throw new ArgumentNullException("A valid path must be entered to load the file from");
            if (!File.Exists(settingsFilePath))
                throw new FileNotFoundException("Filter file does not exist.", settingsFilePath);
            var serial = new XmlSerializer(typeof(LogFiltersBuilder));
            using (var sr = new StreamReader(settingsFilePath))
            {
                return (serial.Deserialize(sr) as LogFiltersBuilder).Build();
            }
        }

        public static void Save(string settingsFilePath, ILogFilters filters)
        {
            var serial = new XmlSerializer(typeof(LogFiltersBuilder));
            var settings = new XmlWriterSettings() { Indent = true };
            using (var writer = XmlWriter.Create(settingsFilePath, settings))
            {
                serial.Serialize(writer, LogFiltersBuilder.Create(filters));
            }
        }

    }
}