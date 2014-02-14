using MFE.Core;
using System.Collections;

namespace BusNetwork.Network
{
    public class BusModule : INotifyPropertyChanged
    {
        #region Fields
        private BusHub busHub;
        private ushort address = 0;
        private byte type = 255; // unknown
        private ArrayList controlLines = new ArrayList();
        private string name = "";
        #endregion

        #region Properties
        public BusHub BusHub
        {
            get { return busHub; }
        }
        public ushort Address
        {
            get { return address; }
        }
        public byte Type
        {
            get { return type; }
        }
        public string ProductName
        {
            get { return BusModuleProductName.Get(type); }
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
        public ArrayList ControlLines
        {
            get { return controlLines; }
        }
        #endregion

        #region Events
        public event PropertyChangeEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public BusModule(BusHub busHub, ushort address, byte type)
        {
            this.busHub = busHub;
            this.address = address;
            this.type = type;
        }
        #endregion

        #region Private methods
        //internal void QueryType()
        //{
        //    if (busMaster != null)
        //    {
        //        byte[] response = new byte[1];
        //        if (busMaster.BusModuleWriteRead(this, new byte[] { BusModule.CmdGetType }, response))
        //            Type = response[0];
        //    }
        //}
        internal void QueryControlLines()
        {
            if (busHub != null)
            {
                for (byte type = 0; type < BusModuleAPI.ControlLineTypesToRequest; type++)
                {
                    byte[] response = new byte[1]; // up to 256 numbers for one type
                    if (busHub.BusModuleWriteRead(this, new byte[] { BusModuleAPI.CmdGetControlLineCount, type }, response))
                    {
                        for (byte number = 0; number < response[0]; number++)
                        {
                            ControlLine controlLine = new ControlLine(busHub, this, (ControlLineType)type, number);
                            ControlLines.Add(controlLine);

                            // query control line state:
                            controlLine.QueryState();
                        }
                    }
                }
            }
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }
        #endregion
    }
}
