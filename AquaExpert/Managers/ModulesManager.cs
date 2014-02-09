using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;
using GT = Gadgeteer;

namespace AquaExpert.Managers
{
    public class ModulesManager
    {
        #region Fields
        private Hashtable modules = new Hashtable();
        private GT.Timer timerUpdate;
        private const int updateInterval = 1000;
        #endregion

        #region Properties
        public Hashtable Modules
        {
            get { return modules; }
        }
        #endregion

        #region Events
        public event CollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Constructor
        public ModulesManager()
        {
            timerUpdate = new GT.Timer(updateInterval);
            timerUpdate.Tick += timerUpdate_Tick;
            timerUpdate.Start();
        }
        #endregion

        #region Private methods
        private void Scan()
        {
            ArrayList addressesAdded = new ArrayList();
            ArrayList addressesRemoved = new ArrayList();

            // get all addresses on bus:
            ArrayList activeAddresses = Program.Bus.Scan(1, 127, Program.BusClockRate, Program.BusTimeout);

            // remove nonexisting modules:
            foreach (ushort address in modules.Keys)
                if (!activeAddresses.Contains(address))
                {
                    modules.Remove(address);
                    addressesRemoved.Add(address);
                }
            
            // add only new modules:
            foreach (ushort address in activeAddresses)
                if (!modules.Contains(address))
                {
                    byte[] summary = GetModuleSummary(address);

                    Module module = new Module(address, summary[0])
                    {
                        RelayCount = summary[1],
                        WaterSensorCount = summary[2],
                        PhSensorCount = summary[3],
                        OrpSensorCount = summary[4],
                        TemperatureSensorCount = summary[5]
                    };
                    modules.Add(address, module);
                    addressesAdded.Add(address);
                }

            if ((addressesAdded.Count != 0 || addressesRemoved.Count != 0) && CollectionChanged != null)
                CollectionChanged(addressesAdded, addressesRemoved);
        }
        private byte[] GetModuleSummary(ushort address)
        {
            byte[] response = new byte[6];
            response[0] = 255; // set unknown type

            I2CDevice.Configuration config = new I2CDevice.Configuration(address, Program.BusClockRate);
            if (Program.Bus.TryGetRegisters(config, Program.BusTimeout, Module.CMD_GET_SUMMARY, response))
            {
            }
            
            return response;
        }
        #endregion

        #region Event handlers
        private void timerUpdate_Tick(GT.Timer timer)
        {
            timerUpdate.Stop();
            Scan();
            timerUpdate.Start();
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
