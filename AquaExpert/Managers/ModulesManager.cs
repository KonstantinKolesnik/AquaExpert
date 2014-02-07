using MFE.Hardware;
using Microsoft.SPOT.Hardware;
using System.Collections;

namespace AquaExpert.Managers
{
    static class ModulesManager
    {
        private static I2CDevice bus = new I2CDevice(null);
        private const int busClockRate = 400; // 400 kHz
        private const int timeout = 1000; // 1 sec
        private static Hashtable modules = new Hashtable();

        public static void Scan()
        {
            // get all addresses on bus:
            ArrayList activeAddresses = bus.Scan(1, 127, busClockRate, timeout);

            // remove nonexisting modules:
            foreach (ushort address in modules.Keys)
                if (!activeAddresses.Contains(address))
                    modules.Remove(address);
            
            // add only new modules:
            foreach (ushort address in activeAddresses)
            {
                Module module;

                if (!modules.Contains(address))
                {
                    module = new Module(address);
                    modules.Add(address, module);
                }
                else
                    module = (Module)modules[address];

                GetModuleProperties(module);
            }
        }

        private static void GetModuleProperties(Module module)
        {
            //I2CDevice.Configuration config = new I2CDevice.Configuration(module.Address, busClockRate);
            //byte register = 8;
            //byte value;
            
            //if (bus.TryGetRegister(config, timeout, register, out value))
            //{
            //    byte bbb = value;




            //}
        }
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
