using System;
using System.Globalization;
using System.Windows.Data;
using LightPi.Midi2OrchestratorBridge.ViewModels.Mappings;

namespace LightPi.Midi2OrchestratorBridge.UI.Converter
{
    public class MappingToOutputNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapping = (MappingViewModel)value;
            return string.Join(", ", mapping.Mapping.Outputs);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
