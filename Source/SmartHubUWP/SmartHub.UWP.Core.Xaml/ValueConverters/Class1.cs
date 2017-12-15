using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Core.Xaml.ValueConverters
{
    public class SizeToPropertyNameStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Application.Current.Resources[(bool) value ? "PropertyNameSmallStyle" : "PropertyNameStyle"] as Style;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
    public class SizeToPropertyValueHStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Application.Current.Resources[(bool) value ? "PropertyValueSmallHStyle" : "PropertyValueHStyle"] as Style;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
    public class SizeToPropertyValueVStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Application.Current.Resources[(bool) value ? "CaptionTextBlockStyle" : "BodyTextBlockStyle"] as Style;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
