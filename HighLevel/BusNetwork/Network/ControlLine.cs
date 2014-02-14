
namespace BusNetwork.Network
{
    public class ControlLine
    {
        #region Fields
        private byte[] state = new byte[2];
        #endregion

        #region Properties
        public BusMaster BusMaster
        {
            get;
            private set;
        }
        public BusModule BusModule
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
            get
            {
                return state;

                int length = state.Length;

                byte[] result = new byte[length];

                for (int i = 0; i < length; i++)
                    result[i] = state[i];
                
                return result;
            }
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

                return "[" + (BusMaster != null ? BusMaster.Address.ToString() : "-") + "][" + (BusModule != null ? BusModule.Address.ToString() : "-") + "] " + type + " #" + Number;
            }
        }
        #endregion

        #region Constructor
        public ControlLine(BusMaster busMaster, BusModule busModule, ControlLineType type, byte number)
        {
            BusMaster = busMaster;
            BusModule = busModule;
            Type = type;
            Number = number;

            ResetState();
        }
        #endregion

        #region Public methods
        public void ResetState()
        {
            state = new byte[2] { 0, 0 };
        }
        #endregion
    }
}
