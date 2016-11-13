using System;
using System.Net;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Orchestrator;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
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
            var ipAddress = _settingsService.Settings.OrchestratorAddress;
            if (_settingsService.Settings.Target == Target.Emulator)
            {
                ipAddress = IPAddress.Loopback.ToString();
            }

            _client = new OrchestratorClient(ipAddress);
        }

        public void SetOutputState(int id, bool state)
        {
            _client?.SetOutput(id, state);

            var stateText = state ? "On" : "Off";
            _logService.Information($"Set output {id} {stateText}");
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

            _logService.Information($"Sent {BitConverter.ToString(result.State)} to orchestrator");
        }
    }
}
