using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using SmartHub.Core.Plugins;
using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.SignalR
{
    [Plugin]
    public class SignalRPlugin : PluginBase
    {
        #region Fields
        // This will *ONLY* bind to localhost, if you want to bind to all addresses use http://*:8080 to bind to all addresses. 
        // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx for more information.
        private const string url = "http://localhost:55556";
        private IDisposable wsServer;
        #endregion

        #region Import
        [ImportMany("ADD7FADD-D706-4D5A-9103-09B26FD2FD9C")]
        public Action<string, string>[] OnClientSignal { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            //var handlers = RegisterHandlers();
        }
        public override void StartPlugin()
        {
            wsServer = WebApp.Start<WSStartup>(url);
        }
        public override void StopPlugin()
        {
            if (wsServer != null)
            {
                wsServer.Dispose();
                wsServer = null;
            }
        }
        #endregion

        #region Private methods
        //private InternalDictionary<ListenerHandlerBase> RegisterHandlers()
        //{
        //    var handlers = new InternalDictionary<ListenerHandlerBase>();

        //    // регистрируем обработчики для методов плагинов
        //    foreach (var action in HttpCommandHandlers)
        //    {
        //        Logger.Info("Register WebApi command handler '{0}'", action.Metadata.Url);

        //        var handler = new ApiListenerHandler(action.Value);
        //        handlers.Register(action.Metadata.Url, handler);
        //    }

        //    // регистрируем обработчики для ресурсов
        //    foreach (var plugin in Context.GetAllPlugins())
        //    {
        //        Type type = plugin.GetType();
        //        var attributes = type.GetCustomAttributes<HttpResourceAttribute>();

        //        foreach (var attribute in attributes)
        //        {
        //            Logger.Info("Register HTTP resource handler: '{0}'", attribute.Url);

        //            var resHandler = new ResourceListenerHandler(type.Assembly, attribute.ResourcePath, attribute.ContentType);
        //            handlers.Register(attribute.Url, resHandler);
        //        }
        //    }

        //    return handlers;
        //}
        #endregion

        public class WSStartup
        {
            public void Configuration(IAppBuilder app)
            {
                //app.UseCors(CorsOptions.AllowAll);
                app.MapSignalR();
            }
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
