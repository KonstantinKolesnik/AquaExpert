using System.Collections;
using System.Threading;

namespace BusNetwork.Network
{
    public abstract class BusMaster
    {
        #region Fields
        private uint address = 0;
        private ArrayList busModules = new ArrayList();
        private Timer timerUpdate = null;
        private const int updateInterval = 1000;
        #endregion

        #region Properties
        public uint Address
        {
            get { return address; }
        }
        public ArrayList BusModules
        {
            get { return busModules; }
        }
        public BusModule this[uint busModuleAddress]
        {
            get
            {
                foreach (BusModule busModule in busModules)
                    if (busModule.Address == busModuleAddress)
                        return busModule;

                return null;
            }
        }
        public ArrayList BusControlLines
        {
            get
            {
                ArrayList list = new ArrayList();

                foreach (BusModule busModule in busModules)
                    foreach (ControlLine cl in busModule.ControlLines)
                        list.Add(cl);

                return list;
            }
        } // control lines of all bus modules
        #endregion

        #region Constructors
        public BusMaster(uint address)
        {
            this.address = address;
            StartTimer();
        }
        #endregion

        #region Events
        public event CollectionChangedEventHandler BusModulesCollectionChanged;
        protected void NotifyBusModulesCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        {
            if (BusModulesCollectionChanged != null && (addressesAdded.Count != 0 || addressesRemoved.Count != 0))
                BusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }
        #endregion

        #region Public methods
        public abstract bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response);
        #endregion

        #region Private methods
        protected abstract void Scan();

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
