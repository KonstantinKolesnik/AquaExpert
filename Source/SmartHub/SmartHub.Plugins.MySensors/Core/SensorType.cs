
namespace SmartHub.Plugins.MySensors.Core
{
    public enum SensorType : byte
    {
        Door = 0,               // Door and window sensors
        Motion = 1,             // Motion sensors
        Smoke = 2,              // Smoke sensor
        Light = 3,              // Light Actuator (on/off)
        Binary = 3,             // Binary light or relay, V_STATUS (or V_LIGHT), V_WATT (same as S_LIGHT)
        Dimmer = 4,             // Dimmable device of some kind
        Cover = 5,              // Window covers or shades
        Temperature = 6,        // Temperature sensor
        Humidity = 7,           // Humidity sensor
        Barometer = 8,          // Barometer sensor (Pressure)
        Wind = 9,               // Wind sensor
        Rain = 10,              // Rain sensor
        UV = 11,                // UV sensor
        Weight = 12,            // Weight sensor for scales etc.
        Power = 13,             // Power measuring device, like power meters
        Heater = 14,            // Heater device
        Distance = 15,          // Distance sensor
        LightLevel = 16,        // Light sensor
        Device = 17,       // Arduino node device
        Repeater = 18,          // Arduino repeating node device
        Lock = 19,              // Lock device
        IR = 20,                // IR sender/receiver device
        Water = 21,             // Water meter
        AirQuality = 22,        // Air quality sensor e.g. MQ-2
        Custom = 23,            // Use this for custom sensors where no other fits.
        Dust = 24,              // Dust level sensor
        SceneController = 25,   // Scene controller device
        RGBLight,               // RGB light. Send color component data using V_RGB. Also supports V_WATT 
        RGBWLight,              // RGB light with an additional White component. Send data using V_RGBW. Also supports V_WATT
        ColorSensor,            // Color sensor, send color information using V_RGB
        HVAC,                   // Thermostat/HVAC device. V_HVAC_SETPOINT_HEAT, V_HVAC_SETPOINT_COLD, V_HVAC_FLOW_STATE, V_HVAC_FLOW_MODE, V_TEMP
        Multimeter,             // Multimeter device, V_VOLTAGE, V_CURRENT, V_IMPEDANCE 
        Sprinkler,              // Sprinkler, V_STATUS (turn on/off), V_TRIPPED (if fire detecting device)
        WaterLeak,              // Water leak sensor, V_TRIPPED, V_ARMED
        Sound,                  // Sound sensor, V_TRIPPED, V_ARMED, V_LEVEL (sound level in dB)
        Vibration,              // Vibration sensor, V_TRIPPED, V_ARMED, V_LEVEL (vibration in Hz)
        Moisture,               // Moisture sensor, V_TRIPPED, V_ARMED, V_LEVEL (water content or moisture in percentage?) 


        Ph = 200,               // Ph sensor
        ORP,                    // ORP sensor
        Informer,               // Display device
    }
}
