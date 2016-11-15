using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.UI
{
    public class SpriteSurface : Canvas
    {
        private readonly List<OutputSprite> _sprites = new List<OutputSprite>();
        private readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private BitmapImage _backgroundSprite;

        public SpriteSurface()
        {
            IsHitTestVisible = false;
        }

        public void SetBackground(string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));

            _backgroundSprite = TryLoadImageFromFile(filename);
        }

        public void AddOutput(OutputViewModel output, string spriteFilename)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (spriteFilename == null) throw new ArgumentNullException(nameof(spriteFilename));

            var image = TryLoadImageFromFile(spriteFilename); ;
            _sprites.Add(new OutputSprite(output, image));
        }

        public void Update()
        {
            InvalidateVisual();
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

            foreach (var sprite in _sprites)
            {
                if (sprite.Output.IsActive && sprite.Sprite != null)
                {
                    dc.DrawImage(sprite.Sprite, spriteRect);
                }
            }
        }

        private BitmapImage TryLoadImageFromFile(string filename)
        {
            filename = Path.Combine(_baseDirectory, filename);

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
