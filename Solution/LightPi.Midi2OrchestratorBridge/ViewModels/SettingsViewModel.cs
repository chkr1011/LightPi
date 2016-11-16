using System;
using System.Collections.Generic;
using System.Linq;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class SettingsViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IMidiService _midiService;
        private readonly ILogService _logService;

        public SettingsViewModel(ISettingsService settingsService, IMidiService midiService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _settingsService = settingsService;
            _midiService = midiService;
            _logService = logService;
        }
        
        public string OrchestratorAddress { get; set; }

        public List<MidiPortViewModel> AvailableMidiPorts { get; } = new List<MidiPortViewModel>();

        public void LoadSettings()
        {
            OrchestratorAddress = _settingsService.Settings.OrchestratorAddress;

            foreach (var midiPort in _midiService.GetMidiPorts())
            {
                var isSelected = midiPort.Name.Equals(_settingsService.Settings.MidiIn);

                var midiPortViewModel = new MidiPortViewModel(midiPort, isSelected);
                AvailableMidiPorts.Add(midiPortViewModel);
            }

            if (!AvailableMidiPorts.Any(p => p.IsSelected) && AvailableMidiPorts.Any())
            {
                AvailableMidiPorts.First().IsSelected = true;
            }
        }
        
        public void Close(DialogResult dialogResult)
        {
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            try
            {
                _settingsService.Settings.OrchestratorAddress = OrchestratorAddress;
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
