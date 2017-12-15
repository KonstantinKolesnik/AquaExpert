using System;

namespace SmartHub.Plugins.Weather.Api
{
    public class WeatherLocatioinModel
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public WeatherNowDataModel Now { get; set; }
        public WeatherNowDataModel[] Today { get; set; }
        public WeatherDayDataModel[] Forecast { get; set; }
    }
}
