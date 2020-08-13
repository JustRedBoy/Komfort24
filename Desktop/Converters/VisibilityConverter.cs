using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Desktop.Converters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((int)value) switch
        {
            0 => Visibility.Visible,
            1 => Visibility.Hidden,
            2 => Visibility.Collapsed,
            _ => throw new ArgumentException("Некорректное число для структуры Visibility")
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
