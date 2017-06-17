using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StreamServer
    {
        #region Fields
        private StreamSocketListener listener;
        private string serviceName;
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
                listener.Control.KeepAlive = true;

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

        #region Event handlers
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                using (var socket = args.Socket)
                {
                    string requestDto = await Utils.ReceiveAsync(socket);
                    var request = Utils.DtoDeserialize<CommandRequest>(requestDto);

                    if (request != null)
                    {
                        var response = CommandProcessor?.Invoke(request.Name, request.Parameters);
                        if (response != null)
                        {
                            var dataDto = Utils.DtoSerialize(response);
                            await Utils.SendAsync(socket, dataDto);

                            await socket.CancelIOAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    await Stop();
                    await Start(serviceName);
                }
            }
        }
        #endregion
    }
}
