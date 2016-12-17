using System;
using System.Reflection;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IFactoryService _factoryService;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly IMidiService _midiService;
        private readonly IOrchestratorService _orchestratorService;
        private readonly ILogService _logService;
        private bool _hasDialog;

        public MainWindowViewModel(
            IFactoryService factoryService, 
            IDialogService dialogService,
            ISettingsService settingsService,
            IMidiService midiService,
            IOrchestratorService orchestratorService,
            ILogService logService,
            EmulatorViewModel emulatorViewModel,
            MappingsListViewModel mappingsViewModel, 
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
            Emulator = emulatorViewModel;
            Log = logViewModel;

            RouteCommand(ToolBarCommand.Settings, ChangeSettings);
            RouteCommand(ToolBarCommand.Outputs, ChangeOutputs);

            Initialize();

            _dialogService.DialogShown += (s, e) => HasDialog = true;
            _dialogService.DialogClosed += (s, e) => HasDialog = false;

            var version = Assembly.GetEntryAssembly().GetName().Version;
            Title = $"MIDI 2 Orchestrator Bridge v{version} - LightPi";
        }

        public string Title { get; private set; }

        public EmulatorViewModel Emulator { get; }

        public MappingsListViewModel Mappings { get; }

        public LogViewModel Log { get; }

        public bool HasDialog
        {
            get { return _hasDialog; }
            set { _hasDialog = value; OnPropertyChanged(); }
        }

        private void ChangeSettings()
        {
            var settingsViewModel =_factoryService.GetInstance<SettingsViewModel>();
            settingsViewModel.LoadSettings();

            var dialogResult = _dialogService.ShowDialog("Settings", settingsViewModel);

            if (dialogResult == DialogResult.OK)
            {
                Initialize();
            }
        }

        private void ChangeOutputs()
        {
            var outputsViewModel = _factoryService.GetInstance<OutputListViewModel>();
            outputsViewModel.LoadOutputs();

            var dialogResult = _dialogService.ShowDialog("Outputs", outputsViewModel);
            if (dialogResult == DialogResult.Cancel)
            {
                return;
            }

            foreach (var output in outputsViewModel.Outputs)
            {
                _settingsService.Settings.Outputs[output.Output.Id] = output.Output;
            }

            _settingsService.SaveSettings();
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
