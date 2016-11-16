using System;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.Services
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