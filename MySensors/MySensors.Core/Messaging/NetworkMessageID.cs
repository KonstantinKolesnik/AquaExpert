
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

        // Sender: STATION / CLIENT
        // - Param "WebTheme"
        // - Param "UnitSystem", M or I
        // Sender: CLIENT
        // - No params: request for station settings
        public const string Settings = "Settings";

        // Sender: STATION / CLIENT
        // - Param "Version"
        // Sender: CLIENT
        // - No params: request for station version
        public const string Version = "Version";

        //-----------------------------------------------------------------------

        // Sender: CLIENT
        // - No params: request for all nodes list
        public const string GetNodes = "GetNodes";
    }
}
