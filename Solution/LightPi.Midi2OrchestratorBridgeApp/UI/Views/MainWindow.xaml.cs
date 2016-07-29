using System;
using System.Windows;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Views
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            try
            {
                var logService = new LogService();

                var settingsService = new SettingsService();
                settingsService.Load();

                var midiService = new MidiService(logService);
                var orchestratorService = new OrchestratorService(settingsService, logService);
                var dialogService = new DialogService(this);

                var settingsViewModel = new SettingsViewModel(settingsService, midiService, orchestratorService, logService);
                var mappingsViewModel = new MappingsViewModel(settingsService, midiService, orchestratorService, dialogService, logService);
                var logViewModel = new LogViewModel(logService);

                var mainWindowViewModel = new MainWindowViewModel(settingsViewModel, mappingsViewModel, logViewModel);
                
                DataContext = mainWindowViewModel;

                InitializeComponent();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
