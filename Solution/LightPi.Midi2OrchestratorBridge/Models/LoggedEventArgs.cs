using System;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    public class LoggedEventArgs : EventArgs
    {
        public LoggedEventArgs(LogEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            Entry = entry;
        }

        public LogEntry Entry { get; }
    }
}
