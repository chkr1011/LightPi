using System;
using System.Globalization;
using System.Windows.Data;
using LightPi.Midi2OrchestratorBridge.Models;

namespace LightPi.Midi2OrchestratorBridge.UI.Converter
{
    public class NoteToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as NoteEventReceivedEventArgs;
            if (eventArgs == null)
            {
                return "-";
            }

            return $"{eventArgs.Channel}-{eventArgs.Note}{eventArgs.Octave}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
