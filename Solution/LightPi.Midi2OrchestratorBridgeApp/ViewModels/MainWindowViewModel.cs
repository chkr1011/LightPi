using System;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(SettingsViewModel settingsViewModel, MappingsViewModel mappingsViewModel, LogViewModel logViewModel)
        {
            if (settingsViewModel == null) throw new ArgumentNullException(nameof(settingsViewModel));
            if (mappingsViewModel == null) throw new ArgumentNullException(nameof(mappingsViewModel));
            if (logViewModel == null) throw new ArgumentNullException(nameof(logViewModel));

            Settings = settingsViewModel;
            Mappings = mappingsViewModel;
            Log = logViewModel;
        }

        public SettingsViewModel Settings { get; }

        public MappingsViewModel Mappings { get; }

        public LogViewModel Log { get; }
    }
}
