
namespace SmartHub.Plugins.MySensors.Core
{
    public enum SensorValueType : byte
    {
        Temperature = 0,    // Temperature
        Humidity = 1,       // Humidity

        Status,             // S_LIGHT, S_DIMMER, S_SPRINKLER, S_HVAC, S_HEATER. Used for setting/reporting binary (on/off) status. 1=on, 0=off  
        Light = 2,          // Same as V_STATUS
        Switch = 2,

        Percentage,         // S_DIMMER. Used for sending a percentage value 0-100 (%). 
        Dimmer = 3,         // S_DIMMER. Same as V_PERCENTAGE.
        Pressure = 4,       // Atmospheric Pressure
        Forecast = 5,       // Whether forecast. One of "stable", "sunny", "cloudy", "unstable", "thunderstorm" or "unknown"
        Rain = 6,           // Amount of rain
        RainRate = 7,       // Rate of rain
        Wind = 8,           // Windspeed
        Gust = 9,           // Gust
        Direction = 10,     // Wind direction
        UV = 11,            // UV light level
        Weight = 12,        // Weight (for scales etc)
        Distance = 13,      // Distance
        Impedance = 14,     // Impedance value
        Armed = 15,         // Armed status of a security sensor. 1=Armed, 0=Bypassed
        Tripped = 16,       // Tripped status of a security sensor. 1=Tripped, 0=Untripped
        Watt = 17,          // Watt value for power meters
        KWH = 18,           // Accumulated number of KWH for a power meter
        SceneOn = 19,       // Turn on a scene
        SceneOff = 20,      // Turn of a scene

        Heater = 21,        // Deprecated. Use V_HVAC_FLOW_STATE instead.
        HVACFlowState = 21, // S_HEATER, S_HVAC. HVAC flow state ("Off", "HeatOn", "CoolOn", or "AutoChangeOver")

        HVACSpeed,          // S_HVAC, S_HEATER. HVAC/Heater fan speed ("Min", "Normal", "Max", "Auto")
        LightLevel = 23,    // S_LIGHT_LEVEL. Uncalibrated light level. 0-100%. Use V_LEVEL for light level in lux
        Var1 = 24,          // Custom value
        Var2 = 25,          // Custom value
        Var3 = 26,          // Custom value
        Var4 = 27,          // Custom value
        Var5 = 28,          // Custom value
        Up = 29,            // Window covering. Up.
        Down = 30,          // Window covering. Down.
        Stop = 31,          // Window covering. Stop.
        IRSend = 32,        // Send out an IR-command
        IRReceive = 33,     // This message contains a received IR-command
        Flow = 34,          // Flow of water (in meter)
        Volume = 35,        // Water volume
        LockStatus = 36,    // Set or get lock status. 1=Locked, 0=Unlocked
        Level,              // S_DUST, S_AIR_QUALITY, S_SOUND (dB), S_VIBRATION (hz), S_LIGHT_LEVEL (lux)
        Voltage = 38,       // Voltage level
        Current = 39,       // Current level
        RGB,                // S_RGB_LIGHT, S_COLOR_SENSOR. Used for sending color information for multi color LED lighting or color sensors. Sent as ASCII hex: RRGGBB (RR=red, GG=green, BB=blue component)
        RGBW,               // S_RGBW_LIGHT. Used for sending color information to multi color LED lighting. Sent as ASCII hex: RRGGBBWW (WW=white component)
        ID,                 // S_TEMP. Used for sending in sensors hardware ids (i.e. OneWire DS1820b). 
        UnitPrefix,         // S_DUST, S_AIR_QUALITY. Allows sensors to send in a string representing the unit prefix to be displayed in GUI, not parsed by controller! E.g. cm, m, km, inch. Can be used for S_DISTANCE or gas concentration
        HVACSetpointCool,   // S_HVAC. HVAC cool setpoint (Integer between 0-100)
        HVACSetpointHeat,   // S_HEATER, S_HVAC. HVAC/Heater setpoint (Integer between 0-100)
        HVACFlowMode,       // S_HVAC. Flow mode for HVAC ("Auto", "ContinuousOn", "PeriodicOn")



        Ph = 200,           // Ph level
        ORP,                // ORP level, V/mV
    }
}
