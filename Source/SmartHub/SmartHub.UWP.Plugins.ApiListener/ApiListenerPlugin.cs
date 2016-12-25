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
        public override void StopPlugin()
        {
            Stop();
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
        private void Stop()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }
        #endregion

        #region Event handlers
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var reader = new DataReader(args.Socket.InputStream);
            reader.InputStreamOptions = InputStreamOptions.Partial;

            try
            {
                while (true)
                {
                    uint sizeFieldLength = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldLength == sizeof(uint))
                    {
                        uint dataLength = reader.ReadUInt32();
                        uint actualDataLength = await reader.LoadAsync(dataLength);
                        if (dataLength == actualDataLength)
                        {
                            string str = reader.ReadString(actualDataLength);
                            var data = Transport.Deserialize<ApiRequest>(str);

                            var result = apiHandlers.ContainsKey(data.CommandName) ? apiHandlers[data.CommandName](data.Parameters) : null;

                            using (DataWriter writer = new DataWriter(args.Socket.OutputStream))
                            {
                                str = Transport.Serialize(result);
                                writer.WriteUInt32(writer.MeasureString(str));
                                writer.WriteString(str);

                                await writer.StoreAsync();
                                await writer.FlushAsync();
                                writer.DetachStream();
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    Stop();
                    await Start();
                }
            }
        }
        #endregion
    }
}
