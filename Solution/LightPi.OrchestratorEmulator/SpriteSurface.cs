using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LightPi.OrchestratorEmulator
{
    public class SpriteSurface : Canvas
    {
        private BitmapImage _backgroundSprite;

        public SpriteSurface()
        {
            IsHitTestVisible = false;
        }

        public ObservableCollection<Output> Outputs { get; } = new ObservableCollection<Output>();

        public event EventHandler Updated;

        public void RegisterBackgroundSprite(string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            _backgroundSprite = TryLoadImageFromFile(filename);
        }

        public void RegisterOutput(int id, double watts, string spriteFilename)
        {
            if (spriteFilename == null) throw new ArgumentNullException(nameof(spriteFilename));

            var image = TryLoadImageFromFile(spriteFilename); ;
            Outputs.Add(new Output(id, watts, image, this));
        }

        public void Update()
        {
            InvalidateVisual();
            Updated?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (_backgroundSprite == null)
            {
                return;
            }

            var scaleHeight = ActualHeight / _backgroundSprite.Height;
            var scaleWidth = ActualWidth / _backgroundSprite.Width;
            var scale = Math.Min(scaleHeight, scaleWidth);

            var imageWidth = _backgroundSprite.Width * scale;
            var imageHeight = _backgroundSprite.Height * scale;

            var xPosition = ActualWidth / 2 - imageWidth / 2;
            var yPosition = ActualHeight / 2 - imageHeight / 2;

            var spriteRect = new Rect(xPosition, yPosition, imageWidth, imageHeight);

            dc.DrawImage(_backgroundSprite, spriteRect);

            foreach (var output in Outputs)
            {
                if (output.IsActive && output.Sprite != null)
                {
                    dc.DrawImage(output.Sprite, spriteRect);
                }
            }
        }

        private static BitmapImage TryLoadImageFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var image = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            image.Freeze();

            return image;
        }
    }
}
