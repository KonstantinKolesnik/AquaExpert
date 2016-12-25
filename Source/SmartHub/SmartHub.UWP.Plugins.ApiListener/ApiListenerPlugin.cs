using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Communication.Transporting;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [Plugin]
    public class ApiListenerPlugin : PluginBase
    {
        public const string ServiceName = "11111";

        #region Fields
        private StreamSocketListener listener;
        private readonly Dictionary<string, ApiCommand> apiHandlers = new Dictionary<string, ApiCommand>();
        #endregion

        #region Imports
        [ImportMany]
        public IEnumerable<Lazy<ApiCommand, ApiCommandAttribute>> ApiCommands { get; set; }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            foreach (var handler in ApiCommands)
                apiHandlers.Add(handler.Metadata.CommandName, handler.Value);
        }
        public async override void StartPlugin()
        {
            await Start();
        }
        public async override void StopPlugin()
        {
            await Stop();
        }
        #endregion

        #region Private methods
        private async Task Start()
        {
            if (listener == null)
            {
                listener = new StreamSocketListener();
                listener.ConnectionReceived += Listener_ConnectionReceived;

                // If necessary, tweak the listener's control options before carrying out the bind operation.
                // These options will be automatically applied to the connected StreamSockets resulting from
                // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
                // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
                listener.Control.KeepAlive = false;

                await listener.BindServiceNameAsync(ServiceName);
            }
        }
        private async Task Stop()
        {
            if (listener != null)
            {
                await listener.CancelIOAsync();
                listener.Dispose();
                listener = null;
            }
        }

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
                var request = Transport.Deserialize<ApiRequest>(str);

                var result = apiHandlers.ContainsKey(request.CommandName) ? apiHandlers[request.CommandName](request.Parameters) : null;

                await Send(args.Socket, result);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    await Stop();
                    await Start();
                }
            }
        }
        #endregion
    }
}
