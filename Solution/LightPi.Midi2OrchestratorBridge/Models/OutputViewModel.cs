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

            RouteCommand(OutputCommand.ToggleState, ()=> IsActive = !IsActive);
        }

        public event EventHandler StateChanged;

        public Output Output { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                var hasChanged = _isActive != value;
                if (!hasChanged)
                {
                    return;
                }

                _isActive = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
