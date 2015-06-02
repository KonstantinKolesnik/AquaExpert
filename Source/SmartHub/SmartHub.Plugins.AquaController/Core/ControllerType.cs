using System.ComponentModel;

namespace SmartHub.Plugins.AquaController.Core
{
    public enum ControllerType
    {
        [Description("Обогреватель")]
        Heater,
        [Description("Освещение")]
        Light,
        [Description("Уровень воды")]
        WaterLevel,
        [Description("PH")]
        PH,
        [Description("ORP")]
        ORP,
        [Description("CO2")]
        CO2,
        [Description("Кормление")]
        Feeder,

        [Description("Другой")]
        Custom
    }
}
