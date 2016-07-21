using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LightPi.OrchestratorEmulator
{
    public sealed class Output : INotifyPropertyChanged, ICommand
    {
        private readonly SpriteSurface _surface;
        private bool _isActive;

        public Output(int id, double watts, BitmapImage sprite, SpriteSurface surface)
        {
            if (sprite == null) throw new ArgumentNullException(nameof(sprite));
            if (surface == null) throw new ArgumentNullException(nameof(surface));

            ID = id;
            Watts = watts;
            Sprite = sprite;

            _surface = surface;
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; }

        public double Watts { get; }

        public BitmapImage Sprite { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            IsActive = !IsActive;
            _surface.Update();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
