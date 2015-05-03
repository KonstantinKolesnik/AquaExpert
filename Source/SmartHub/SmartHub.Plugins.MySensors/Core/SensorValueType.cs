
namespace SmartHub.Plugins.MySensors.Core
{
    public enum SensorValueType : byte
    {
        Temperature = 0,    // Temperature
        Humidity = 1,       // Humidity
        //Light = 2,        // Light status. 0=off 1=on
        Switch = 2,         // Switch status. 0=off 1=on
        Dimmer = 3,         // Dimmer value. 0-100%
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
        Heater = 21,        // Mode of header. One of "Off", "HeatOn", "CoolOn", or "AutoChangeOver"
        HeaterSW = 22,      // Heater switch power. 1=On, 0=Off
        LightLevel = 23,    // Light level. 0-100%
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
        DustLevel = 37,     // Dust level
        Voltage = 38,       // Voltage level
        Current = 39,       // Current level
        Ph = 40,            // Ph level
        ORP = 41,           // ORP level, V/mV
    }
}
