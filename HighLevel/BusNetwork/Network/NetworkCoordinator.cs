using MFE.Core;
using System.Collections;

namespace BusNetwork.Network
{
    public class NetworkCoordinator
    {
        private ArrayList busMasters = new ArrayList();

        public ArrayList BusMasters
        {
            get { return busMasters; }
        }

        #region Events
        public event CollectionChangedEventHandler BusMastersCollectionChanged;
        private void NotifyBusMastersCollectionChanged(ArrayList addressesAdded, ArrayList addressesRemoved)
        {
            if (BusMastersCollectionChanged != null && (addressesAdded.Count != 0 || addressesRemoved.Count != 0))
                BusMastersCollectionChanged(addressesAdded, addressesRemoved);
        }
        #endregion

        public void Scan()
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

            NotifyBusMastersCollectionChanged(addressesAdded, addressesRemoved);
        }
    }
}
