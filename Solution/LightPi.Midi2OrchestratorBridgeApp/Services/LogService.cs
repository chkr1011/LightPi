using System;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class LogService
    {
        public event EventHandler<LoggedEventArgs> Logged;

        public void Verbose(string message)
        {
            Log(LogEntrySeverity.Verbose, message);
        }

        public void Information(string message)
        {
            Log(LogEntrySeverity.Information, message);
        }

        public void Warning(string message)
        {
            Log(LogEntrySeverity.Warning, message);
        }

        public void Error(string message)
        {
            Log(LogEntrySeverity.Error, message);
        }

        private void Log(LogEntrySeverity severity, string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var entry = new LogEntry(severity, DateTime.Now, message);
            Logged?.Invoke(this, new LoggedEventArgs(entry));
        }
    }
}
