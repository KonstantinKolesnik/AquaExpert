using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.KKS;
using AquaExpert.UI;
using System.Threading;
using Gadgeteer.Modules.LoveElectronics;

namespace AquaExpert
{
    class RFManager
    {
        private NRF24 rfMaster;
        private NRF24 rfSlave;
        private byte[] address = new byte[5] { 0, 1, 2, 3, 4 };
        private byte[] address2 = new byte[5] { 99, 88, 77, 66, 55 };
        //byte msg = 77;
        byte msgSlave = 77;

        public RFManager()
        {
            rfMaster = new NRF24(11);
            rfMaster.Tag = "Master";
            rfMaster.DataReceived += rfMaster_DataReceived;
            rfMaster.TransmitFailed += rfMaster_TransmitFailed;
            rfMaster.TransmitSuccess += rfMaster_TransmitSuccess;
            //PrintInfo(rfMaster);

            rfSlave = new NRF24(1);
            rfSlave.Tag = "Slave";
            rfSlave.DataReceived += rfSlave_DataReceived;
            rfSlave.TransmitFailed += rfSlave_TransmitFailed;
            rfSlave.TransmitSuccess += rfSlave_TransmitSuccess;
            //PrintInfo(rfSlave);


            //byte[] channels = rfMaster.ScanChannels();
            //for (int i = 0; i < channels.Length; i++)
            //    Debug.Print("Challel # " + i + ": " + channels[i]);

            //rfMaster.EnableAckPayload();
            //rfMaster.OpenReadingPipe(1, address);
            //rfMaster.OpenWritingPipe(address2);
            rfMaster.Configure(address);

            //rfSlave.EnableAckPayload();
            //rfSlave.OpenReadingPipe(1, address2);
            //rfSlave.OpenWritingPipe(address);
            //rfSlave.StartListening();
            rfSlave.Configure(address2);

            //rfSlave.WriteAckPayload(1, new byte[] { msgSlave });
            //UIManager.DebugPage.AddLine("Write ack: " + msgSlave);

            //rfMaster.StartWrite(new byte[] { 150 });
            //rfMaster.StartListening();
            //UIManager.DebugPage.AddLine("Send: " + 0);

            Program.Button.ButtonReleased += delegate(Button sender, Button.ButtonState state)
            {
                UIManager.DebugPage.Clear();
                UIManager.DebugPage.AddLine("Send: " + 0);
                rfMaster.SendTo(address2, new byte[] { 0 });
            };

            Thread.Sleep(100);
            //rfMaster.SendTo(address2, new byte[] { 0 });

            //nrf24.SendTo(nrf24.RX_Adress, Encoding.UTF8.GetBytes("Test"));
            //new string(Encoding.UTF8.GetChars())


            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);
            //        rfMaster.StartWrite(new byte[] { 130 });
            //        //UIManager.DebugPage.AddLine("Send: " + 0);
            //    }
            //}).Start();
        }

        void rfMaster_TransmitSuccess(object sender, EventArgs e)
        {
            //rfMaster.StartWrite(new byte[] { 0 });
        }
        void rfMaster_TransmitFailed(object sender, EventArgs e)
        {
            //rfMaster.StartWrite(new byte[] { msg });
            //UIManager.DebugPage.Text = "TransmitFailed";
            //rfMaster.StartWrite(new byte[] { 0 });
            UIManager.DebugPage.AddLine("TransmitFailed");
        }
        void rfMaster_DataReceived(byte[] data)
        {
            //rfMaster.StopListening();

            byte msg = data[0];
            UIManager.DebugPage.AddLine("Send: " + msg);
            Thread.Sleep(500);
            rfMaster.SendTo(address2, new byte[] { msg });
            //UIManager.DebugPage.Text = msg.ToString();

            //if (msg == 255)
            //    msg = 0;
            //else
            //    msg++;

            //rfMaster.StartWrite(new byte[] { msg });

            //rfMaster.StartListening();

        }


        void rfSlave_TransmitSuccess(object sender, EventArgs e)
        {
        }
        void rfSlave_TransmitFailed(object sender, EventArgs e)
        {
            //rfSlave.WriteAckPayload(1, new byte[] { msgSlave });
            UIManager.DebugPage.AddLine("ResponseFailed");
        }
        void rfSlave_DataReceived(byte[] data)
        {
            byte msg = data[0];

            if (msg == 255)
                msg = 0;
            else
                msg++;

            //rfSlave.WriteAckPayload(1, new byte[] { msgSlave });
            //UIManager.DebugPage.AddLine("Write ack: " + msgSlave);

            //rfSlave.StopListening();
            //rfSlave.StartWrite(new byte[] {msg});
            UIManager.DebugPage.AddLine("Send response: " + msg);
            //rfSlave.StartListening();

            Thread.Sleep(500);
            rfSlave.SendTo(address, new byte[] { msg });
        }

