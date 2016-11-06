using System;
using System.Globalization;
using System.Windows.Data;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.UI.Converter
{
    public class ChannelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var channel = (MidiChannel) value;

            if (channel == MidiChannel.All)
            {
                return "<All>";
            }

            return "Channel " + (int) channel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
