using System;
using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridgeApp.Models;
using LightPi.Midi2OrchestratorBridgeApp.Services;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Outputs
{
    public class OutputListViewModel : BaseViewModel, IDialogViewModel
    {
        private readonly ISettingsService _settingsService;

        public OutputListViewModel(ISettingsService settingsService, ILogService logService)
        {
            if (settingsService == null) throw new ArgumentNullException(nameof(settingsService));

            _settingsService = settingsService;
        }

        public List<Output> Outputs { get; } = new List<Output>();

        public List<OutputGroup> OutputGroup { get; } = new List<OutputGroup>();

        public void LoadOutputs()
        {
            for (int i = 0; i < 51; i++)
            { 
                Outputs.Add(new Output {Id = i, Name = "Output-" + i});
            }
                

            //Outputs.AddRange(_settingsService.Settings.Outputs);
        }

        public void Close(DialogResult dialogResult)
        {
        }
    }
}
