namespace Yalf.Reporting.Formatters
{
    public class OrderedOutput
    {
        public readonly int Level;
        public readonly string FormattedLine;

        public OrderedOutput(int level, string formattedLine)
        {
            this.Level = level;
            this.FormattedLine = formattedLine;
        }
    }
}