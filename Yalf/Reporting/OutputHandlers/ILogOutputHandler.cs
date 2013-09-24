using Yalf.LogEntries;

namespace Yalf.Reporting.OutputHandlers
{
   public interface ILogOutputHandler
   {
       int CurrentThreadId { get; }
       ILogFilters Filters { get; }

       void Initialise();

       void HandleThread(ThreadData entry); 
       void HandleMethodEntry(MethodEntry entry, int indentLevel, bool displayEnabled);
       void HandleMethodExit(MethodExit entry, int indentLevel, bool displayEnabled);
       void HandleException(ExceptionTrace entry, int indentLevel);
       void HandleLogEvent(LogEvent entry, int indentLevel, bool displayEnabled);

       void Complete();
       
   }
}
