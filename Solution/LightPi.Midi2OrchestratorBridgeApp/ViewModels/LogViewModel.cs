using System;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public LogViewModel(ILogService logService)
        {
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            logService.Logged += (sender, args) => Logged?.Invoke(this, args);
        }

        public event EventHandler<LoggedEventArgs> Logged;
    }
}
