using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.System.Threading;

namespace SmartHub.UWP.Core.Communication.Tcp
{
    public class StreamServer
    {
        #region Fields
        private StreamSocketListener listener;
        private string serviceName;
        #endregion

        #region Properties
        public ApiRequestHandler ApiRequestHandler
        {
            get; set;
        }
        #endregion

        #region Public methods
        public async Task StartAsync(string serviceName)
        {
            if (listener == null)
            {
                this.serviceName = serviceName;

                listener = new StreamSocketListener();
                //listener.ConnectionReceived += Listener_ConnectionReceived;
                listener.ConnectionReceived += (s, e) => ThreadPool.RunAsync(async w => await ProcessRequestAsync(e.Socket));
                //listener.ConnectionReceived += async (s, e) =>
                //{
                //    await ThreadPool.RunAsync(async w => await ProcessRequestAsync(e.Socket));
                //};


                // If necessary, tweak the listener's control options before carrying out the bind operation.
                // These options will be automatically applied to the connected StreamSockets resulting from
                // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
                // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
                listener.Control.KeepAlive = false;

                await listener.BindServiceNameAsync(serviceName);
            }
        }
        public async Task StopAsync()
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
        private async Task ProcessRequestAsync(StreamSocket socket)
        {
            try
            {
                var requestDto = await Utils.ReceiveAsync(socket);
                var request = CommunucationUtils.DtoDeserialize<ApiRequest>(requestDto);

                if (request != null)
                {
                    var response = ApiRequestHandler?.Invoke(request);
                    if (response != null)
                    {
                        var responseDto = CommunucationUtils.DtoSerialize(response);
                        await Utils.SendAsync(socket, responseDto);
                    }
                }

                await socket.CancelIOAsync();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    await StopAsync();
                    await StartAsync(serviceName);
                }
            }
        }
        #endregion
    }
}
