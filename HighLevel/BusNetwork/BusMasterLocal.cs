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
            foreach (BusModule busModule in busModules)
                if (!onlineAddresses.Contains(busModule.Address))
                {
                    addressesRemoved.Add(busModule.Address);
                    busModules.Remove(busModule);
                }
            
            // add new modules:
            foreach (ushort address in onlineAddresses)
                if (this[address] == null) // no registered module with this address
                {
                    byte type = GetBusModuleType(address);
                    BusModule busModule = new BusModule(address, type);
                    //GetBusModuleSummary(busModule);

                    addressesAdded.Add(address);
                    busModules.Add(busModule);
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
        private void GetBusModuleSummary(BusModule busModule)
        {

            //byte[] summary = GetBusModuleSummary(module.Address);

            //module.ControlLineTypesCounts.Add(ControlLineType.Relay, summary[1]);
            //module.ControlLineTypesCounts.Add(ControlLineType.WaterSensor, summary[2]);
            //module.ControlLineTypesCounts.Add(ControlLineType.PHSensor, summary[3]);
            //module.ControlLineTypesCounts.Add(ControlLineType.ORPSensor, summary[4]);
            //module.ControlLineTypesCounts.Add(ControlLineType.ConductivitySensor, summary[5]);
            //module.ControlLineTypesCounts.Add(ControlLineType.TemperatureSensor, summary[6]);
            //module.ControlLineTypesCounts.Add(ControlLineType.Dimmer, summary[7]);



            //I2CDevice.Configuration config = new I2CDevice.Configuration(busModuleAddress, BusConfiguration.ClockRate);
            //if (busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineCount, response))
            //{
            //}
            
            //return response;
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
