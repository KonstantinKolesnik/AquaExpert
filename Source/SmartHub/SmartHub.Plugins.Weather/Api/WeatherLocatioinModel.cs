﻿using System;

namespace SmartHub.Plugins.Weather.Api
{
    public class WeatherLocatioinModel
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public WeatherDataModel Now { get; set; }
        public WeatherDataModel[] Today { get; set; }
        public DailyWeatherDataModel[] Forecast { get; set; }
    }
}
