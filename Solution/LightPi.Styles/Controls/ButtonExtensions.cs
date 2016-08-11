using System.Windows;
using System.Windows.Media;

namespace LightPi.Styles.Controls
{
    public static class ButtonExtensions
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached(
            "Image", typeof(Geometry), typeof(ButtonExtensions), new PropertyMetadata(default(Geometry)));

        public static void SetImage(DependencyObject element, Geometry value)
        {
            element.SetValue(ImageProperty, value);
        }

        public static Geometry GetImage(DependencyObject element)
        {
            return (Geometry) element.GetValue(ImageProperty);
        }
    }
}
