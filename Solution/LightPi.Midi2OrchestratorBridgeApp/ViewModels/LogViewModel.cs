using System;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class LogViewModel
    {
        private readonly LogService _logService;

        public LogViewModel(LogService logService)
        {
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _logService = logService;

            _logService.Logged += (sender, args) => Logged?.Invoke(this, args);
        }

        public event EventHandler<LoggedEventArgs> Logged;
    }
}
