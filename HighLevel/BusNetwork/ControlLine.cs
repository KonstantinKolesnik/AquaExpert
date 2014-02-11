
namespace BusNetwork
{
    public class ControlLine
    {
        public uint BusMasterAddress { get; private set; }
        public uint BusModuleAddress { get; private set; }
        public ControlLineType Type { get; private set; }
        public int Number { get; private set; }

        public string UserName { get; set; }
        public string FriendlyName
        {
            get
            {
                string type;

                switch (Type)
                {
                    case ControlLineType.Relay: type = "Relay"; break;
                    case ControlLineType.WaterSensor: type = "Water sensor"; break;
                    case ControlLineType.PHSensor: type = "PH sensor"; break;
                    case ControlLineType.ORPSensor: type = "ORP sensor"; break;
                    case ControlLineType.ConductivitySensor: type = "Conductivity sensor"; break;
                    case ControlLineType.TemperatureSensor: type = "Temperature sensor"; break;
                    case ControlLineType.Dimmer: type = "Dimmer"; break;
                    default: type = "[Unknown]"; break;
                }

                return "[" + BusMasterAddress + ":" + BusModuleAddress + "] " + type + " # " + Number;
            }
        }

        public ControlLine(uint busMasterAddress, uint busModuleAddress, ControlLineType type, int number)
        {
            BusMasterAddress = busMasterAddress;
            BusModuleAddress = busModuleAddress;
            Type = type;
            Number = number;
        }
    }
}
