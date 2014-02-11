using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;
using System.Threading;

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
        protected override void ScanBusModules()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            // get all addresses on bus:
            ArrayList onlineAddresses = busConfig.Bus.Scan(1, 127, BusConfiguration.ClockRate, BusConfiguration.Timeout);

            // remove nonexisting modules:
            foreach (BusModule busModule in BusModules)
                if (!onlineAddresses.Contains(busModule.Address))
                {
                    addressesRemoved.Add(busModule.Address);
                    BusModules.Remove(busModule);
                }
            
            // add new modules:
            foreach (ushort address in onlineAddresses)
                if (this[address] == null) // no registered module with this address
                {
                    byte type = GetBusModuleType(address);
                    BusModule busModule = new BusModule(address, type);
                    GetBusModuleControlLines(busModule);

                    addressesAdded.Add(address);
                    BusModules.Add(busModule);
                }

            NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }

        private byte GetBusModuleType(ushort busModuleAddress)
        {
            byte type = 0;

            I2CDevice.Configuration config = new I2CDevice.Configuration(busModuleAddress, BusConfiguration.ClockRate);
            if (!busConfig.Bus.TryGetRegister(config, BusConfiguration.Timeout, BusModule.CmdGetType, out type))
                type = 255; // set "unknown" type

            return type;
        }
        private void GetBusModuleControlLines(BusModule busModule)
        {
            for (byte i = 0; i < BusModule.MaxControlLineTypes; i++)
            {
                byte[] result = new byte[1];
                I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
                if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineCount, i, result))
                    result[0] = 0;

                for (int number = 0; number < result[0]; number++)
                    busModule.ControlLines.Add(new ControlLine(0, busModule.Address, (ControlLineType)i, number));
            }
        }
        #endregion
    }
}


//var socket = Socket.GetSocket(12, true, null, null);
//var i2c = new I2CBus(socket, 1, busClockRate, null);

//byte[] b = new byte[5];
//int i = i2c.Read(b, 1000);
//Debug.Print(b[0].ToString() + i);

//byte[] b = new byte[1];
//int i = i2c.WriteRead(new byte[] {0x00}, b, 1000);
//Debug.Print(b[0].ToString() + i);
//- See more at: https://www.ghielectronics.com/community/forum/topic?id=13503&page=2#msg137894
