
namespace MySensors.Controllers.Communication
{
    class NetworkMessageID
    {
        // Sender: STATION
        // - Param "Msg": message
        // - Param "Type": type - "", "Warning", "Error"
        public const string Message = "Message";

        // Sender: STATION
        // - Param "WebTheme"
        // - Param "UnitSystem", M or I
        // Sender: CLIENT
        // - No params: request for station settings
        // - Param "WebTheme"
        // - Param "UnitSystem", M or I
        public const string Settings = "Settings";

        // Sender: STATION
        // - Param "Version"
        // Sender: CLIENT
        // - No params: request for station version
        public const string Version = "Version";

        // Sender: STATION
        // - Param "Ms"
        // Sender: CLIENT
        // - No params: request for station time
        // - Param "Ms"
        public const string Time = "Time";

        // Sender: STATION
        // - Param "Zone"
        // Sender: CLIENT
        // - No params: request for station timezone
        // - Param "Zone"
        public const string TimeZone = "TimeZone";

        //-----------------------------------------------------------------------

        // Sender: STATION
        // - Param "Nodes"
        // Sender: CLIENT
        // - No params: request for all nodes list
        public const string GetNodes = "GetNodes";

        // Sender: STATION
        // - Param "Node"
        public const string NodePresentation = "NodePresentation";

        // Sender: STATION
        // - Param "Sensor"
        public const string SensorPresentation = "SensorPresentation";

        // Sender: STATION
        // - Param "Level"
        public const string BatteryLevel = "BatteryLevel";

        // Sender: STATION
        // - Param "Value"
        public const string SensorValue = "SensorValue";

        // Sender: CLIENT
        // - Param "Msg"
        public const string SensorMessage = "SensorMsg";

        // Sender: STATION
        // - Param "Modules"
        // Sender: CLIENT
        // - No params: request for all modules list
        public const string GetModules = "GetModules";

        // Sender: STATION
        // - Param "Modules"
        // Sender: CLIENT
        // - No params: request for new module
        public const string AddModule = "AddModule";

        // Sender: CLIENT
        // - Param "Module"
        public const string SetModule = "SetModule";

        // Sender: CLIENT
        // - Param "ModuleID"
        public const string DeleteModule = "DeleteModule";

    }
}
