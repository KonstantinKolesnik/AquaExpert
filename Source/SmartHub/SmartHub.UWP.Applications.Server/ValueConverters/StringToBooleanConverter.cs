using System;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Applications.Server.ValueConverters
{
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
