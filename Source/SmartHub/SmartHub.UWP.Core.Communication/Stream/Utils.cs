using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Stream
{
    static class Utils
    {
        private const int MAX_BUFFER_SIZE_KB = 1024;

        #region Fields
        private static JsonSerializerSettings dtoSettings = new JsonSerializerSettings()
        {
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,

            //TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };
        #endregion

        #region Public methods
        public static async Task<bool> ConnectAsync(StreamSocket socket, string hostName, string serviceName, int timeOut = 10000)
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(timeOut);

                await socket.ConnectAsync(new HostName(hostName), serviceName).AsTask(cts.Token);
                return true;
            }
            catch (TaskCanceledException ex)
            {
            }
            catch (Exception ex)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    throw;
            }

            return false;
        }
        public static async Task<string> ReceiveAsync(StreamSocket socket)
        {
            string result = null;

            if (socket != null)
                using (var reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    try
                    {
                        uint sizeFieldLength = await reader.LoadAsync(sizeof(uint));
                        if (sizeFieldLength == sizeof(uint))
                        {
                            uint dataLength = reader.ReadUInt32();

                            uint actualDataLength = 0;
                            var sb = new StringBuilder();
                            while (actualDataLength < dataLength)
                            {
                                var read = await reader.LoadAsync(MAX_BUFFER_SIZE_KB * 1024);
                                sb.Append(reader.ReadString(read));
                                actualDataLength += read;
                            }

                            result = sb.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // If this is an unknown status it means that the error if fatal and retry will likely fail.
                        //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                        //    throw;
                    }

                    reader.DetachStream();
                }

            return result;
        }
        public static async Task<bool> SendAsync(StreamSocket socket, string data)
        {
            if (socket != null && !string.IsNullOrEmpty(data))
                using (var writer = new DataWriter(socket.OutputStream))
                {
                    try
                    {
                        writer.WriteUInt32(writer.MeasureString(data));
                        writer.WriteString(data);

                        await writer.StoreAsync();
                        await writer.FlushAsync();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        // If this is an unknown status it means that the error if fatal and retry will likely fail.
                        //if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                        //    throw;
                    }

                    writer.DetachStream();
                }

            return false;
        }

        public static string DtoSerialize(object data)
        {
            return JsonConvert.SerializeObject(data, typeof(object), dtoSettings);
        }
        public static T DtoDeserialize<T>(string data)
        {
            T result = default(T);

            if (!string.IsNullOrEmpty(data))
                try
                {
                    result = JsonConvert.DeserializeObject<T>(data, dtoSettings);
                }
                catch (Exception ex)
                {
                }

            return result;

            //List<JsonConverter> converters = new List<JsonConverter>();
            //converters.Add(new ToolConverter());
            //converters.Add(new CLSToolOperationConverter());
            //return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { Converters = converters });
        }
        #endregion
    }
}
