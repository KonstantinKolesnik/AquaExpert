using System.ComponentModel;

namespace SmartHub.Plugins.Monitors.Core
{
    public enum MonitorType
    {
        [Description("Выключатель")]
        Switch,
        [Description("Температура")]
        Temperature,
        [Description("Влажность")]
        Humidity,
        [Description("Давление")]
        Pressure,




    }
}
