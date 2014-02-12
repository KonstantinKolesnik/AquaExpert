using System.Collections;
using System.Threading;

namespace BusNetwork
{
    public abstract class BusMaster
    {
        #region Fields
        protected uint address = 0;
        private ArrayList busModules = new ArrayList();
        private Timer timerUpdate = null;
        private const int updateInterval = 2000;
        #endregion

        #region Properties
        public ArrayList BusModules
        {
            get { return busModules; }
        }
        public BusModule this[uint address]
        {
            get
            {
                foreach (BusModule busModule in busModules)
                    if (busModule.Address == address)
                        return busModule;

                return null;
            }
        }
        public ArrayList ControlLines
        {
            get
            {
                ArrayList list = new ArrayList();

                foreach (BusModule busModule in busModules)
                    foreach (ControlLine cl in busModule.ControlLines)
                        list.Add(cl);

                return list;
            }
        }
        #endregion

        #region Constructor
        public BusMaster()
        {
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

        #region Private methods
        protected abstract void ScanBusModules();
        protected abstract byte GetBusModuleType(ushort busModuleAddress);
        protected abstract void GetBusModuleControlLines(BusModule busModule);

        public abstract byte[] GetControlLineState(ControlLine controlLine);



        ////TODO: test!!!!!!!!!!!!!!!!
        //public bool GetRelayState(int idx)
        //{
        //    byte res = 0;
        //    I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
        //    if (Program.Bus.TryGetRegisters2(config, Program.BusTimeout, CMD_GET_RELAY_STATE, (byte)idx, new byte[] { res }))
        //        return res == 1;

        //    return false;
        //}
        //public void SetRelayState(int idx, bool on)
        //{
        //    I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
        //    if (Program.Bus.TrySetRegister(config, Program.BusTimeout, CMD_SET_RELAY_STATE, new byte[] { 0, (byte)(on ? 1 : 0) }))
        //    {
        //    }
        //}

        //public float GetTemperature(int idx)
        //{
        //    byte[] res = new byte[2];
        //    I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
        //    if (Program.Bus.TryGetRegisters2(config, Program.BusTimeout, CMD_GET_TEMPERATURE, (byte)idx, res))
        //    {
        //        return (float)res[0] + (float)res[1] / 100;
        //    }

        //    return 0;
        //}





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
            ScanBusModules();
            StartTimer();
        }
        #endregion
    }
}
