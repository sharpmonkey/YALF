using System;

namespace Yalf.Reporting.Formatters
{
    public interface IIndentableSingleLineMethodFormatter
    {
        bool IsLogEventLine(String formattedLine);
        bool IsExceptionTraceLine(String formattedLine);
        bool IndentIncreaseRequired(string formattedLine);
    }
}
