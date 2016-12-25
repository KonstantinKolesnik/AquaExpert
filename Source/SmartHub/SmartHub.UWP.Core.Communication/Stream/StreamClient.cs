using SmartHub.UWP.Core.Communication.Transporting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StreamClient
    {
        #region Fields
        private StreamSocket socket;
        private string hostName;
        private string serviceName;
        #endregion

        #region Public methods
        public async Task StartAsync(string hostName, string serviceName)
        {
            if (socket == null)
            {
                this.hostName = hostName;
                this.serviceName = serviceName;

                socket = new StreamSocket();
                socket.Control.KeepAlive = false;

                await Connect(socket, hostName, serviceName);
            }
        }
        public void Stop()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }

        public async Task<T> RequestAsync<T>(string commandName, params object[] parameters)
        {
            var data = new ApiRequest()
            {
                CommandName = commandName,
                Parameters = parameters
            };

            await Send(socket, data);

            string str = await Receive(socket);

            return Transport.Deserialize<T>(str);
        }
        public async Task RequestAsync(string commandName, params object[] parameters)
        {
            string result = await RequestAsync<string>(commandName, parameters);
        }
        #endregion

        public static async Task<T> RequestAsync<T>(string hostName, string serviceName, string commandName, params object[] parameters)
        {
            var socket = new StreamSocket();
            socket.Control.KeepAlive = false;

            await Connect(socket, hostName, serviceName);

            var data = new ApiRequest()
            {
                CommandName = commandName,
                Parameters = parameters
            };
            await Send(socket, data);

            string str = await Receive(socket);

            socket.Dispose();

            return Transport.Deserialize<T>(str);
        }
        public static async Task RequestAsync(string hostName, string serviceName, string commandName, params object[] parameters)
        {
            string result = await RequestAsync<string>(hostName, serviceName, commandName, parameters);
        }

        #region Private methods
        private static async Task Connect(StreamSocket client, string hostName, string serviceName)
        {
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
        private static async Task Send(StreamSocket client, ApiRequest data)
        {
            if (client != null)
                using (var writer = new DataWriter(client.OutputStream))
                {
                    var str = Transport.Serialize(data);
                    writer.WriteUInt32(writer.MeasureString(str));
                    writer.WriteString(str);

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
                }
        }
        private static async Task<string> Receive(StreamSocket client)
        {
            string result = null;

            if (client != null)
                using (var reader = new DataReader(client.InputStream))
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
        #endregion
    }

    // the one from Ecos:
    public class StreamClientNew : IDisposable
    {
        #region Fields
        private const int MAX_BUFFER_SIZE = 4096; //64
        private bool isDisposed = false;
        private StreamSocket socket = null;
        private string hostName;
        private string serviceName;
        #endregion

        #region Events
        public event EventHandler Disconnected;
        public event EventHandler ServerDisconnected;
        public event EventHandler<StringEventArgs> Received;
        #endregion

        ~StreamClientNew()
        {
            Dispose(false);
        }

        #region Public Methods
        public async Task StartAsync(string hostName, string serviceName, int timeOut = 10000)
        {
            if (socket == null)
            {
                this.hostName = hostName;
                this.serviceName = serviceName;

                try
                {
                    socket = new StreamSocket();
                    socket.Control.KeepAlive = true;

                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(timeOut);

                    await socket.ConnectAsync(new HostName(hostName), serviceName).AsTask(cts.Token);

                    ReceiveAsync();
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    //    throw;
                }

                //Stop();
            }
        }
        public void Stop()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }

        public async Task SendAsync(string commandName, params object[] parameters)
        {
            if (socket != null)
            {
                try
                {
                    using (DataWriter writer = new DataWriter(socket.OutputStream))
                    {
                        var data = new ApiRequest()
                        {
                            CommandName = commandName,
                            Parameters = parameters
                        };

                        var newData = Transport.Serialize(data);
                        writer.WriteUInt32(writer.MeasureString(newData));
                        writer.WriteString(newData);

                        await writer.StoreAsync();
                        writer.DetachStream();
                    }
                }
                catch (Exception ex)
                {
                    //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    //    throw;

                    Stop();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods
        private async Task ReceiveAsync()
        {
            if (socket != null)
            {
                StringBuilder strBuilder = new StringBuilder();
                uint bytesRead = 0;

                try
                {
                    using (DataReader reader = new DataReader(socket.InputStream))
                    {
                        reader.InputStreamOptions = InputStreamOptions.Partial;

                        bytesRead = await reader.LoadAsync(MAX_BUFFER_SIZE);

                        if (bytesRead > 0)
                        {
                            string str = reader.ReadString(reader.UnconsumedBufferLength);
                            Received?.Invoke(this, new StringEventArgs(str));

                            await ReceiveAsync();
                        }
                        else // indicates graceful closure and that no more bytes will ever be read
                        {
                            //if (IsConnected)
                            {
                                Stop();
                                ServerDisconnected?.Invoke(this, EventArgs.Empty);
                            }
                        }
                        //reader.DetachStream();


                        //while (reader.UnconsumedBufferLength > 0)
                        //{
                        //    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
                        //    await reader.LoadAsync(1);// MAX_BUFFER_SIZE);
                        //}

                        //do
                        //{
                        //    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
                        //    bytesRead = await reader.LoadAsync(MAX_BUFFER_SIZE);
                        //} while (reader.UnconsumedBufferLength > 0);

                        //reader.DetachStream();

                        //Received?.Invoke(this, new StringEventArgs(strBuilder.ToString()));

                        //await ReceiveAsync();
                    }
                }
                catch (Exception ex)
                {
                    SocketErrorStatus ses = SocketError.GetStatus(ex.HResult);
                    if (ses == SocketErrorStatus.Unknown)
                        throw;
                    else if (ses == SocketErrorStatus.OperationAborted) // The overlapped operation was aborted due to the closure of the Socket.
                    {
                    }

                    Stop();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    Stop();
                }

                // If disposing is false, only the following code is executed.
                {
                    // Clean up unmanaged resources here. 
                }

                isDisposed = true;
            }
        }
        #endregion
    }
}
