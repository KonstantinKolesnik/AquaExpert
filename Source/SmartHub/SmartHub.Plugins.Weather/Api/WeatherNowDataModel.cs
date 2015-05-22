using System;

namespace SmartHub.Plugins.Weather.Api
{
    public class WeatherNowDataModel
    {
        public DateTime DateTime { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public int Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }
}
