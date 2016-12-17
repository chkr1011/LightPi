using System.Collections.Generic;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.ViewModels.Mappings
{
    public class MappingViewModel : BaseViewModel
    {
        private bool _isActive;
        private bool _isHidden;

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

        public bool IsHidden
        {
            get { return _isHidden; }
            set { _isHidden = value; OnPropertyChanged(); }
        }
    }
}
