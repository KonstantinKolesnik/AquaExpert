
namespace MySensors.Core.Messaging
{
    public class NetworkMessageID
    {
        // Sender: STATION
        // - Param "Msg": message
        public const string OK = "OK";

        // Sender: STATION
        // - Param "Msg": message
        public const string Information = "Information";

        // Sender: STATION
        // - Param "Msg": message
        public const string Warning = "Warning";

        // Sender: STATION
        // - Param "Msg": message
        public const string Error = "Error";

        //-----------------------------------------------------------------------

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
        // - Param "Level"
        public const string BatteryLevel = "BatteryLevel";

        // Sender: STATION
        // - Param "Value"
        public const string SensorValue = "SensorValue";


        
    }
}
