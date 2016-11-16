using System;
using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.Services;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public class OutputListViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly ISettingsService _settingsService;

        public OutputListViewModel(ISettingsService settingsService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));

            _settingsService = settingsService;
        }

        public List<OutputViewModel> Outputs { get; } = new List<OutputViewModel>();

        public void LoadOutputs()
        {
            Outputs.AddRange(_settingsService.OutputViewModels);
        }

        public void Close(DialogResult dialogResult)
        {
        }
    }
}
