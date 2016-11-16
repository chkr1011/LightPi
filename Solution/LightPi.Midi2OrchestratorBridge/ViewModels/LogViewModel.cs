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

            logService.Logged += ForwardLogEntry;
        }

        public bool IsEnabled { get; set; }

        public event EventHandler<LoggedEventArgs> Logged;

        private void ForwardLogEntry(object sender, LoggedEventArgs loggedEventArgs)
        {
            if (!IsEnabled)
            {
                return;
            }

            Logged?.Invoke(this, loggedEventArgs);
        }
    }
}
