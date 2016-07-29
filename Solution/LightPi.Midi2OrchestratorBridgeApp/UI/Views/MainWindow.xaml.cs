using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Views
{
    public partial class MainWindow 
    {
        public MainWindow()
        {
            var logService = new LogService();

            var settingsService = new SettingsService();
            settingsService.Load();

            var dialogService = new DialogService(this);
            var midiService = new MidiService(logService);
            var orchestratorService = new OrchestratorService(settingsService);

            DataContext = new MainWindowViewModel(settingsService, midiService, orchestratorService, logService, dialogService);

            InitializeComponent();
        }
    }
}
