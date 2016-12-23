using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using SmartHub.UWP.Core.Communication.Transporting;
using System.Threading;
using System.Text;
using System.IO;

namespace SmartHub.UWP.Core.Communication.Stream
{
    public class StreamClientOld
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
                using (var writer = new DataWriter(client.OutputStream))
                {
                    // Write first the length of the string as UINT32 value followed up by the string. 
                    //string stringToSend = "Hello";
                    //writer.WriteUInt32(writer.MeasureString(stringToSend));
                    //writer.WriteString(stringToSend);

                    //var newData = parameters.Select(p => Transport.Serialize(p)).ToArray();
                    var newData = Transport.Serialize(parameters);

                    writer.WriteUInt32(writer.MeasureString(commandName + newData));
                    writer.WriteString(commandName + newData);

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

    public class StreamClient : IDisposable
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

        ~StreamClient()
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
        public async Task SendAsync(string commandName, params object[] parameters)
        {
            if (socket != null)
            {
                try
                {
                    using (DataWriter writer = new DataWriter(socket.OutputStream))
                    {
                        var data = new StreamData()
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
        public void Stop()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
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
