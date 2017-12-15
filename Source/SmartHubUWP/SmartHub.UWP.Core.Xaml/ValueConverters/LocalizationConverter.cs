using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace SmartHub.UWP.Core.Xaml.ValueConverters
{
    public sealed class LocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var loader = ResourceLoader.GetForViewIndependentUse("SmartHub.UWP.Core.StringResources.Labels");

            var labelId = parameter as string;
            return !string.IsNullOrEmpty(labelId) ? loader.GetString(labelId) : string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
