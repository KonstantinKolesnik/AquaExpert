
namespace BusNetwork.Network
{
    public class ControlLine
    {
        #region Properties
        public uint BusMasterAddress
        {
            get;
            private set;
        }
        public ushort BusModuleAddress
        {
            get;
            private set;
        }
        public ControlLineType Type
        {
            get;
            private set;
        }
        public byte Number
        {
            get;
            private set;
        }
        public byte[] State
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
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

                return "[" + BusMasterAddress + "][" + BusModuleAddress + "] " + type + " #" + Number;
            }
        }
        #endregion

        #region Constructor
        public ControlLine(uint busMasterAddress, ushort busModuleAddress, ControlLineType type, byte number)
        {
            BusMasterAddress = busMasterAddress;
            BusModuleAddress = busModuleAddress;
            Type = type;
            Number = number;

            ResetState();
        }
        #endregion

        #region Public methods
        public void ResetState()
        {
            State = new byte[2] { 0, 0 };
        }
        #endregion
    }
}
