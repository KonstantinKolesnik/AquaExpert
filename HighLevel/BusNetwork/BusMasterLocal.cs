using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace BusNetwork
{
    public class BusMasterLocal : BusMaster
    {
        #region Fields
        private BusConfiguration busConfig;
        #endregion

        #region Constructor
        public BusMasterLocal(BusConfiguration busConfig)
        {
            this.busConfig = busConfig;
        }
        #endregion

        #region Private methods
        //protected override void ScanBusModules()
        //{
        //    ArrayList addressesAdded = new ArrayList();
        //    ArrayList addressesRemoved = new ArrayList();

        //    // get all addresses on bus:
        //    ArrayList onlineAddresses = busConfig.Bus.Scan(1, 127, BusConfiguration.ClockRate, BusConfiguration.Timeout);

        //    // remove nonexisting modules:
        //    foreach (BusModule busModule in BusModules)
        //        if (!onlineAddresses.Contains(busModule.Address))
        //        {
        //            addressesRemoved.Add(busModule.Address);
        //            BusModules.Remove(busModule);
        //        }
            
        //    // add new modules:
        //    foreach (ushort address in onlineAddresses)
        //        if (this[address] == null) // no registered module with this address
        //        {
        //            byte type = GetBusModuleType(address);
        //            BusModule busModule = new BusModule(address, type);
        //            GetBusModuleControlLines(busModule);

        //            addressesAdded.Add(address);
        //            BusModules.Add(busModule);
        //        }

        //    NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        //}

        // for test!!!
        protected override void ScanBusModules()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            // get all addresses on bus:
            ArrayList onlineAddresses = busConfig.Bus.Scan(1, 127, BusConfiguration.ClockRate, BusConfiguration.Timeout);

            BusModules.Clear();

            // add new modules:
            foreach (ushort address in onlineAddresses)
            {
                byte type = GetBusModuleType(address);
                BusModule busModule = new BusModule(address, type);
                GetBusModuleControlLines(busModule);

                addressesAdded.Add(address);
                BusModules.Add(busModule);
            }

            NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }

        protected override byte GetBusModuleType(ushort busModuleAddress)
        {
            byte type = 0;

            I2CDevice.Configuration config = new I2CDevice.Configuration(busModuleAddress, BusConfiguration.ClockRate);
            if (!busConfig.Bus.TryGetRegister(config, BusConfiguration.Timeout, BusModule.CmdGetType, out type))
                type = 255; // set "unknown" type

            return type;
        }
        protected override void GetBusModuleControlLines(BusModule busModule)
        {
            for (byte i = 0; i < BusModule.MaxControlLineTypes; i++)
            {
                byte[] result = new byte[1];
                I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
                if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineCount, new byte[] {i}, result))
                    result[0] = 0;

                for (byte number = 0; number < result[0]; number++)
                    busModule.ControlLines.Add(new ControlLine(0, busModule.Address, (ControlLineType)i, number));
            }
        }

        public override byte[] GetControlLineState(ControlLine controlLine)
        {
            byte[] result = new byte[10];
            I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModuleAddress, BusConfiguration.ClockRate);
            int size = busConfig.Bus.GetRegistersAny(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineState, new byte[] { (byte)controlLine.Type, controlLine.Number }, result);

            byte[] res = new byte[size];
            for (int i = 0; i < size; i++)
                res[i] = result[i];

            return res;
        }



        #endregion
    }
}
