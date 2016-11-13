using System;
using System.Globalization;
using System.Windows.Data;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Converter
{
    public class ChannelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Channel " + (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
