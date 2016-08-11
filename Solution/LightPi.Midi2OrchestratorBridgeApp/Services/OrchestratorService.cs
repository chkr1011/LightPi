using System;
using System.Net;
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
        
        public void AttachOrchestrator(IPAddress ipAddress)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            _client = new OrchestratorClient(ipAddress);
        }

        public void SetOutputState(int id, bool state)
        {
            _client?.SetOutput(id, state);

            string stateText = state ? "On" : "Off";
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
                return;
            }

            _logService.Information($"Sent {BitConverter.ToString(result.State)} to orchestrator");
        }
    }
}
