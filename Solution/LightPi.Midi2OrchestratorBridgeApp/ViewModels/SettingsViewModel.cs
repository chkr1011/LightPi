using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly MidiService _midiService;
        private readonly OrchestratorService _orchestratorService;
        private readonly LogService _logService;

        private bool _showLog;

        public SettingsViewModel(SettingsService settingsService, MidiService midiService, OrchestratorService orchestratorService, LogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _midiService = midiService;
            _orchestratorService = orchestratorService;
            _logService = logService;

            RegisterCommand(SettingsCommand.Save, SaveSettings);
            LoadSettings();
        }

        public bool UseOrchestrator { get; set; }

        public bool UseEmulator { get; set; }

        public string OrchestratorAddress { get; set; }

        public bool ShowLog
        {
            get { return _showLog; }
            set
            {
                _showLog = value;
                OnPropertyChanged();
            }
        }

        public List<MidiPortViewModel> AvailableMidiPorts { get; } = new List<MidiPortViewModel>();

        private void LoadSettings()
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

            if (!AvailableMidiPorts.Any(p => p.IsSelected))
            {
                AvailableMidiPorts.First().IsSelected = true;
            }

            _logService.Information($"Found {AvailableMidiPorts.Count} MIDI input ports");
        }

        private void SaveSettings()
        {
            _settingsService.Settings.Target = UseOrchestrator ? Target.Orchestrator : Target.Emulator;

            _settingsService.Settings.OrchestratorAddress = IPAddress.Parse(OrchestratorAddress);
            _settingsService.Settings.MidiIn = AvailableMidiPorts.First(m => m.IsSelected).MidiPort.Name;

            _settingsService.Save();

            _logService.Information("Successfully saved settings");

            _midiService.AttachMidiPort(AvailableMidiPorts.First(p => p.IsSelected).MidiPort);
            _orchestratorService.AttachOrchestrator();
        }
    }
}
