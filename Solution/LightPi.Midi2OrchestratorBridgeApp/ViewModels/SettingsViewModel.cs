using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IMidiService _midiService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogService _logService;

        public SettingsViewModel(ISettingsService settingsService, IMidiService midiService, IOrchestratorService orchestratorService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _midiService = midiService;
            _orchestratorService = orchestratorService;
            _logService = logService;
        }

        public bool UseOrchestrator { get; set; }

        public bool UseEmulator { get; set; }

        public string OrchestratorAddress { get; set; }

        public List<MidiPortViewModel> AvailableMidiPorts { get; } = new List<MidiPortViewModel>();

        public void LoadSettings()
        {
            if (_settingsService.Settings.Target == Target.Orchestrator)
            {
                UseOrchestrator = true;
            }
            else
            {
                UseEmulator = true;
            }

            OrchestratorAddress = _settingsService.Settings.OrchestratorAddress?.ToString();

            foreach (var midiPort in _midiService.GetMidiPorts())
            {
                bool isSelected = midiPort.Name.Equals(_settingsService.Settings.MidiIn);

                var midiPortViewModel = new MidiPortViewModel(midiPort, isSelected);
                AvailableMidiPorts.Add(midiPortViewModel);
            }

            if (!AvailableMidiPorts.Any(p => p.IsSelected) && AvailableMidiPorts.Any())
            {
                AvailableMidiPorts.First().IsSelected = true;
            }

            _logService.Information($"Found {AvailableMidiPorts.Count} MIDI input ports");
        }
        
        public void Close(DialogResult dialogResult)
        {
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            try
            {
                _settingsService.Settings.Target = UseOrchestrator ? Target.Orchestrator : Target.Emulator;

                IPAddress ipAddress;
                IPAddress.TryParse(OrchestratorAddress, out ipAddress);
                _settingsService.Settings.OrchestratorAddress = ipAddress;

                _settingsService.Settings.MidiIn = AvailableMidiPorts.First(m => m.IsSelected).MidiPort.Name;

                _settingsService.Save();

                _logService.Information("Successfully saved settings");
            }
            catch (Exception exception)
            {
                _logService.Error("Failed to save settings: " + exception);
            }
        }
    }
}
