using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LightPi.Midi2OrchestratorBridge.ViewModels
{
    public abstract class BaseViewModel : ICommand, INotifyPropertyChanged
    {
        private readonly Dictionary<object, Action> _actions = new Dictionary<object, Action>();

        public event EventHandler CanExecuteChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            Action action;
            if (_actions.TryGetValue(parameter, out action))
            {
                action();
            }
        }

        protected void RouteCommand(object parameter, Action action)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (action == null) throw new ArgumentNullException(nameof(action));

            _actions[parameter] = action;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
