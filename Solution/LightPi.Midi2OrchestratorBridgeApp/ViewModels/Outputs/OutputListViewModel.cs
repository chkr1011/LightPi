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

        public List<OutputGroup> OutputGroups { get; } = new List<OutputGroup>();

        public void LoadOutputs()
        {
            for (var i = 0; i < 51; i++)
            {
                string name;
                if (!_settingsService.Settings.Outputs.TryGetValue(i, out name))
                {
                    name = "Output-" + i;
                }

                Outputs.Add(new Output { Id = i, Name = name });
            }
        }

        public void Close(DialogResult dialogResult)
        {
            foreach (var output in Outputs)
            {
                _settingsService.Settings.Outputs[output.Id] = output.Name;
            }

            _settingsService.Save();
        }
    }
}
