using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace LightPi.OrchestratorEmulator
{
    public sealed class Sprite : INotifyPropertyChanged
    {
        private bool _isVisible;

        public Sprite(int outputId, BitmapImage image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            OutputId = outputId;
            Image = image;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int OutputId { get; }

        public BitmapImage Image { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
