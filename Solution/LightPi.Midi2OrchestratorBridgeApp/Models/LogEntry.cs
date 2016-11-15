using System;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    public class LogEntry
    {
        public LogEntry(LogEntrySeverity severity, DateTime timestamp, string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Severity = severity;
            Timestamp = timestamp;
            Message = message;
        }

        public LogEntrySeverity Severity { get; }

        public DateTime Timestamp { get; }

        public string Message { get; }
    }
}
