using System;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Core.Xaml.ValueConverters
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            return str != null && !string.IsNullOrEmpty(str.Trim()) && !string.IsNullOrWhiteSpace(str.Trim());
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
