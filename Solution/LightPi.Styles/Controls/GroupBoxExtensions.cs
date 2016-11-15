using System.Windows;

namespace LightPi.Styles.Controls
{
    public static class GroupBoxExtensions
    {
        public static readonly DependencyProperty ShowContentProperty = DependencyProperty.RegisterAttached(
            "ShowContent", typeof(bool), typeof(GroupBoxExtensions), new PropertyMetadata(true));

        public static readonly DependencyProperty CanHideContentProperty = DependencyProperty.RegisterAttached(
            "CanHideContent", typeof(bool), typeof(GroupBoxExtensions), new PropertyMetadata(default(bool)));

        public static void SetCanHideContent(DependencyObject element, bool value)
        {
            element.SetValue(CanHideContentProperty, value);
        }

        public static bool GetCanHideContent(DependencyObject element)
        {
            return (bool) element.GetValue(CanHideContentProperty);
        }
        public static void SetShowContent(DependencyObject element, bool value)
        {
            element.SetValue(ShowContentProperty, value);
        }

        public static bool GetShowContent(DependencyObject element)
        {
            return (bool) element.GetValue(ShowContentProperty);
        }
    }
}
