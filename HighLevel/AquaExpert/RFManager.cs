using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.KKS;
using AquaExpert.UI;
using System.Threading;

namespace AquaExpert
{
    class RFManager
    {
        private NRF24 rfMaster;
        private NRF24 rfSlave;
        private byte[] address = new byte[5] { 0, 1, 2, 3, 4 };
        byte msg = 111;
        byte msgSlave = 0;

        public RFManager()
        {
            rfMaster = new NRF24(11);
            rfMaster.DataReceived += rfMaster_DataReceived;
            rfMaster.TransmitFailed += rfMaster_TransmitFailed;
            rfMaster.TransmitSuccess += rfMaster_TransmitSuccess;

            rfSlave = new NRF24(1);
            rfSlave.DataReceived += rfSlave_DataReceived;
            rfSlave.TransmitFailed += rfSlave_TransmitFailed;
            rfSlave.TransmitSuccess += rfSlave_TransmitSuccess;

            //byte[] channels = rfMaster.ScanChannels();
            //for (int i = 0; i < channels.Length; i++)
            //    Debug.Print("Challel # " + i + ": " + channels[i]);

            rfMaster.EnableAckPayload();
            rfMaster.OpenWritingPipe(address);

            rfSlave.EnableAckPayload();
            rfSlave.OpenReadingPipe(1, address);
            rfSlave.StartListening();

            rfMaster.StartWrite(new byte[] { msg });

            //nrf24.SendTo(nrf24.RX_Adress, Encoding.UTF8.GetBytes("Test"));
            //new string(Encoding.UTF8.GetChars())
        }

        void rfMaster_TransmitSuccess(object sender, EventArgs e)
        {
        }
        void rfMaster_TransmitFailed(object sender, EventArgs e)
        {
            //rfMaster.StartWrite(new byte[] { msg });
        }
        void rfMaster_DataReceived(byte[] data)
        {
            // received an ack payload

            msg = data[0];
            UIManager.DebugPage.Text = msg.ToString();
            Thread.Sleep(100);
            rfMaster.StartWrite(new byte[] { msg });
        }


        void rfSlave_TransmitSuccess(object sender, EventArgs e)
        {
            // Ack Payload:Sent
            int a = 0;
            int b = a;
        }
        void rfSlave_TransmitFailed(object sender, EventArgs e)
        {
            // Ack Payload:Failed
            //rfSlave.WriteAckPayload(1, new byte[] { msgSlave });
        }
        void rfSlave_DataReceived(byte[] data)
        {
            // received a time message

            msgSlave = data[0];
            if (msgSlave == 255)
                msgSlave = 0;
            else
                msgSlave++;

            rfSlave.WriteAckPayload(1, new byte[] { msgSlave });
        }




