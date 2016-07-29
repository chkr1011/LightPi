using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(
            SettingsService settingsService,
            MidiService midiService,
            OrchestratorService orchestratorService,
            LogService logService,
            DialogService dialogService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            try
            {
                Settings = new SettingsViewModel(settingsService, midiService, orchestratorService, logService);
                Mappings = new MappingsViewModel(settingsService, midiService, orchestratorService, dialogService, logService);
                Log = new LogViewModel(logService);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public SettingsViewModel Settings { get; }

        public MappingsViewModel Mappings { get; }

        public LogViewModel Log { get; }
    }
}
