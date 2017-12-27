using SmartHub.UWP.Core.Communication;
using SmartHub.UWP.Core.Communication.Http;
using SmartHub.UWP.Core.Communication.Http.RequestHandlers;
using SmartHub.UWP.Core.Communication.Tcp;
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

        #region Fields
        private readonly Dictionary<string, ApiMethod> apiMethods = new Dictionary<string, ApiMethod>();
        private StreamServer tcpServer = new StreamServer();
        private HttpServer httpServer = new HttpServer();
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

            var requestHandler = new RESTHandler();
            requestHandler.RegisterController(new RestController());
            httpServer.RequestHandler = requestHandler;
        }
        public async override void StartPlugin()
        {
            await tcpServer.StartAsync(TcpServiceName);
            await httpServer.StartAsync(WebServiceName);
        }
        public async override void StopPlugin()
        {
            await tcpServer.StopAsync();
            await httpServer.StopAsync();
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
