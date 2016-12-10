using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingViewModel : BaseViewModel
    {
        private bool _isActive;

        public int Channel { get; set; }

        public string Note { get; set; }

        public int Octave { get; set; }

        public string Comment { get; set; }

        public List<OutputViewModel> Outputs { get; private set; } = new List<OutputViewModel>();

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }
    }
}
