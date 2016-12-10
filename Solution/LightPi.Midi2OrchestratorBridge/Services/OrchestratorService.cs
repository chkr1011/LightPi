using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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

            ////Task.Factory.StartNew(CommitStateLoop);
        }

        ////private void CommitStateLoop()
        ////{
        ////    while (true)
        ////    {
        ////        //CommitChanges();
        ////        Thread.Sleep(50); // TODO: As Settings.
        ////    }
        ////}

        public void Initialize()
        {
            IPAddress orchestratorIpAddress;
            if(IPAddress.TryParse(_settingsService.Settings.OrchestratorAddress, out orchestratorIpAddress))
            {
                orchestratorIpAddress = Dns.GetHostAddresses(_settingsService.Settings.OrchestratorAddress).First();
                _logService.Information($"Resolved orchestrator address '{_settingsService.Settings.OrchestratorAddress}' to '{orchestratorIpAddress}'");
            }

            _client = new OrchestratorClient(orchestratorIpAddress)
            {
                ForceCommits = Properties.Settings.Default.ForceCommits
            };
        }

        public void Reset()
        {
            var result = _client.Reset();
            ChangesCommitted?.Invoke(this, new ChangesCommittedEventArgs(result.State));
        }

        public event EventHandler<ChangesCommittedEventArgs> ChangesCommitted;

        public void SetOutputState(int id, bool isActive)
        {
            _client?.SetOutput(id, isActive, SetOutputMode.Set);

            _logService.Information("Set " + id + " = " + (isActive ? "on" : "off"));
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

            _logService.Information("Sent " + BitConverter.ToString(result.State));
      
            ChangesCommitted?.Invoke(this, new ChangesCommittedEventArgs(result.State));
        }
    }
}
