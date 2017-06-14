using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Core.Communication.Stream
{
    static class Utils
    {
        #region Fields
        private static JsonSerializerSettings dtoSettings = new JsonSerializerSettings()
        {
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,

            //TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };
        #endregion

        #region Public methods
        public static async Task ConnectAsync(StreamSocket socket, string hostName, string serviceName)
        {
            try
            {
                var host = new HostName(hostName);
                await socket.ConnectAsync(host, serviceName);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;
            }
        }
        public static async Task<string> ReceiveAsync(StreamSocket socket)
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
        public static async Task SendAsync(StreamSocket socket, string data)
        {
            if (socket != null && !string.IsNullOrEmpty(data))
                using (var writer = new DataWriter(socket.OutputStream))
                {
                    writer.WriteUInt32(writer.MeasureString(data));
                    writer.WriteString(data);

                    try
                    {
                        await writer.StoreAsync();
                        //await writer.FlushAsync();
                    }
                    catch (Exception exception)
                    {
                        // If this is an unknown status it means that the error if fatal and retry will likely fail.
                        if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                            throw;
                    }

                    writer.DetachStream();
                }
        }

        public static string DtoSerialize(object data)
        {
            return JsonConvert.SerializeObject(data, typeof(object), dtoSettings);
        }
        public static T DtoDeserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, dtoSettings);

            //List<JsonConverter> converters = new List<JsonConverter>();
            //converters.Add(new ToolConverter());
            //converters.Add(new CLSToolOperationConverter());
            //return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { Converters = converters });
        }
        #endregion
    }
}
