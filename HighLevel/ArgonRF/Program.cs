
using Gadgeteer.Modules.Gralin;
using System.Threading;
using Gadgeteer.Modules.LoveElectronics;

namespace ArgonRF
{
    public partial class Program
    {
        private Nordic nrf2;
        private byte msg2 = 0;
        private byte[] address1 = new byte[5] { 11, 22, 33, 44, 55 };
        private byte[] address2 = new byte[5] { 99, 88, 77, 66, 55 };
        int pause = 1000;

        void ProgramStarted()
        {
            nrf2 = new Nordic(5);
            nrf2.DataReceived += nrf2_DataReceived;
            nrf2.TransmitSuccess += nrf2_TransmitSuccess;
            nrf2.TransmitFailed += nrf2_TransmitFailed;
            nrf2.Configure(address2, 2);
            nrf2.Enable();

            Mainboard.SetDebugLED(true);
            ledArray.Clear();
        }

        void nrf2_TransmitSuccess()
        {
            ledArray[0] = false;
            ledArray[6] = false;
        }
        void nrf2_TransmitFailed()
        {
            //Thread.Sleep(pause);
            ledArray[6] = true;
            nrf2.SendTo(address1, new[] { msg2 });
        }
        void nrf2_DataReceived(byte[] data)
        {
            msg2 = data[0];
            if (msg2 == 255)
                msg2 = 0;
            else
                msg2++;

            Thread.Sleep(pause);
            ledArray[0] = true;
            nrf2.SendTo(address1, new [] { msg2 });
        }
    }
}
