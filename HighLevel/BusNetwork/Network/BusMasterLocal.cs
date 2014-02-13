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
            for (byte type = 0; type < BusModule.ControlLineTypesToRequest; type++)
            {
                byte[] result = new byte[1];
                I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
                if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineCount, new byte[] { type }, result))
                    result[0] = 0;

                //for (byte number = 0; number < result[0]; number++)
                //    busModule.ControlLines.Add(new ControlLine(Address, busModule.Address, (ControlLineType)type, number));

                for (byte number = 0; number < result[0]; number++)
                {
                    ControlLine controlLine = new ControlLine(Address, busModule.Address, (ControlLineType)type, number);
                    busModule.ControlLines.Add(controlLine);
                    GetControlLineState(controlLine);
                }

            }
        }
        public override void GetControlLineState(ControlLine controlLine)
        {
            byte[] data = new byte[2] { (byte)controlLine.Type, controlLine.Number };

            I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModuleAddress, BusConfiguration.ClockRate);
            if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineState, data, controlLine.State))
                controlLine.ResetState();
        }
        public void SetControlLineState(ControlLine controlLine, byte[] state)
        {
            byte[] data = new byte[state.Length + 2];
            data[0] = (byte)controlLine.Type;
            data[1] = controlLine.Number;
            for (int i = 0; i < state.Length; i++)
                data[i + 2] = state[i];

            I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModuleAddress, BusConfiguration.ClockRate);
            if (busConfig.Bus.TrySetRegister(config, BusConfiguration.Timeout, BusModule.CmdSetControlLineState, data))
            {
            }
        }

        #endregion

        #region Private methods
        bool on = false;
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

                on = !on;
                SetControlLineState((ControlLine)busModule.ControlLines[3], new byte[] { (byte)(on ? 1 : 0), 0 });
                GetBusModuleControlLines(busModule);
            }



            NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }
        #endregion
    }
}
