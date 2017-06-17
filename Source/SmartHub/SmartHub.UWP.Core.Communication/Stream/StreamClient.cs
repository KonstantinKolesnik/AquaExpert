using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace SmartHub.UWP.Core.Communication.Stream
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
            var request = new CommandRequest(commandName, parameters);
            await Utils.SendAsync(socket, Utils.DtoSerialize(request));

            var responseDto = await Utils.ReceiveAsync(socket);
            return Utils.DtoDeserialize<T>(responseDto);
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

                var connected = await Utils.ConnectAsync(socket, hostName, serviceName);
                if (connected)
                {
                    var request = new CommandRequest(commandName, parameters);
                    await Utils.SendAsync(socket, Utils.DtoSerialize(request));

                    var responseDto = await Utils.ReceiveAsync(socket);
                    result = Utils.DtoDeserialize<T>(responseDto);
                }
            }

            return result;
        }
        #endregion
    }

    // the one from Ecos:
    //public class StreamClientNew : IDisposable
    //{
    //    #region Fields
    //    private const int MAX_BUFFER_SIZE = 4096; //64
    //    private bool isDisposed = false;
    //    private StreamSocket socket = null;
    //    private string hostName;
    //    private string serviceName;
    //    #endregion

    //    #region Events
    //    public event EventHandler Disconnected;
    //    public event EventHandler ServerDisconnected;
    //    public event EventHandler<StringEventArgs> Received;
    //    #endregion

    //    ~StreamClientNew()
    //    {
    //        Dispose(false);
    //    }

    //    #region Public Methods
    //    public async Task StartAsync(string hostName, string serviceName, int timeOut = 10000)
    //    {
    //        if (socket == null)
    //        {
    //            this.hostName = hostName;
    //            this.serviceName = serviceName;

    //            try
    //            {
    //                socket = new StreamSocket();
    //                socket.Control.KeepAlive = true;

    //                var cts = new CancellationTokenSource();
    //                cts.CancelAfter(timeOut);

    //                await socket.ConnectAsync(new HostName(hostName), serviceName).AsTask(cts.Token);

    //                ReceiveAsync();
    //            }
    //            catch (TaskCanceledException)
    //            {
    //            }
    //            catch (Exception ex)
    //            {
    //                //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
    //                //    throw;
    //            }

    //            //Stop();
    //        }
    //    }
    //    public void Stop()
    //    {
    //        if (socket != null)
    //        {
    //            socket.Dispose();
    //            socket = null;
    //        }
    //    }

    //    public async Task SendAsync(string commandName, params object[] parameters)
    //    {
    //        if (socket != null)
    //        {
    //            try
    //            {
    //                using (DataWriter writer = new DataWriter(socket.OutputStream))
    //                {
    //                    var data = new CommandRequest(commandName, parameters);

    //                    var dataDto = Transport.Serialize(data);
    //                    writer.WriteUInt32(writer.MeasureString(dataDto));
    //                    writer.WriteString(dataDto);

    //                    await writer.StoreAsync();
    //                    writer.DetachStream();
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
    //                //    throw;

    //                Stop();
    //            }
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        GC.SuppressFinalize(this);
    //    }
    //    #endregion

    //    #region Private Methods
    //    private async Task ReceiveAsync()
    //    {
    //        if (socket != null)
    //        {
    //            StringBuilder strBuilder = new StringBuilder();
    //            uint bytesRead = 0;

    //            try
    //            {
    //                using (DataReader reader = new DataReader(socket.InputStream))
    //                {
    //                    reader.InputStreamOptions = InputStreamOptions.Partial;

    //                    bytesRead = await reader.LoadAsync(MAX_BUFFER_SIZE);

    //                    if (bytesRead > 0)
    //                    {
    //                        string str = reader.ReadString(reader.UnconsumedBufferLength);
    //                        Received?.Invoke(this, new StringEventArgs(str));

    //                        await ReceiveAsync();
    //                    }
    //                    else // indicates graceful closure and that no more bytes will ever be read
    //                    {
    //                        //if (IsConnected)
    //                        {
    //                            Stop();
    //                            ServerDisconnected?.Invoke(this, EventArgs.Empty);
    //                        }
    //                    }
    //                    //reader.DetachStream();


    //                    //while (reader.UnconsumedBufferLength > 0)
    //                    //{
    //                    //    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
    //                    //    await reader.LoadAsync(1);// MAX_BUFFER_SIZE);
    //                    //}

    //                    //do
    //                    //{
    //                    //    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
    //                    //    bytesRead = await reader.LoadAsync(MAX_BUFFER_SIZE);
    //                    //} while (reader.UnconsumedBufferLength > 0);

    //                    //reader.DetachStream();

    //                    //Received?.Invoke(this, new StringEventArgs(strBuilder.ToString()));

    //                    //await ReceiveAsync();
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                SocketErrorStatus ses = SocketError.GetStatus(ex.HResult);
    //                if (ses == SocketErrorStatus.Unknown)
    //                    throw;
    //                else if (ses == SocketErrorStatus.OperationAborted) // The overlapped operation was aborted due to the closure of the Socket.
    //                {
    //                }

    //                Stop();
    //            }
    //        }
    //    }

    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (!isDisposed)
    //        {
    //            // If disposing equals true, dispose all managed and unmanaged resources. 
    //            if (disposing)
    //            {
    //                // Dispose managed resources.
    //                Stop();
    //            }

    //            // If disposing is false, only the following code is executed.
    //            {
    //                // Clean up unmanaged resources here. 
    //            }

    //            isDisposed = true;
    //        }
    //    }
    //    #endregion
    //}
}
