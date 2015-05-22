using SmartHub.Plugins.Weather.Api;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Linq;

namespace SmartHub.Plugins.Weather
{
    [Tile]
    public class WeatherTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic parameters)
        {
            tileWebModel.title = "Weather";
            tileWebModel.url = "webapp/weather/forecast";

            string strCityId = parameters.cityId;
            if (string.IsNullOrWhiteSpace(strCityId))
            {
                tileWebModel.content = "Missing cityId parameter";
                return;
            }
            Guid cityId;
            if (!Guid.TryParse(strCityId, out cityId))
            {
                tileWebModel.content = "CityId parameter must contain GUID value";
                return;
            }

            WeatherLocatioinModel location = Context.GetPlugin<WeatherPlugin>().GetWeatherData(DateTime.Now).FirstOrDefault(l => l.LocationId == cityId);

            if (location == null)
            {
                tileWebModel.content = string.Format("Локация с id = {0} не найдена", cityId);
                return;
            }

            tileWebModel.title = location.LocationName;

            // текущая погода
            if (location.Now == null)
            {
                tileWebModel.content = "<нет данных>";
                return;
            }
            tileWebModel.className = "btn-info th-tile-icon th-tile-icon-wa " + WeatherUtils.GetIconClass(location.Now.Code);

            string formattedNow = WeatherUtils.FormatTemperature(location.Now.Temperature);
            tileWebModel.content = string.Format("сейчас: {0}°C", formattedNow);

            // погода на завтра
            var tomorrow = location.Forecast.FirstOrDefault();
            if (tomorrow != null)
            {
                string formattedTomorrow = WeatherUtils.FormatTemperatureRange(tomorrow.MinTemperature, tomorrow.MaxTemperature);
                tileWebModel.content += string.Format("\nзавтра: {0}°C", formattedTomorrow);
            }
        }
    }
}
