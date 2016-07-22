using System;
using System.Collections.ObjectModel;
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

            _backgroundSprite = LoadImageFromFile(filename);
        }

        public void RegisterOutput(int id, double watts, string spriteFilename)
        {
            if (spriteFilename == null) throw new ArgumentNullException(nameof(spriteFilename));

            var image = LoadImageFromFile(spriteFilename); ;
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

            double scaleHeight = ActualHeight / _backgroundSprite.Height;
            double scaleWidth = ActualWidth / _backgroundSprite.Width;
            double scale = Math.Min(scaleHeight, scaleWidth);

            double imageWidth = _backgroundSprite.Width * scale;
            double imageHeight = _backgroundSprite.Height * scale;

            double xPosition = ActualWidth / 2 - imageWidth / 2;
            double yPosition = ActualHeight / 2 - imageHeight / 2;

            Rect spriteRect = new Rect(xPosition, yPosition, imageWidth, imageHeight);

            dc.DrawImage(_backgroundSprite, spriteRect);

            foreach (var output in Outputs)
            {
                if (output.IsActive)
                {
                    dc.DrawImage(output.Sprite, spriteRect);
                }
            }
        }

        private BitmapImage LoadImageFromFile(string filename)
        {
            var image = new BitmapImage(new Uri(filename, UriKind.RelativeOrAbsolute));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Fant);
            image.Freeze();

            return image;
        }
    }
}
