using System.Collections;

namespace BusNetwork
{
    public class BusModule
    {
        #region API
        public const byte MaxControlLineTypes = 32;

        public const int CmdGetType = 0;
        public const int CmdGetControlLineCount = 1;
        //public const int CMD_GET_RELAY_STATE = 2;
        //public const int CMD_SET_RELAY_STATE = 3;
        //public const int CMD_GET_TEMPERATURE = 4;

        #endregion

        #region Fields
        private ushort address = 0;
        private byte type = 255; // unknown
        private ArrayList controlLines = new ArrayList();
        #endregion

        #region Properties
        public ushort Address
        {
            get { return address; }
        }
        public byte Type
        {
            get { return type; }
        }
        public string FriendlyName
        {
            get
            {
                switch (type)
                {
                    case 0: return "AE full module";
                    case 1: return "AE-R8";

                    default: return type.ToString() + " [Unknown]";
                }
            }
        }
        public ArrayList ControlLines
        {
            get { return controlLines; }
        }
        #endregion

        #region Constructor
        public BusModule(ushort address, byte type)
        {
            this.address = address;
            this.type = type;
        }
        #endregion
    }
}
