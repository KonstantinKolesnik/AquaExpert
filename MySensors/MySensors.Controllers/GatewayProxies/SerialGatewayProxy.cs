using MySensors.Core;
using System;
using System.IO;
using System.IO.Ports;

namespace MySensors.Controllers.GatewayProxies
{
    public class SerialGatewayProxy : IGatewayProxy
    {
        #region Fields
        private SerialPort serialPort;
        #endregion

        #region Properties
        public bool IsStarted
        {
            get { return serialPort.IsOpen; }
        }
        #endregion

        #region Events
        public event SensorMessageEventHandler MessageReceived;
        #endregion

        #region Constructor
        public SerialGatewayProxy()
        {
            serialPort = new SerialPort();
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

        #region Public methods
        public void Connect()
        {
            if (serialPort.IsOpen)
                return;

            foreach (string portName in SerialPort.GetPortNames())
            {
                serialPort.PortName = portName;

                try
                {
                    serialPort.Open();

                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            string str = serialPort.ReadLine();
                            SensorMessage msg = SensorMessage.FromRawMessage(str);
                            if (msg != null && msg.Type == SensorMessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
                            {
                                //if (MessageReceived != null)
                                //    MessageReceived(this, msg);

                                return;
                            }
                        }
                        catch (TimeoutException) { }

                        Disconnect();
                    }
                }
                catch (Exception) {}
            }
        }
        public void Disconnect()
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        public void Send(SensorMessage message)
        {
            if (serialPort.IsOpen && message != null)
                serialPort.WriteLine(message.ToRawMessage());
        }
        #endregion

        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = serialPort.ReadLine();
                SensorMessage msg = SensorMessage.FromRawMessage(str);

                if (msg != null && MessageReceived != null)
                    MessageReceived(this, new SensorMessageEventArgs(msg));
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
