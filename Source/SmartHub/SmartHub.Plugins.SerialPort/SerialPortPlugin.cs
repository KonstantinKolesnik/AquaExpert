using SmartHub.Core.Plugins;
using System;
using System.IO;
using System.IO.Ports;

namespace SmartHub.Plugins.SerialPort
{
    [Plugin]
    public class SerialPortPlugin : PluginBase
    {
        #region Fields
        private System.IO.Ports.SerialPort serialPort;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return serialPort.IsOpen; }
        }
        #endregion

        #region Constructor
        public SerialPortPlugin()
        {
            serialPort = new System.IO.Ports.SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DtrEnable = true;
            serialPort.NewLine = "\n";
            serialPort.ReadTimeout = 4000;
            serialPort.WriteTimeout = 4000;

            serialPort.DataReceived += serialPort_DataReceived;
            serialPort.ErrorReceived += serialPort_ErrorReceived;
            serialPort.PinChanged += serialPort_PinChanged;
        }
        #endregion



        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //int dataLength = serialPort.BytesToRead;
                //byte[] data = new byte[dataLength];
                //int nbrDataRead = serialPort.Read(data, 0, dataLength);
                //if (nbrDataRead == 0)
                //    return;

                //string str = null;
                //while (!string.IsNullOrEmpty(str = serialPort.ReadLine()))
                //{
                //    SensorMessage msg = SensorMessage.FromRawMessage(str);

                //    if (msg != null && MessageReceived != null)
                //        MessageReceived(this, new SensorMessageEventArgs(msg));
                //}
            }
            catch (TimeoutException) { }
            catch (IOException) { }
        }
        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            int a = 0;
            int b = a;
        }
        private void serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            int a = 0;
            int b = a;
        }
        #endregion

    }
}
