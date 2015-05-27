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
            tileWebModel.title = "Погода";
            tileWebModel.url = "webapp/weather/forecast";

            string strCityId = parameters.cityId;
            if (string.IsNullOrWhiteSpace(strCityId))
            {
                tileWebModel.content = "Отсутствует cityId параметр";
                return;
            }
            Guid cityId;
            if (!Guid.TryParse(strCityId, out cityId))
            {
                tileWebModel.content = "Параметр cityId должен содержать GUID";
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
                tileWebModel.content = "&lt;нет данных&gt";
            else
            {
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-wa " + WeatherUtils.GetIconClass(location.Now.Code);
                tileWebModel.content = string.Format("сейчас: {0}°C", WeatherUtils.FormatTemperature(location.Now.Temperature));

                // погода на завтра
                var tomorrow = location.Forecast.FirstOrDefault();
                if (tomorrow != null)
                    tileWebModel.content += string.Format("\nзавтра: {0}°C", WeatherUtils.FormatTemperatureRange(tomorrow.MinTemperature, tomorrow.MaxTemperature));
            }
        }
    }
}
