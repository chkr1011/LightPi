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
            return (Geometry)element.GetValue(ImageProperty);
        }

        public static readonly DependencyProperty ImageBrushProperty = DependencyProperty.RegisterAttached(
            "ImageBrush", typeof(Brush), typeof(ButtonExtensions), new PropertyMetadata((Brush)Application.Current.FindResource("TextNormalBrush")));

        public static void SetImageBrush(DependencyObject element, Brush value)
        {
            element.SetValue(ImageBrushProperty, value);
        }

        public static Brush GetImageBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(ImageBrushProperty);
        }
    }
}
