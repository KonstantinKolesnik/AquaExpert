using MySensors.Core.Sensors;
using System;
using System.IO.Ports;

namespace MySensors.Core.Services.Connectors
{
    public class SerialGatewayConnector : IGatewayConnector
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
        public event MessageEventHandler MessageReceived;
        #endregion

        #region Constructor
        public SerialGatewayConnector()
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
                            Message msg = Message.FromRawString(str);
                            if (msg != null && msg.Type == MessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
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

        public void Send(Message message)
        {
            if (serialPort.IsOpen && message != null)
                serialPort.WriteLine(message.ToRawString());
        }
        #endregion

        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = serialPort.ReadLine();
                Message msg = Message.FromRawString(str);

                if (msg != null && MessageReceived != null)
                    MessageReceived(this, msg);
            }
            catch (TimeoutException) { }
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
