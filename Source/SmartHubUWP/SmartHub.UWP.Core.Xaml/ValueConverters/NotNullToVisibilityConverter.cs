using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Core.Xaml.ValueConverters
{
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == DependencyProperty.UnsetValue || value == null ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
