using System;
using LightPi.Midi2OrchestratorBridge.ViewModels;

namespace LightPi.Midi2OrchestratorBridge.Models
{
    public class OutputViewModel : BaseViewModel
    {
        private bool _isActive;

        public OutputViewModel(Output output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            Output = output;

            RouteCommand(OutputCommand.ToggleState, ToggleState);
        }

        public event EventHandler StateChanged;

        public Output Output { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }

        private void ToggleState()
        {
            IsActive = !IsActive;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
