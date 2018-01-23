using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Udp
{
    public class UdpClient
    {
        #region Fields
        private DatagramSocket socket = null;
        #endregion

        #region Events
        public event UdpMessageEventHandler MessageReceived;
        #endregion

        #region Public methods
        public async Task Start(string localServiceName, string remoteMulticastAddress = null)
        {
            if (socket == null)
            {
                socket = new DatagramSocket();
                socket.Control.DontFragment = true;
                socket.Control.MulticastOnly = true; // true - use shared port
                socket.MessageReceived += DataReceived;

                //try
                {
                    await socket.BindServiceNameAsync(localServiceName);

                    if (!string.IsNullOrEmpty(remoteMulticastAddress))
                        socket.JoinMulticastGroup(new HostName(remoteMulticastAddress));
                }
                //catch (Exception exception)
                //{
                //    await Stop();

                //    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                //    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                //        throw;
                //}
            }
        }
        public async Task Send(string remoteHostAddress, string remoteServiceName, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    // GetOutputStreamAsync can be called multiple times on a single DatagramSocket instance to obtain
                    // IOutputStreams pointing to various different remote endpoints. The remote hostname given to
                    // GetOutputStreamAsync can be a unicast, multicast or broadcast address.
                    var outputStream = await socket.GetOutputStreamAsync(new HostName(remoteHostAddress), remoteServiceName);

                    using (var writer = new DataWriter(outputStream))
                    {
                        writer.WriteString(data);
                        await writer.StoreAsync();
                        await writer.FlushAsync();
                        writer.DetachStream();
                    }
                }
                catch (Exception exception)
                {
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                        throw;
                }
            }
        }
        public async Task Stop()
        {
            if (socket != null)
            {
                await socket.CancelIOAsync();
                socket.Dispose();
                socket = null;
            }
        }
        #endregion

        #region Private methods
        private void DataReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                var length = args.GetDataReader().UnconsumedBufferLength;
                var data = args.GetDataReader().ReadString(length);

                //NotifyUserFromAsyncThread("Received data from remote peer (Remote Address: " + args.RemoteAddress.CanonicalName + ", Remote Port: " + args.RemotePort + "): \"" + str + "\"", NotifyType.StatusMessage);

                MessageReceived?.Invoke(this, new UdpMessageEventArgs(args.RemoteAddress, data));
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;
            }
        }
        #endregion
    }
}
