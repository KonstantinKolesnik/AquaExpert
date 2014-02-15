using MFE.Core;
using System.Collections;
using System.Threading;

namespace BusNetwork.Network
{
    public class NetworkCoordinator
    {
        #region Fields
        private ArrayList busHubs = new ArrayList();
        private ArrayList busModules = new ArrayList();
        private ArrayList busControlLines = new ArrayList();
        private Timer timerUpdate = null;
        private const int updateInterval = 2000;
        #endregion

        #region Properties
        public ArrayList BusHubs
        {
            get { return busHubs; }
        }
        public ArrayList BusModules
        {
            get { return busModules; }
        }
        public ArrayList BusControlLines
        {
            get { return busControlLines; }
        }
        #endregion

        #region Events
        public event CollectionChangedEventHandler BusHubsCollectionChanged;
        private void NotifyBusHubsCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        {
            if (BusHubsCollectionChanged != null && (addressesAdded.Count != 0 || addressesRemoved.Count != 0))
                BusHubsCollectionChanged(addressesAdded, addressesRemoved);
        }
        #endregion

        private void Scan()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            // get all addresses in the network:
            //ArrayList onlineAddresses = busConfig.Bus.Scan(1, 127, BusConfiguration.ClockRate, BusConfiguration.Timeout);


            //// remove nonexisting modules:
            //foreach (BusMaster busMaster in BusMasters)
            //    if (!onlineAddresses.Contains(busMaster.Address) && !(busMaster is BusMasterLocal))
            //    {
            //        addressesRemoved.Add(busMaster.Address);
            //        BusMasters.Remove(busMaster);
            //    }

            //// add new modules:
            //foreach (ushort address in onlineAddresses)
            //    if (this[address] == null) // no registered module with this address
            //    {
            //        byte type = GetBusModuleType(address);
            //        BusModule busModule = new BusModule(address, type);
            //        GetBusModuleControlLines(busModule);

            //        addressesAdded.Add(address);
            //        BusModules.Add(busModule);
            //    }

            //NotifyBusHubsCollectionChanged(addressesAdded, addressesRemoved);
        }
    }
}

//subscribe on hubs'
        //public event CollectionChangedEventHandler BusModulesCollectionChanged;
        //public event CollectionChangedEventHandler BusControlLinesCollectionChanged;
        //public event PropertyChangeEventHandler PropertyChanged;
