using System;
using System.Globalization;
using System.Windows.Data;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridge.UI.Converter
{
    public class MappingToNoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapping = value as MappingViewModel;
            if (mapping == null)
            {
                return string.Empty;
            }

            return $"{mapping.Channel}-{mapping.Note}{mapping.Octave}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
