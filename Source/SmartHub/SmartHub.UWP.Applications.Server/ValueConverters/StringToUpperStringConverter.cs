using System;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Applications.Server.ValueConverters
{
    public class StringToUpperStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            return !string.IsNullOrEmpty(str) ? str.ToUpper() : "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            return !string.IsNullOrEmpty(str) ? str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower() : "";
        }
    }
}
