using System;

namespace SmartHub.Plugins.Weather.Data
{
    public class WeatherData
    {
        public virtual Guid Id { get; set; }
        public virtual Location Location { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string WeatherCode { get; set; }
        public virtual string WeatherDescription { get; set; }

        /// <summary>
        /// Атмосферное давление
        /// </summary>
        public virtual decimal Pressure { get; set; }

        /// <summary>
        /// Температура, С
        /// </summary>
        public virtual decimal Temperature { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        public virtual int Humidity { get; set; }

        /// <summary>
        /// Облачность, %
        /// </summary>
        public virtual int Cloudiness { get; set; }

        /// <summary>
        /// Скорость ветра, м/с
        /// </summary>
        public virtual decimal WindSpeed { get; set; }

        /// <summary>
        /// Направление ветра, deg
        /// </summary>
        public virtual decimal WindDirection { get; set; }
    }
}
