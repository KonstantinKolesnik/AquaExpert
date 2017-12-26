using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.System.Threading;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http
{
    //http://devstyle.io/blog/2015/11/4/minimal-rest-server-for-windows-10-iot-core-
    public class HttpServer
    {
        #region Fields
        private StreamSocketListener listener;
        private string serviceName;
        private List<String> acceptedVerbs = new List<String> { HttpMethod.Get.Method, HttpMethod.Post.Method, HttpMethod.Delete.Method, HttpMethod.Put.Method };
        #endregion

        #region Properties
        public RESTHandler RestHandler
        {
            get;
        } = new RESTHandler();
        #endregion

        #region Public methods
        public async Task StartAsync(string serviceName)
        {
            if (listener == null)
            {
                this.serviceName = serviceName;

                listener = new StreamSocketListener();
                listener.ConnectionReceived += (s, e) => ThreadPool.RunAsync(async w => ProcessRequestAsync(e.Socket));

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
        private async void ProcessRequestAsync(StreamSocket socket)
        {
            try
            {
                HttpRequest request;

                try
                {
                    request = HttpRequest.Read(socket);
                }
                catch (Exception ex)
                {
                    await WriteInternalServerErrorResponse(socket, ex);
                    return;
                }

                if (acceptedVerbs.Contains(request.Method.Method))
                {
                    HttpResponse response;

                    try
                    {
                        response = await RestHandler.Handle(request);
                    }
                    catch (Exception ex)
                    {
                        await WriteInternalServerErrorResponse(socket, ex);
                        return;
                    }

                    await WriteResponse(response, socket);

                    await socket.CancelIOAsync();
                    socket.Dispose();
                }
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

        #region Private methods
        private static async Task WriteInternalServerErrorResponse(StreamSocket socket, Exception ex)
        {
            var msg = "Internal server error.";
            //if (Debugger.IsAttached)
            msg += Environment.NewLine + ex;

            await WriteResponse(new HttpResponse(HttpStatusCode.InternalServerError, msg), socket);
        }
        private static async Task WriteResponse(HttpResponse response, StreamSocket socket)
        {
            using (var stream = socket.OutputStream.AsStreamForWrite())
                await response.WriteToStream(stream);
        }
        #endregion
    }
}
