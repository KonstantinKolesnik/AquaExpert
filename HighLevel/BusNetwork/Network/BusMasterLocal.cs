using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace BusNetwork.Network
{
    public class BusMasterLocal : BusMaster
    {
        #region Fields
        private BusConfiguration busConfig;
        #endregion

        #region Constructor
        public BusMasterLocal(BusConfiguration busConfig)
            : base(0)
        {
            this.busConfig = busConfig;
        }
        #endregion

        #region Public methods
        public override byte GetBusModuleType(ushort busModuleAddress)
        {
            byte type = 255; // initially set "unknown" type

            I2CDevice.Configuration config = new I2CDevice.Configuration(busModuleAddress, BusConfiguration.ClockRate);
            if (!busConfig.Bus.TryGetRegister(config, BusConfiguration.Timeout, BusModule.CmdGetType, out type))
                type = 255; // set "unknown" type

            return type;
        }
        public override void GetBusModuleControlLines(BusModule busModule)
        {
            for (byte i = 0; i < BusModule.ControlLineTypesToRequest; i++)
            {
                byte[] result = new byte[1];
                I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
                if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineCount, new byte[] { i }, result))
                    result[0] = 0;

                for (byte number = 0; number < result[0]; number++)
                    busModule.ControlLines.Add(new ControlLine(Address, busModule.Address, (ControlLineType)i, number));
            }
        }
        public override void GetControlLineState(ControlLine controlLine)
        {
            byte[] data = new byte[2] { (byte)controlLine.Type, controlLine.Number };

            I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModuleAddress, BusConfiguration.ClockRate);
            if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineState, data, controlLine.State))
                controlLine.ResetState();
        }
        #endregion

        #region Private methods
        protected override void Scan()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            // get all addresses on bus:
            ArrayList onlineAddresses = busConfig.Bus.Scan(1, 127, BusConfiguration.ClockRate, BusConfiguration.Timeout);

            //// remove nonexisting modules:
            //foreach (BusModule busModule in BusModules)
            //    if (!onlineAddresses.Contains(busModule.Address))
            //    {
            //        addressesRemoved.Add(busModule.Address);
            //        BusModules.Remove(busModule);
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



            // for test!!!
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
        #endregion
    }
}
