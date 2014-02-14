using MFE.Core;
using System.Collections;
using System.Threading;

namespace BusNetwork.Network
{
    public abstract class BusHub : INotifyPropertyChanged
    {
        #region Fields
        private uint address = 0;
        private string name = "";
        private ArrayList busModules = new ArrayList();
        private ArrayList busControlLines = new ArrayList();
        private Timer timerUpdate = null;
        private const int updateInterval = 2000;
        #endregion

        #region Properties
        public uint Address
        {
            get { return address; }
        }
        public string ProductName
        {
            get { return (this is BusHubI2C) ?  "Integrated Hub" : "RF Hub"; }
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
        public ArrayList BusModules
        {
            get { return busModules; }
        }
        public BusModule this[ushort busModuleAddress]
        {
            get
            {
                foreach (BusModule busModule in busModules)
                    if (busModule.Address == busModuleAddress)
                        return busModule;

                return null;
            }
        }
        public ArrayList BusControlLines // control lines of all bus modules
        {
            get { return busControlLines; }
        }
        #endregion

        #region Constructors
        public BusHub(uint address)
        {
            this.address = address;
            StartTimer();
        }
        #endregion

        #region Events
        public event CollectionChangedEventHandler BusModulesCollectionChanged;
        public event CollectionChangedEventHandler BusControlLinesCollectionChanged;
        public event PropertyChangeEventHandler PropertyChanged;
        #endregion

        #region Private methods
        protected abstract void Scan();
        internal abstract bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response);
        internal abstract bool BusModuleWrite(BusModule busModule, byte[] request);

        protected void NotifyBusModulesCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        {
            ArrayList controlLinesAdded = new ArrayList();
            ArrayList controlLinesRemoved = new ArrayList();

            // removed control lines
            foreach (ushort address in addressesAdded)
                foreach (ControlLine controlLine in busControlLines)
                {
                    if (controlLine.BusModule.Address == address)
                    {
                        controlLinesRemoved.Add(controlLine);
                        busControlLines.Remove(controlLine);
                    }
                }

            // added control lines
            foreach (ushort address in addressesAdded)
            {
                BusModule busModule = this[address];
                foreach (ControlLine controlLine in busModule.ControlLines)
                {
                    controlLinesAdded.Add(controlLine);
                    busControlLines.Add(controlLine);
                }
            }

            // BusModulesCollectionChanged
            if (BusModulesCollectionChanged != null && (addressesAdded.Count != 0 || addressesRemoved.Count != 0))
                BusModulesCollectionChanged(addressesAdded, addressesRemoved);

            // BusControlLinesCollectionChanged
            if (BusControlLinesCollectionChanged != null && (controlLinesAdded.Count != 0 || controlLinesRemoved.Count != 0))
                BusControlLinesCollectionChanged(controlLinesAdded, controlLinesRemoved);
        }
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyName);
        }

        private void StartTimer()
        {
            timerUpdate = new Timer(new TimerCallback(Update), null, updateInterval, updateInterval);
        }
        private void StopTimer()
        {
            timerUpdate.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void Update(object state)
        {
            StopTimer();
            Scan();
            StartTimer();
        }
        #endregion
    }
}
