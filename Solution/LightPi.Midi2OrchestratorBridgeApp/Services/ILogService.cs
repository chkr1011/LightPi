using System;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public interface ILogService
    {
        event EventHandler<LoggedEventArgs> Logged;

        void Error(string message);
        void Information(string message);
        void Verbose(string message);
        void Warning(string message);
    }
}