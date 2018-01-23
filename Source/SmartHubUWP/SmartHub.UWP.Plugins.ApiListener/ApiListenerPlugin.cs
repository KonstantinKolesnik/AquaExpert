using SmartHub.UWP.Core.Communication;
using SmartHub.UWP.Core.Communication.Http;
using SmartHub.UWP.Core.Communication.Http.RequestHandlers;
using SmartHub.UWP.Core.Communication.Tcp;
using SmartHub.UWP.Core.Communication.Udp;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [Plugin]
    public class ApiListenerPlugin : PluginBase
    {
        public const string TcpServiceName = "11111";
        public const string WebServiceName = "7777";
        public const string UdpServiceName = "55555";
        public const string UdpMulticastAddress = "224.3.0.5";

        #region Fields
        private StreamServer tcpServer = new StreamServer();
        private HttpServer httpServer = new HttpServer();
        private UdpClient udpClient = new UdpClient();

        private readonly Dictionary<string, ApiMethod> apiMethods = new Dictionary<string, ApiMethod>();
        #endregion

        #region Imports
        /// <summary>
        /// API methods from all plugins
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<ApiMethod, ApiMethodAttribute>> ApiMethods
        {
            get; set;
        }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            foreach (var apiMethod in ApiMethods)
                apiMethods.Add(apiMethod.Metadata.MethodName, apiMethod.Value);

            tcpServer.ApiRequestHandler = ApiRequestHandler;

            //var requestHandler = new RESTHandler();
            //requestHandler.RegisterController(new RestController());
            //httpServer.RequestHandler = requestHandler;

            httpServer.RequestHandler = new PlainHandler() { ApiRequestHandler = ApiRequestHandler };

            udpClient.MessageReceived += /*async*/ (sender, e) =>
            {
                int a = 0;
                int b = a;
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                //{
                //    foreach (var msg in WemosMessage.FromDto(e.Data))
                //        await ProcessMessage(msg, e.RemoteAddress);
                //});
            };

        }
        public async override void StartPlugin()
        {
            await tcpServer.StartAsync(TcpServiceName);
            await httpServer.StartAsync(WebServiceName);
            await udpClient.Start(UdpServiceName, UdpMulticastAddress);
        }
        public async override void StopPlugin()
        {
            await tcpServer.StopAsync();
            await httpServer.StopAsync();
            await udpClient.Stop();
        }
        #endregion

        #region Private methods
        private object ApiRequestHandler(ApiRequest request)
        {
            try
            {
                return apiMethods.ContainsKey(request.Name) ? apiMethods[request.Name].Invoke(request.Parameters) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
