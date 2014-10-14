using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartNetwork.API
{
    public class Module : ObservableObject
    {
        #region Fields
        private Coordinator coordinator;
        private byte[] address;
        private ModuleType type = ModuleType.Unknown;
        private string name = "";
        private IEnumerable<ControlLine> controlLines = new ObservableCollection<ControlLine>();
        #endregion

        #region Properties
        public Coordinator Coordinator
        {
            get { return coordinator; }
        }
        public byte[] Address
        {
            get { return address; }
        }
        public ModuleType Type
        {
            get { return type; }
        }
        public string TypeName
        {
            get
            {
                switch (type)
                {
                    case ModuleType.Unknown: return "SNM test module";
                    case ModuleType.D5: return "SNM-D5";
                    case ModuleType.D6: return "SNM-D6";
                    case ModuleType.D8: return "SNM-D8";


                    default: return string.Format("{0} [Unknown]", type.ToString());
                }
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
        public IEnumerable<ControlLine> ControlLines
        {
            get { return controlLines; }
        }
        #endregion

        #region Constructor
        public Module(Coordinator coordinator, byte[] address, ModuleType type)
        {
            this.coordinator = coordinator;
            this.address = address;
            this.type = type;
        }
        #endregion

        #region Private methods
        //internal void QueryType()
        //{
        //    if (coordinator != null)
        //    {
        //        byte[] response = new byte[1];
        //        if (coordinator.BusModuleWriteRead(this, new byte[] { Commands.GetType }, response))
        //            Type = (ModuleType)response[0];
        //    }
        //}
        //internal void QueryControlLines(bool updateState = false)
        //{
        //    if (busHub != null)
        //    {
        //        for (byte type = 0; type < BusModuleAPI.ControlLineTypesToRequest; type++)
        //        {
        //            byte[] response = new byte[1]; // up to 256 numbers for one type
        //            if (busHub.BusModuleWriteRead(this, new byte[] { BusModuleAPI.CmdGetControlLineCount, type }, response))
        //            {
        //                for (byte number = 0; number < response[0]; number++)
        //                {
        //                    ControlLine controlLine = new ControlLine(busHub, this, (ControlLineType)type, number);
        //                    ControlLines.Add(controlLine);

        //                    // query control line state:
        //                    if (updateState)
        //                        controlLine.QueryState();
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
