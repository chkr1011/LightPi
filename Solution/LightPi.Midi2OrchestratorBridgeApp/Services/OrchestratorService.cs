using System;
using System.Net;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Orchestrator;

namespace LightPi.Midi2OrchestratorBridge.Services
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;

        private OrchestratorClient _client;

        public OrchestratorService(ISettingsService settingsService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _logService = logService;
        }
        
        public void Initialize()
        {
            _client = new OrchestratorClient(IPAddress.Parse(_settingsService.Settings.OrchestratorAddress))
            {
                ForceCommits = Properties.Settings.Default.ForceCommits
            };
        }

        public event EventHandler<ChangesCommittedEventArgs> ChangesCommitted;

        public void SetOutputState(int id, bool state)
        {
            _client?.SetOutput(id, state);

            _logService.Information("Set output " + id + " " + (state ? "on" : "off"));
        }

        public void CommitChanges()
        {
            var result = _client?.CommitChanges();
            if (result == null)
            {
                return;
            }

            if (!result.StateChanged)
            {
                _logService.Verbose("State not changed.");
                return;
            }

            _logService.Information("Sent " + BitConverter.ToString(result.State));

            ChangesCommitted?.Invoke(this, new ChangesCommittedEventArgs(result.State));
        }
    }
}
