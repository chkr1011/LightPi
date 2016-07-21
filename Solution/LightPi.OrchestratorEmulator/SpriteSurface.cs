using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<Sprite> Sprites { get; } = new ObservableCollection<Sprite>();

        public void RegisterBackgroundSprite(string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            _backgroundSprite = LoadImageFromFile(filename);
        }

        public void RegisterOutputSprite(int outputId, string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            var image = LoadImageFromFile(filename); ;
            Sprites.Add(new Sprite(outputId, image));
        }

        public void SetOutputState(int outputId, bool state)
        {
            Sprite sprite = Sprites.FirstOrDefault(s => s.OutputId == outputId);
            if (sprite == null)
            {
                return;
            }

            sprite.IsVisible = state;
        }

        public void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_backgroundSprite == null)
            {
                return;
            }

            double scaleHeight = ActualHeight / _backgroundSprite.Height;
            double scaleWidth = ActualWidth / _backgroundSprite.Width;
            double scale = Math.Min(scaleHeight, scaleWidth);

            double imageWidth = _backgroundSprite.Width * scale;
            double imageHeight = _backgroundSprite.Height * scale;
            double yPosition = ActualHeight / 2 - imageHeight / 2;
            double xPosition = ActualWidth / 2 - imageWidth / 2;

            Rect spriteRect = new Rect(xPosition, yPosition, imageWidth, imageHeight);

            dc.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
            dc.DrawImage(_backgroundSprite, spriteRect);

            foreach (var sprite in Sprites)
            {
                if (sprite.IsVisible)
                {
                    dc.DrawImage(sprite.Image, spriteRect);
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
