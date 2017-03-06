using SmartHub.UWP.Core.Communication.Transporting;
using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StreamServer
    {
        #region Fields
        private StreamSocketListener listener;
        private string serviceName;
        //private List<> clients
        #endregion

        #region Properties
        public CommandProcessor CommandProcessor
        {
            get; set;
        }
        #endregion

        #region Public methods
        public async Task Start(string serviceName)
        {
            if (listener == null)
            {
                this.serviceName = serviceName;

                listener = new StreamSocketListener();
                listener.ConnectionReceived += Listener_ConnectionReceived;

                // If necessary, tweak the listener's control options before carrying out the bind operation.
                // These options will be automatically applied to the connected StreamSockets resulting from
                // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
                // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
                listener.Control.KeepAlive = false;

                await listener.BindServiceNameAsync(serviceName);
            }
        }
        public async Task Stop()
        {
            if (listener != null)
            {
                await listener.CancelIOAsync();
                listener.Dispose();
                listener = null;
            }
        }
        #endregion

        #region Private methods
        private static async Task<string> Receive(StreamSocket socket)
        {
            string result = null;

            if (socket != null)
                using (var reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    uint sizeFieldLength = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldLength == sizeof(uint))
                    {
                        uint dataLength = reader.ReadUInt32();
                        uint actualDataLength = await reader.LoadAsync(dataLength);
                        if (dataLength == actualDataLength)
                            result = reader.ReadString(actualDataLength);
                    }

                    reader.DetachStream();
                }

            return result;
        }
        private static async Task Send(StreamSocket socket, object data)
        {
            if (socket != null)
            {
                using (var writer = new DataWriter(socket.OutputStream))
                {
                    var str = Transport.Serialize(data);
                    writer.WriteUInt32(writer.MeasureString(str));
                    writer.WriteString(str);

                    try
                    {
                        await writer.StoreAsync();
                        //await writer.FlushAsync();
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
                }

                await socket.CancelIOAsync();
                socket.Dispose();
            }
        }
        #endregion

        #region Event handlers
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                string str = await Receive(args.Socket);

                var request = Transport.Deserialize<CommandRequest>(str);
                var result = CommandProcessor?.Invoke(request.Name, request.Parameters);

                await Send(args.Socket, result);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    await Stop();
                    await Start(serviceName);
                }
            }
        }
        #endregion
    }
}
