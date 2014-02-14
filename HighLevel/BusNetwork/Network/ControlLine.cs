using MFE.Core;
using System;

namespace BusNetwork.Network
{
    public class ControlLine : INotifyPropertyChanged
    {
        #region Fields
        private byte[] state = new byte[2];
        private string name = "";
        #endregion

        #region Properties
        public BusHub BusHub
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
        public string ProductName
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

                return "[" + (BusHub != null ? BusHub.Address.ToString() : "-") + "][" + (BusModule != null ? BusModule.Address.ToString() : "-") + "] " + type + " #" + Number;
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public byte[] State
        {
            get
            {
                byte[] result = new byte[state.Length];
                Array.Copy(state, result, state.Length);
                return result;
            }
            private set
            {
                if (value.Length == state.Length)
                    for (ushort i = 0; i < state.Length; i++)
                        if (state[i] != value[i])
                        {
                            Array.Copy(value, state, state.Length);
                            NotifyPropertyChanged("State");
                            break;
                        }
            }
        }
        #endregion

        #region Events
        public event PropertyChangeEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public ControlLine(BusHub busMaster, BusModule busModule, ControlLineType type, byte number)
        {
            BusHub = busMaster;
            BusModule = busModule;
            Type = type;
            Number = number;

            for (byte i = 0; i < state.Length; i++)
                state[i] = 0;
        }
        #endregion

        #region Public methods
        public void QueryState()
        {
            if (BusHub != null)
            {
                byte[] response = new byte[state.Length];
                if (BusHub.BusModuleWriteRead(BusModule, new byte[] { BusModuleAPI.CmdGetControlLineState, (byte)Type, Number }, response))
                    State = response;
            }
        }
        public void SetState(byte[] state)
        {
            if (BusHub != null)
            {
                byte[] data = new byte[3 + state.Length];
                data[0] = BusModuleAPI.CmdSetControlLineState;
                data[1] = (byte)Type;
                data[2] = Number;
                Array.Copy(state, 0, data, 3, state.Length);

                byte[] response = new byte[state.Length];
                if (BusHub.BusModuleWriteRead(BusModule, data, response))
                    State = response;
            }
        }
        #endregion

        #region Private methods
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }
        #endregion
    }
}
