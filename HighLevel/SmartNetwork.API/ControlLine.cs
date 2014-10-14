using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNetwork.API
{
    public class ControlLine : ObservableObject
    {
        #region Fields
        private byte[] state = new byte[4];
        private string name = "";
        #endregion

        #region Properties
        public Coordinator Coordinator
        {
            get;
            private set;
        }
        public Module Module
        {
            get;
            private set;
        }
        public IEnumerable<ControlLineMode> Modes
        {
            get;
            private set;
        }
        public ControlLineMode Mode
        {
            get;
            private set;
        }
        public byte Address // up to 256 lines in a single module
        {
            get;
            private set;
        }
        //public string TypeName
        //{
        //    get
        //    {
        //        string type;

        //        switch (Type)
        //        {
        //            case ControlLineType.PWM: type = "PWM"; break;
        //            case ControlLineType.Relay: type = "Relay"; break;
        //            case ControlLineType.Liquid: type = "Liquid"; break;
        //            case ControlLineType.Ph: type = "Ph"; break;
        //            default: type = "[Unknown]"; break;
        //        }

        //        return "[" + (BusHub != null ? BusHub.Address.ToString() : "-") + "][" + (BusModule != null ? BusModule.Address.ToString() : "-") + "] " + type + " #" + Address;
        //    }
        //}
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

        #region Constructor
        public ControlLine(Coordinator coordinator, Module module, ControlLineMode mode, byte address)
        {
            Coordinator = coordinator;
            Module = module;
            Mode = mode;
            Address = address;

            //for (byte i = 0; i < state.Length; i++)
            //    state[i] = 0;
        }
        #endregion

        #region Public methods
        //public void QueryState()
        //{
        //    if (BusHub != null && Module != null)
        //    {
        //        byte[] request = new byte[] { BusModuleAPI.CmdGetControlLineState, (byte)Type, Address };
        //        byte[] response = new byte[state.Length];
        //        if (BusHub.BusModuleWriteRead(Module, request, response))
        //            state = response;
        //    }
        //}
        //public void SetState(byte[] state)
        //{
        //    if (BusHub != null && Module != null)
        //    {
        //        byte[] data = new byte[3 + state.Length];
        //        data[0] = BusModuleAPI.CmdSetControlLineState;
        //        data[1] = (byte)Type;
        //        data[2] = Address;
        //        Array.Copy(state, 0, data, 3, state.Length);

        //        byte[] response = new byte[state.Length];
        //        if (BusHub.BusModuleWriteRead(Module, data, response))
        //            State = response;
        //    }
        //}
        #endregion
    }
}
