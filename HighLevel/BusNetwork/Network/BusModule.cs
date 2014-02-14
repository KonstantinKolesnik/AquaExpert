using System.Collections;

namespace BusNetwork.Network
{
    public class BusModule
    {
        #region API
        public const byte ControlLineTypesToRequest = 32;

        public const byte CmdGetType = 0;
        public const byte CmdGetControlLineCount = 1;
        public const byte CmdGetControlLineState = 2;
        public const byte CmdSetControlLineState = 3;
        #endregion

        #region Fields
        private BusMaster busMaster;
        private ushort address = 0;
        private byte type = 255; // unknown
        private ArrayList controlLines = new ArrayList();
        #endregion

        #region Properties
        public BusMaster BusMaster
        {
            get { return busMaster; }
        }
        public ushort Address
        {
            get { return address; }
        }
        public byte Type
        {
            get { return type; }
            private set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        public string FriendlyName
        {
            get { return BusModuleName.Get(type); }
        }
        public string UserName
        {
            get;
            set;
        }
        public ArrayList ControlLines
        {
            get { return controlLines; }
        }
        #endregion

        #region Events
        public event PropertyChangeEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }

        public event CollectionChangedEventHandler ControlLinesCollectionChanged;
        protected void NotifyControlLinesCollectionChanged(ArrayList controlLinesAdded, ArrayList controlLinesRemoved)
        {
            if (ControlLinesCollectionChanged != null && (controlLinesAdded.Count != 0 || controlLinesRemoved.Count != 0))
                ControlLinesCollectionChanged(controlLinesAdded, controlLinesRemoved);
        }
        #endregion

        #region Constructor
        public BusModule(BusMaster busMaster, ushort address, byte type)
        {
            this.busMaster = busMaster;
            this.address = address;
            this.type = type;
        }
        #endregion

        public void RequestType()
        {
            if (busMaster != null)
            {
                byte[] response = new byte[1];
                if (busMaster.BusModuleWriteRead(this, new byte[] { BusModule.CmdGetType }, response))
                    Type = response[0];
            }
        }
        public void RequestControlLines()
        {
            if (busMaster != null)
            {
                for (byte type = 0; type < BusModule.ControlLineTypesToRequest; type++)
                {
                    byte[] response = new byte[1]; // up to 256 numbers for one type
                    if (busMaster.BusModuleWriteRead(this, new byte[] { BusModule.CmdGetControlLineCount, type }, response))
                    {
                        for (byte number = 0; number < response[0]; number++)
                        {
                            ControlLine controlLine = new ControlLine(busMaster, this, (ControlLineType)type, number);
                            ControlLines.Add(controlLine);


                            //NotifyControlLinesCollectionChanged


                            // query control line state:
                            //GetControlLineState(controlLine);
                        }
                    }
                }
            }
        }
    }
}
