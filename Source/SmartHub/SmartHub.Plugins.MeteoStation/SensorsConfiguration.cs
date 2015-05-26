using System;

namespace SmartHub.Plugins.MeteoStation
{
    public class SensorsConfiguration
    {
        public Guid SensorTemperatureInnerID { get; set; }
        public Guid SensorTemperatureOuterID { get; set; }
        public Guid SensorHumidityInnerID { get; set; }
        public Guid SensorHumidityOuterID { get; set; }
        public Guid SensorAtmospherePressureID { get; set; }
        public Guid SensorForecastID { get; set; }
        public int Height { get; set; }

        public static SensorsConfiguration Default
        {
            get
            {
                return new SensorsConfiguration()
                {
                    SensorTemperatureInnerID = Guid.Empty,
                    SensorTemperatureOuterID = Guid.Empty,
                    SensorHumidityInnerID = Guid.Empty,
                    SensorHumidityOuterID = Guid.Empty,
                    SensorAtmospherePressureID = Guid.Empty,
                    SensorForecastID = Guid.Empty,
                    Height = 0
                };
            }
        }
    }
}
