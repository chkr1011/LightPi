using System;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Mappings
{
    public class MappingViewModel : BaseViewModel
    {
        private bool _state;

        public MappingViewModel(Mapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Mapping = mapping;
        }

        public Mapping Mapping { get; }

        public bool State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }
    }
}
