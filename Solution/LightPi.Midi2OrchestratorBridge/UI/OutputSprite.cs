using System;
using System.Windows.Media.Imaging;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.UI
{
    public sealed class OutputSprite
    {
        public OutputSprite(OutputViewModel output, BitmapImage sprite)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));
            
            Output = output;
            Sprite = sprite;
        }

        public OutputViewModel Output { get; }
        
        public BitmapImage Sprite { get; }
    }
}
