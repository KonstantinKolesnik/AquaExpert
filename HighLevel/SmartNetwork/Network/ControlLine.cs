using System;

namespace SmartNetwork.Network
{
    public class ControlLine : ObservableObject
    {
        #region Fields
        private string name = "";
        private ControlLineMode mode;
        private byte[] state = new byte[4];
        #endregion

        #region Properties
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
        public Module Module
        {
            get;
            private set;
        }
        public byte Address // up to 256 lines in a single module
        {
            get;
            private set;
        }
        public byte Modes // combination of ControlLineMode flags
        {
            get;
            private set;
        }
        public ControlLineMode Mode // active mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    NotifyPropertyChanged("Mode");
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
        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}] {3}", Module != null ? Module.Address.ToString() : "-",  Address, Mode.ToString(), Name);
        }
        #endregion

        #region Constructor
        public ControlLine(Module module, byte address, byte modes, ControlLineMode mode)
        {
            Module = module;
            Address = address;
            Modes = modes;
            Mode = mode;

            for (byte i = 0; i < state.Length; i++)
                state[i] = 0;
        }
        #endregion

        #region Public methods
        public void SetMode(ControlLineMode mode)
        {
            if (Module != null && Module.Coordinator != null)
            {
                byte[] request = new byte[] { (byte)CommandType.SetControlLineMode, Address, (byte)mode };
                byte[] response = new byte[1];

                if (Module.Coordinator.WriteRead(Module, request, response))
                    Mode = (ControlLineMode)response[0];
            }
        }
        public void QueryState()
        {
            if (Module != null && Module.Coordinator != null)
            {
                byte[] request = new byte[] { (byte)CommandType.GetControlLineState, Address };
                byte[] response = new byte[state.Length];

                if (Module.Coordinator.WriteRead(Module, request, response))
                    State = response;
            }
        }
        public void SetState(byte[] state)
        {
            if (Module != null && Module.Coordinator != null)
            {
                byte[] request = new byte[2 + state.Length];
                request[0] = (byte)CommandType.SetControlLineState;
                request[1] = Address;
                Array.Copy(state, 0, request, 2, state.Length);

                byte[] response = new byte[state.Length];
                if (Module.Coordinator.WriteRead(Module, request, response))
                    State = response;
            }
        }
        #endregion
    }
}
