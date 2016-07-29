using System;
using LightPi.Orchestrator;

namespace LightPi.Midi2OrchestratorBridgeApp.Services
{
    public class OrchestratorService
    {
        private readonly SettingsService _settingsService;

        private OrchestratorClient _client;

        public OrchestratorService(SettingsService settingsService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));

            _settingsService = settingsService;
        }

        public void AttachOrchestrator()
        {
            _client = new OrchestratorClient(_settingsService.Settings.OrchestratorAddress);
        }

        public void SetOutputState(int id, bool state)
        {
            _client?.SetOutput(id, state);
        }

        public void SendState()
        {
            _client?.SendState();
        }
    }
}
