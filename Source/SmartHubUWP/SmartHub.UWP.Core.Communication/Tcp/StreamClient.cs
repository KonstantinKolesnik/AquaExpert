using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace SmartHub.UWP.Core.Communication.Tcp
{
    public class StreamClient
    {
        #region Fields
        private StreamSocket socket;
        #endregion

        #region Public methods
        public async Task StartAsync(string hostName, string serviceName)
        {
            if (socket == null)
            {
                socket = new StreamSocket();
                socket.Control.KeepAlive = false;

                await Utils.ConnectAsync(socket, hostName, serviceName);
            }
        }
        public async Task<T> RequestAsync<T>(string commandName, params object[] parameters)
        {
            T result = default(T);

            var request = new ApiRequest(commandName, parameters);
            var success = await Utils.SendAsync(socket, CommunucationUtils.DtoSerialize(request));

            if (success)
            {
                var responseDto = await Utils.ReceiveAsync(socket);
                return CommunucationUtils.DtoDeserialize<T>(responseDto);
            }

            return result;
        }
        public void Stop()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }
        #endregion

        #region Static methods
        public static async Task<T> RequestAsync<T>(string hostName, string serviceName, string commandName, params object[] parameters)
        {
            T result = default(T);

            using (var socket = new StreamSocket())
            {
                socket.Control.KeepAlive = false;

                var success = await Utils.ConnectAsync(socket, hostName, serviceName);
                if (success)
                {
                    var request = new ApiRequest(commandName, parameters);
                    success = await Utils.SendAsync(socket, CommunucationUtils.DtoSerialize(request));

                    if (success)
                    {
                        var responseDto = await Utils.ReceiveAsync(socket);
                        result = CommunucationUtils.DtoDeserialize<T>(responseDto);
                    }
                }
            }

            return result;
        }
        #endregion
    }
}
