using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StreamClient
    {
        #region Fields
        private StreamSocket client;
        private string hostName;
        private string serviceName;
        #endregion

        #region Public methods
        public async Task Start(string hostName, string serviceName)
        {
            if (client == null)
            {
                this.hostName = hostName;
                this.serviceName = serviceName;

                client = new StreamSocket();
                client.Control.KeepAlive = false;

                try
                {
                    var host = new HostName(hostName);
                    await client.ConnectAsync(host, serviceName);
                }
                catch (Exception exception)
                {
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                }
            }
        }
        public async Task Send(string commandName, params object[] parameters)
        {
            if (client != null)
            {
                var writer = new DataWriter(client.OutputStream);

                // Write first the length of the string as UINT32 value followed up by the string. 
                // Writing data to the writer will just store data in memory.
                string stringToSend = "Hello";
                writer.WriteUInt32(writer.MeasureString(stringToSend));
                writer.WriteString(stringToSend);

                // Write the locally buffered data to the network.
                try
                {
                    await writer.StoreAsync();
                }
                catch (Exception exception)
                {
                    // If this is an unknown status it means that the error if fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    {
                        throw;
                    }
                }

                writer.DetachStream();
                writer.Dispose();
            }
        }
        public void Stop()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }
        #endregion
    }
}
