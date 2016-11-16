using System;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingViewModel : BaseViewModel
    {
        private bool _isActive;

        public MappingViewModel(Mapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            Mapping = mapping;
        }

        public Mapping Mapping { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }
    }
}
