using System;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
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
