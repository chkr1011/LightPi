using System;
using LightPi.Midi2OrchestratorBridgeApp.Services;
using LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IFactoryService _factoryService;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly IMidiService _midiService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogService _logService;

        public MainWindowViewModel(
            IFactoryService factoryService, 
            IDialogService dialogService,
            ISettingsService settingsService,
            IMidiService midiService,
            IOrchestratorService orchestratorService,
            ILogService logService,
            MappingsViewModel mappingsViewModel, 
            LogViewModel logViewModel)
        {
            if (factoryService == null) throw new ArgumentNullException(nameof(factoryService));
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));
            if (midiService == null) throw new ArgumentNullException(nameof(midiService));
            if (orchestratorService == null) throw new ArgumentNullException(nameof(orchestratorService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));
            if (mappingsViewModel == null) throw new ArgumentNullException(nameof(mappingsViewModel));
            if (logViewModel == null) throw new ArgumentNullException(nameof(logViewModel));

            _factoryService = factoryService;
            _dialogService = dialogService;
            _settingsService = settingsService;
            _midiService = midiService;
            _orchestratorService = orchestratorService;
            _logService = logService;

            Mappings = mappingsViewModel;
            Log = logViewModel;

            RouteCommand(ToolBarCommand.Settings, ChangeSettings);
        }

        public MappingsViewModel Mappings { get; }

        public LogViewModel Log { get; }

        private void ChangeSettings()
        {
            var settingsViewModel =_factoryService.GetInstance<SettingsViewModel>();
            var dialogResult = _dialogService.ShowDialog("Settings", settingsViewModel);

            if (dialogResult == DialogResult.OK)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            try
            {
                _midiService.Initialize();
                _orchestratorService.Initialize();
            }
            catch (Exception exception)
            {
                _logService.Error("Failed to initialize: " + exception);
            }
        }
    }
}
