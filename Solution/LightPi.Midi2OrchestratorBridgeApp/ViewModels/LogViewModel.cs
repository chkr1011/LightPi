using System;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        public LogViewModel(LogService logService)
        {
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            logService.Logged += (sender, args) => Logged?.Invoke(this, args);
        }

        public event EventHandler<LoggedEventArgs> Logged;
    }
}
