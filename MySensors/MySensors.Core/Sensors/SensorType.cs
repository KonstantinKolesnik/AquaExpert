
namespace MySensors.Core.Sensors
{
    public enum SensorType : byte
    {
        Door =                  0,      // Door and window sensors
        Motion =                1,      // Motion sensors
        Smoke =                 2,      // Smoke sensor
        Light =                 3,      // Light Actuator (on/off)
        Dimmer =                4,      // Dimmable device of some kind
        Cover =                 5,      // Window covers or shades
        Temperature =           6,      // Temperature sensor
        Humidity =              7,      // Humidity sensor
        Barometer =             8,      // Barometer sensor (Pressure)
        Wind =                  9,      // Wind sensor
        Rain =                  10,     // Rain sensor
        UV =                    11,     // UV sensor
        Weight =                12,     // Weight sensor for scales etc.
        Power =                 13,     // Power measuring device, like power meters
        Heater =                14,     // Heater device
        Distance =              15,     // Distance sensor
        LightLevel =            16,     // Light sensor
        ArduinoNode =           17,     // Arduino node device
        ArduinoRelay =          18,     // Arduino repeating node device
        Lock =                  19,     // Lock device
        IR =                    20,     // IR sender/receiver device
        Water =                 21,     // Water meter
        AirQuality =            22,     // Air quality sensor e.g. MQ-2
        Custom =                23,     // Use this for custom sensors where no other fits.
        Dust =                  24,     // Dust level sensor
        SceneController =       25,     // Scene controller device





        //ArduinoGateway =        255,     // Arduino gateway device
    }
}
