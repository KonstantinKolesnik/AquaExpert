using System;

namespace SmartNetwork.Core.Hardware
{
    public class ControlLine : ObservableObject
    {
        #region Fields
        private string name = "";
        private ControlLineMode mode;
        private float state = 0;
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
        public float State
        {
            get { return state; }
            private set
            {
                if (state == value)
                {
                    state = value;
                    NotifyPropertyChanged("State");
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
        }
        #endregion

        #region Public methods
        public void GetMode()
        {
            if (Module != null && Module.Coordinator != null)
            {
                byte[] request = new byte[] { (byte)CommandType.GetControlLineMode, Address };
                byte[] response = new byte[1];

                if (Module.Coordinator.WriteRead(Module, request, response))
                    Mode = (ControlLineMode)response[0];
            }
        }
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
                byte[] response = new byte[4];

                if (Module.Coordinator.WriteRead(Module, request, response))
                {
                    byte[] newArray = new[] { response[2], response[3], response[0], response[1] };
                    State = BitConverter.ToSingle(newArray, 0);
                }
            }
        }
        public void SetState(float state)
        {
            if (Module != null && Module.Coordinator != null)
            {
                byte[] request = new byte[2 + 4];
                request[0] = (byte)CommandType.SetControlLineState;
                request[1] = Address;
                //Array.Copy(state, 0, request, 2, state.Length);

                byte[] response = new byte[4];
                if (Module.Coordinator.WriteRead(Module, request, response))
                {
                    byte[] newArray = new[] { response[2], response[3], response[0], response[1] };
                    State = BitConverter.ToSingle(newArray, 0);
                }
            }
        }
        #endregion
    }
}
