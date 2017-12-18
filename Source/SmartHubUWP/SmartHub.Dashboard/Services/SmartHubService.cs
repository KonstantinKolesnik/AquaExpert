using SmartHub.Dashboard.Common;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SmartHub.Dashboard.Services
{
    public class SmartHubService
    {
        #region Fields
        //public const int remoteServiceName = 11111;
        private const int MAX_BUFFER_SIZE_KB = 1024;
        #endregion

        public T Function<T>(string host, int port, string functionName, params object[] parameters)
        {
            T result = default(T);

            using (var client = new TcpClient(host, port))
            {
                using (var stream = client.GetStream())
                {
                    var request = new SmartHubCommandRequest(functionName, parameters);
                    var data = Utils.DtoSerialize(request);

                    var writer = new BinaryWriter(stream);
                    writer.Write((uint)data.Length);
                    writer.Write(data);

                    //-------------------------------

                    //var reader = new BinaryReader(stream);
                    //var s = reader.ReadString();

                    string responseDto = null;

                    byte[] b = new byte[sizeof(uint)];
                    var read = stream.Read(b, 0, b.Length);
                    if (read == b.Length)
                    {
                        uint dataLength = BitConverter.ToUInt32(b, 0);
                        uint loadedDataLength = 0;
                        var sb = new StringBuilder();

                        while (loadedDataLength < dataLength)
                        {
                            var buffer = new byte[MAX_BUFFER_SIZE_KB * 1024];
                            var n = stream.Read(buffer, 0, buffer.Length);
                            sb.Append(Encoding.UTF8.GetString(buffer, 0, n));

                            loadedDataLength += (uint)n;
                        }

                        responseDto = sb.ToString();
                        result = Utils.DtoDeserialize<T>(responseDto);
                    }

                    stream.Close();
                    client.Close();
                }
            }

            return result;
        }
    }
}
