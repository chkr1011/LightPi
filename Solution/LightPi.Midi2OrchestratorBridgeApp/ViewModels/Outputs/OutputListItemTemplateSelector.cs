using System.Windows;
using System.Windows.Controls;
using LightPi.Midi2OrchestratorBridgeApp.Models;

namespace LightPi.Midi2OrchestratorBridgeApp.ViewModels.Outputs
{
    public class OutputListItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Output)
            {
                return OutputTemplate;
            }

            if (item is OutputGroup)
            {
                return OutputGroupTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate OutputTemplate { get; set; }

        public DataTemplate OutputGroupTemplate { get; set; }
    }
}