        private void PrintInfo()
        {
            UIManager.DebugPage.AddLine("Init nRF24L01+");

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("Status:");
            //UIManager.DebugPage.AddLine(rfMaster.Status.ToString());

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("CRCType: " + (rfMaster.CRCType == NRF24.CRCLength.CRC1 ? "1 byte" : "2 bytes"));
            //UIManager.DebugPage.AddLine("IsCRCEnabled: " + rfMaster.IsCRCEnabled);
            //UIManager.DebugPage.AddLine("IsDataReceivedInterruptEnabled: " + rfMaster.IsDataReceivedInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsDataSentInterruptEnabled: " + rfMaster.IsDataSentInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsResendLimitReachedInterruptEnabled: " + rfMaster.IsResendLimitReachedInterruptEnabled);
            //UIManager.DebugPage.AddLine("IsPowerOn: " + rfMaster.IsPowerOn);
            //UIManager.DebugPage.AddLine("IsReceiver: " + rfMaster.IsReceiver);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsAckEnabledP0: " + nrf24.IsAckEnabledP0);
            //UIManager.DebugPage.AddLine("IsAckEnabledP1: " + nrf24.IsAckEnabledP1);
            //UIManager.DebugPage.AddLine("IsAckEnabledP2: " + nrf24.IsAckEnabledP2);
            //UIManager.DebugPage.AddLine("IsAckEnabledP3: " + nrf24.IsAckEnabledP3);
            //UIManager.DebugPage.AddLine("IsAckEnabledP4: " + nrf24.IsAckEnabledP4);
            //UIManager.DebugPage.AddLine("IsAckEnabledP5: " + nrf24.IsAckEnabledP5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled0: " + nrf24.IsReceiverAddressEnabled0);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled1: " + nrf24.IsReceiverAddressEnabled1);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled2: " + nrf24.IsReceiverAddressEnabled2);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled3: " + nrf24.IsReceiverAddressEnabled3);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled4: " + nrf24.IsReceiverAddressEnabled4);
            //UIManager.DebugPage.AddLine("IsReceiverAddressEnabled5: " + nrf24.IsReceiverAddressEnabled5);

            UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("AddressType: " + rfMaster.AddressType);
            //UIManager.DebugPage.AddLine("AutoRetransmitCount: " + rfMaster.AutoRetransmitCount);
            //UIManager.DebugPage.AddLine("AutoRetransmitDelay: " + rfMaster.AutoRetransmitDelay);
            UIManager.DebugPage.AddLine("Channel: " + rfMaster.Channel);

            //UIManager.DebugPage.AddLine("RetransmittedPacketsCount: " + rfMaster.RetransmittedPacketsCount);
            //UIManager.DebugPage.AddLine("LostPacketsCount: " + rfMaster.LostPacketsCount);

            UIManager.DebugPage.AddLine("---------------");
            UIManager.DebugPage.AddLine("TransmitAddress: " +
                "[" + rfMaster.TransmitAddress[4] + "]" +
                "[" + rfMaster.TransmitAddress[3] + "]" +
                "[" + rfMaster.TransmitAddress[2] + "]" +
                "[" + rfMaster.TransmitAddress[1] + "]" +
                "[" + rfMaster.TransmitAddress[0] + "]"
                );
            UIManager.DebugPage.AddLine("ReceiveAddress0: " +
                "[" + rfMaster.ReceiveAddress0[4] + "]" +
                "[" + rfMaster.ReceiveAddress0[3] + "]" +
                "[" + rfMaster.ReceiveAddress0[2] + "]" +
                "[" + rfMaster.ReceiveAddress0[1] + "]" +
                "[" + rfMaster.ReceiveAddress0[0] + "]"
                );
            UIManager.DebugPage.AddLine("ReceiveAddress1: " +
                "[" + rfMaster.ReceiveAddress1[4] + "]" +
                "[" + rfMaster.ReceiveAddress1[3] + "]" +
                "[" + rfMaster.ReceiveAddress1[2] + "]" +
                "[" + rfMaster.ReceiveAddress1[1] + "]" +
                "[" + rfMaster.ReceiveAddress1[0] + "]"
                );


            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth0: " + nrf24.ReceiverPayloadWidth0);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth1: " + nrf24.ReceiverPayloadWidth1);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth2: " + nrf24.ReceiverPayloadWidth2);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth3: " + nrf24.ReceiverPayloadWidth3);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth4: " + nrf24.ReceiverPayloadWidth4);
            //UIManager.DebugPage.AddLine("ReceiverPayloadWidth5: " + nrf24.ReceiverPayloadWidth5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled0: " + nrf24.IsDynamicPayloadEnabled0);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled1: " + nrf24.IsDynamicPayloadEnabled1);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled2: " + nrf24.IsDynamicPayloadEnabled2);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled3: " + nrf24.IsDynamicPayloadEnabled3);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled4: " + nrf24.IsDynamicPayloadEnabled4);
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled5: " + nrf24.IsDynamicPayloadEnabled5);

            //UIManager.DebugPage.AddLine("---------------");
            //UIManager.DebugPage.AddLine("IsDynamicPayloadEnabled: " + rfMaster.IsDynamicPayloadEnabled);
            //UIManager.DebugPage.AddLine("IsAckPayloadEnabled: " + rfMaster.IsAckPayloadEnabled);
            //UIManager.DebugPage.AddLine("IsDynamicAckEnabled: " + rfMaster.IsDynamicAckEnabled);
        }
    }
}
