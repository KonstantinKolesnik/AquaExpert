using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmartNetwork.Network
{
    public class Module : ObservableObject
    {
        #region Fields
        private Coordinator coordinator;
        private byte[] address;
        private string name = "";
        private ObservableCollection<ControlLine> controlLines = new ObservableCollection<ControlLine>();
        #endregion

        #region Properties
        internal Coordinator Coordinator
        {
            get { return coordinator; }
        }
        public byte[] Address
        {
            get { return address; }
        }
        public ModuleType Type
        {
            get { return (ModuleType)address[0]; }
        }
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case ModuleType.Unknown: return "SNM test module";
                    case ModuleType.D5: return "SNM-D5";
                    case ModuleType.D6: return "SNM-D6";
                    case ModuleType.D8: return "SNM-D8";


                    default: return string.Format("{0} [Unknown]", Type.ToString());
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
        internal Module(Coordinator coordinator, byte[] address)
        {
            this.coordinator = coordinator;
            this.address = address;

            QueryControlLines();
        }
        #endregion

        #region Private methods
        private void QueryControlLines(bool updateState = false)
        {
            if (coordinator != null)
            {
                byte[] linesCount = new byte[1] { 0 };
                if (coordinator.WriteRead(this, new byte[] { (byte)Commands.GetControlLinesCount }, linesCount))
                {
                    for (byte i = 0; i < linesCount[0]; i++)
                    {
                        byte[] lineInfo = new byte[3];
                        if (coordinator.WriteRead(this, new byte[] { (byte)Commands.GetControlLineInfo, i }, lineInfo))
                        {
                            ControlLine line = new ControlLine(this, lineInfo[0], lineInfo[1], (ControlLineMode)lineInfo[2]);
                            controlLines.Add(line);

                            // query control line state:
                            if (updateState)
                                line.QueryState();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
