using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.HttpListener.Handlers;
using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;


namespace SmartHub.Plugins.HttpListener
{
    [Plugin]
    public class HttpListenerPlugin : PluginBase
    {
        #region Fields
        private const string BASE_URL_HTTP = "http://localhost:55555";
        private HttpSelfHostConfiguration httpConfig;
        private HttpSelfHostServer httpServer;

        private const string url = "http://localhost:55556";
        private IDisposable wsServer;
        #endregion

        #region Import
        [ImportMany("5D358D8E-2310-49FE-A660-FB3ED7003B4C")]
        public Lazy<Func<HttpRequestParams, object>, IHttpCommandAttribute>[] HttpCommandHandlers { get; set; }

        [ImportMany("ADD7FADD-D706-4D5A-9103-09B26FD2FD9C")]
        public Action<string, string>[] OnClientSignal { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            var handlers = RegisterHandlers();

            httpConfig = new HttpSelfHostConfiguration(BASE_URL_HTTP);
            httpConfig.DependencyResolver = new DependencyResolver(handlers, Logger);
            httpConfig.Routes.MapHttpRoute("Global", "{*url}", new { controller = "Common", action = "Index" });
        }
        public override void StartPlugin()
        {
            httpServer = new HttpSelfHostServer(httpConfig);
            httpServer.OpenAsync().Wait();

            wsServer = WebApp.Start<WSStartup>(url);
        }
        public override void StopPlugin()
        {
            if (httpServer != null)
            {
                httpServer.CloseAsync().Wait();
                httpServer.Dispose();
                httpServer = null;
            }

            if (wsServer != null)
            {
                wsServer.Dispose();
                wsServer = null;
            }
        }
        #endregion

        #region Private methods
        private InternalDictionary<ListenerHandlerBase> RegisterHandlers()
        {
            var handlers = new InternalDictionary<ListenerHandlerBase>();

            // регистрируем обработчики для методов плагинов
            foreach (var action in HttpCommandHandlers)
            {
                Logger.Info("Register WebApi command handler '{0}'", action.Metadata.Url);

                var handler = new ApiListenerHandler(action.Value);
                handlers.Register(action.Metadata.Url, handler);
            }

            // регистрируем обработчики для ресурсов
            foreach (var plugin in Context.GetAllPlugins())
            {
                Type type = plugin.GetType();
                var attributes = type.GetCustomAttributes<HttpResourceAttribute>();

                foreach (var attribute in attributes)
                {
                    Logger.Info("Register HTTP resource handler: '{0}'", attribute.Url);

                    var resHandler = new ResourceListenerHandler(type.Assembly, attribute.ResourcePath, attribute.ContentType);
                    handlers.Register(attribute.Url, resHandler);
                }
            }

            return handlers;
        }
        #endregion
    }

    class WSStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
            //Run(OnClientSignal, x => x(name, message));
        }
    }
}
