using SmartHub.Plugins.MySensors.Core;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SmartHub.Plugins.MySensors.GatewayProxies
{
    class SerialGatewayProxy : IGatewayProxy
    {
        #region Fields
        private SerialPort serialPort;
        private bool isStopped = true;
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
            //serialPort.RtsEnable = true;
            serialPort.Encoding = Encoding.ASCII;
            serialPort.NewLine = "\n";
            //serialPort.ReadTimeout = 4000;
            //serialPort.WriteTimeout = 4000;

            serialPort.DataReceived += serialPort_DataReceived;
        }
        #endregion

        #region Public methods
        public void Start()
        {
            isStopped = false;

            new Thread(() =>
            {
                while (!isStopped && !FindAndConnect()) ;
            }).Start();
        }
        public void Stop()
        {
            isStopped = true;

            if (IsStarted)
                serialPort.Close();
        }

        public void Send(SensorMessage message)
        {
            if (IsStarted && message != null)
            {
                try
                {
                    serialPort.WriteLine(message.ToRawMessage());
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region Event handlers
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str = null;
                while (IsStarted && !string.IsNullOrEmpty(str = serialPort.ReadLine()))
                {
                    SensorMessage msg = SensorMessage.FromRawMessage(str);
                    if (msg != null && MessageReceived != null)
                        MessageReceived(this, new SensorMessageEventArgs(msg));
                }
            }
            catch (TimeoutException) { }
            catch (IOException) { }
            catch (Exception) { }
        }
        #endregion

        #region Private methods
        private bool FindAndConnect()
        {
            if (!IsStarted)
            {
                var names = SerialPort.GetPortNames();

                foreach (string portName in names)
                {
                    serialPort.PortName = portName;

                    try
                    {
                        serialPort.Open();
                        if (IsStarted)
                        {
                            serialPort.DiscardInBuffer();

                            try
                            {
                                string str = serialPort.ReadLine();
                                SensorMessage msg = SensorMessage.FromRawMessage(str);
                                if (msg != null && msg.Type == SensorMessageType.Internal && (InternalValueType)msg.SubType == InternalValueType.GatewayReady)
                                {
                                    if (MessageReceived != null)
                                        MessageReceived(this, new SensorMessageEventArgs(msg));

                                    return true;
                                }
                            }
                            catch (TimeoutException) { }

                            Stop();
                        }
                    }
                    catch (Exception)
                    {
                        Stop();
                    }
                }

                return false;
            }
            else
                return true;
        }
        #endregion
    }
}
