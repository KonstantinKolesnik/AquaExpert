using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System;
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
        public override bool BusModuleWriteRead(BusModule busModule, byte[] request, byte[] response)
        {
            I2CDevice.Configuration config = new I2CDevice.Configuration(busModule.Address, BusConfiguration.ClockRate);
            return busConfig.Bus.TryGet(config, BusConfiguration.Timeout, request, response);
        }




        //public override void GetControlLineState(ControlLine controlLine)
        //{
        //    byte[] data = new byte[2] { (byte)controlLine.Type, controlLine.Number };

        //    I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModule.Address, BusConfiguration.ClockRate);
        //    if (!busConfig.Bus.TryGetRegisters(config, BusConfiguration.Timeout, BusModule.CmdGetControlLineState, data, controlLine.State))
        //        controlLine.ResetState();
        //}
        //public override void SetControlLineState(ControlLine controlLine, byte[] state)
        //{
        //    byte[] data = new byte[state.Length + 2];
        //    data[0] = (byte)controlLine.Type;
        //    data[1] = controlLine.Number;
        //    Array.Copy(state, 0, data, 2, state.Length);

        //    I2CDevice.Configuration config = new I2CDevice.Configuration(controlLine.BusModule.Address, BusConfiguration.ClockRate);
        //    if (!busConfig.Bus.TrySetRegister(config, BusConfiguration.Timeout, BusModule.CmdSetControlLineState, data))
        //    {
        //    }
        //}
        #endregion

        #region Private methods
        //bool on = false;
        protected override void Scan()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            for (ushort address = 1; address <= 127; address++)
            {
                byte type = 255;

                I2CDevice.Configuration config = new I2CDevice.Configuration(address, BusConfiguration.ClockRate);
                if (busConfig.Bus.TryGetRegister(config, BusConfiguration.Timeout, BusModule.CmdGetType, out type))
                {
                    // address is online

                    BusModule busModule = this[address];

                    if (busModule == null) // no registered module with this address
                    {
                        busModule = new BusModule(this, address, type);

                        // query control lines count:
                        busModule.RequestControlLines();

                        addressesAdded.Add(address);
                        BusModules.Add(busModule);

                        //// for test!!!
                        //on = !on;
                        //SetControlLineState((ControlLine)busModule.ControlLines[3], new byte[] { (byte)(on ? 1 : 0), 0 });
                        //GetBusModuleControlLines(busModule);
                    }
                    else // module with this address is registered
                    {
                        // updated whn added;
                        // update again???????
                    }
                }
                else
                {
                    // address is offline
                    
                    BusModule busModule = this[address];
                    if (busModule != null) // offline module
                    {
                        addressesRemoved.Add(address);
                        BusModules.Remove(busModule);
                    }
                }
            }

            NotifyBusModulesCollectionChanged(addressesAdded, addressesRemoved);
        }
        #endregion
    }
}