        private void PrintInfo(NRF24 rfModule)
        {
            //UIManager.DebugPage.AddLine("Init nRF24L01+");

            UIManager.DebugPage.AddLine("---------------");
            UIManager.DebugPage.AddLine("IsContinuousCarrierTransmitEnabled: " + rfModule.IsContinuousCarrierTransmitEnabled);
            UIManager.DebugPage.AddLine("IsPllLockEnabled: " + rfModule.IsPllLockEnabled);
            UIManager.DebugPage.AddLine("DataRate: " + rfModule.DataRate);
            UIManager.DebugPage.AddLine("TransmitPower: " + rfModule.TransmitPower);
            

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("Status:");
            //UIManager.DebugPage.AddLine(rfModule.Status.ToString());

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("CRCType: " + (rfModule.CRCLength == NRF24.CRCType.CRC1 ? "1 byte" : "2 bytes"));
            //UIManager.DebugPage.AddLine("IsCRCEnabled: " + rfModule.IsCRCEnabled);
            //UIManager.DebugPage.AddLine("IsDataReceivedInterruptEnabled: " + rfModule.IsDataReceivedInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsDataSentInterruptEnabled: " + rfModule.IsDataSentInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsResendLimitReachedInterruptEnabled: " + rfModule.IsResendLimitReachedInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsPowerOn: " + rfModule.IsPowerOn);
            //UIManager.DebugPage.AddLine("IsReceiver: " + rfModule.IsReceiver);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsAckEnabledP0: " + rfModule.IsAutoAckEnabled0);
            //UIManager.DebugPage.AddLine("IsAckEnabledP1: " + rfModule.IsAutoAckEnabled1);
            //UIManager.DebugPage.AddLine("IsAckEnabledP2: " + rfModule.IsAutoAckEnabled2);
            //UIManager.DebugPage.AddLine("IsAckEnabledP3: " + rfModule.IsAutoAckEnabled3);
            //UIManager.DebugPage.AddLine("IsAckEnabledP4: " + rfModule.IsAutoAckEnabled4);
            //UIManager.DebugPage.AddLine("IsAckEnabledP5: " + rfModule.IsAutoAckEnabled5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled0: " + rfModule.IsReceiverAddressEnabled0);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled1: " + rfModule.IsReceiverAddressEnabled1);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled2: " + rfModule.IsReceiverAddressEnabled2);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled3: " + rfModule.IsReceiverAddressEnabled3);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled4: " + rfModule.IsReceiverAddressEnabled4);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled5: " + rfModule.IsReceiverAddressEnabled5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("AddressType: " + rfModule.AddressType);
            //UIManager.DebugPage.AddLine("AutoRetransmitCount: " + rfModule.AutoRetransmitCount);
            //UIManager.DebugPage.AddLine("AutoRetransmitDelay: " + rfModule.AutoRetransmitDelay);
            //UIManager.DebugPage.AddLine("Channel: " + rfMaster.Channel);

            //UIManager.DebugPage.AddLine("RetransmittedPacketsCount: " + rfModule.RetransmittedPacketsCount);
            //UIManager.DebugPage.AddLine("LostPacketsCount: " + rfModule.LostPacketsCount);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("TransmitAddress: " +
            //    "[" + rfModule.TransmitterAddress[4] + "]" +
            //    "[" + rfModule.TransmitterAddress[3] + "]" +
            //    "[" + rfModule.TransmitterAddress[2] + "]" +
            //    "[" + rfModule.TransmitterAddress[1] + "]" +
            //    "[" + rfModule.TransmitterAddress[0] + "]"
            //    );
            //UIManager.DebugPage.AddLine("ReceiveAddress0: " +
            //    "[" + rfModule.ReceiverAddress0[4] + "]" +
            //    "[" + rfModule.ReceiverAddress0[3] + "]" +
            //    "[" + rfModule.ReceiverAddress0[2] + "]" +
            //    "[" + rfModule.ReceiverAddress0[1] + "]" +
            //    "[" + rfModule.ReceiverAddress0[0] + "]"
            //    );
            //UIManager.DebugPage.AddLine("ReceiveAddress1: " +
            //    "[" + rfModule.ReceiverAddress1[4] + "]" +
            //    "[" + rfModule.ReceiverAddress1[3] + "]" +
            //    "[" + rfModule.ReceiverAddress1[2] + "]" +
            //    "[" + rfModule.ReceiverAddress1[1] + "]" +
            //    "[" + rfModule.ReceiverAddress1[0] + "]"
            //    );

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth0: " + rfModule.ReceiverPayloadWidth0);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth1: " + rfModule.ReceiverPayloadWidth1);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth2: " + rfModule.ReceiverPayloadWidth2);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth3: " + rfModule.ReceiverPayloadWidth3);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth4: " + rfModule.ReceiverPayloadWidth4);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth5: " + rfModule.ReceiverPayloadWidth5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled0: " + rfModule.IsDynamicPayloadEnabled0);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled1: " + rfModule.IsDynamicPayloadEnabled1);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled2: " + rfModule.IsDynamicPayloadEnabled2);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled3: " + rfModule.IsDynamicPayloadEnabled3);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled4: " + rfModule.IsDynamicPayloadEnabled4);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled5: " + rfModule.IsDynamicPayloadEnabled5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled: " + rfModule.IsDynamicPayloadFeatureEnabled);
            //UIManager.DebugPage.AddLine("IsAckPayloadEnabled: " + rfModule.IsAckPayloadEnabled);
            //UIManager.DebugPage.AddLine("IsDynamicAckEnabled: " + rfModule.IsDynamicAckEnabled);
        }
    }
}
