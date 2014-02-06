using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Collections;
using MFE.Hardware;

namespace AquaExpert
{
    static class ModulesManager
    {
        private static I2CDevice bus = new I2CDevice(null);
        private const int busClockRate = 400; // 400 kHz
        private static ArrayList modules = new ArrayList();




        public static void Scan()
        {
            modules.Clear();

            ArrayList res = bus.Scan(1, 127, busClockRate);
            foreach (int address in res)
            {
                Module module = new Module(address);
                //byte r;
                //if (bus.TryGetRegister(new I2CDevice.Configuration((ushort)address, busClockRate), 1000, 8, out r))
                //{
                //    byte bbb = r;
                //}


                modules.Add(module);
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
        }




    }
}
