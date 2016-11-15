using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightPi.Styles.Controls
{
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty DoubleClickProperty = DependencyProperty.RegisterAttached(
            "DoubleClickCommand",
            typeof(ICommand),
            typeof(DataGridExtensions),
            new PropertyMetadata(AttachOrRemoveDoubleClickEvent));

        public static readonly DependencyProperty DoubleClickCommandParameterProperty = DependencyProperty.RegisterAttached(
            "DoubleClickCommandParameter",
            typeof(object),
            typeof(DataGridExtensions),
            new PropertyMetadata(default(object)));

        public static void SetDoubleClickCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(DoubleClickCommandParameterProperty, value);
        }

        public static object GetDoubleClickCommandParameter(DependencyObject element)
        {
            return (object)element.GetValue(DoubleClickCommandParameterProperty);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickProperty, value);
        }

        public static void AttachOrRemoveDoubleClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = obj as DataGrid;
            if (dataGrid != null)
            {
                if (args.OldValue == null && args.NewValue != null)
                {
                    dataGrid.MouseDoubleClick += ExecuteDoubleClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    dataGrid.MouseDoubleClick -= ExecuteDoubleClick;
                }
            }
        }

        private static void ExecuteDoubleClick(object sender, MouseButtonEventArgs args)
        {
            var dependencyObject = (DependencyObject)sender;

            var parameter = GetDoubleClickCommandParameter(dependencyObject);
            var command = GetDoubleClickCommand(dependencyObject);

            if (command != null)
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
