using Microsoft.Owin.Hosting;
using Owin;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.HttpListener.Handlers;
using SmartHub.Plugins.HttpListener.Modules;
using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace SmartHub.Plugins.HttpListener
{
    [Plugin]
    public class HttpListenerPlugin : PluginBase
    {
        #region Fields
        private const string BASE_URL_HTTP = "http://+:55555";

        private IDisposable server;
        private InternalDictionary<IListenerHandler> listenerHandlers = new InternalDictionary<IListenerHandler>();
        #endregion

        #region Import
        [ImportMany(HttpCommandAttribute.CLSID)]
        public Lazy<Func<HttpRequestParams, object>, IHttpCommandAttribute>[] HttpCommandHandlers { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            RegisterListenerHandlers();
        }
        public override void StartPlugin()
        {
            // see http://benfoster.io/blog/how-to-write-owin-middleware-in-5-different-steps

            server = WebApp.Start(BASE_URL_HTTP, (IAppBuilder appBuilder) => {
                appBuilder
                    .Use<HttpListenerModule>(listenerHandlers, Logger) // listen to webapi and resource requests
                    .Use<Error404Module>();//.UseErrorPage();
            });
        }
        public override void StopPlugin()
        {
            if (server != null)
            {
                server.Dispose();
                server = null;
            }
        }
        #endregion

        #region Private methods
        private void RegisterListenerHandlers()
        {
            listenerHandlers.Clear();

            // register WebApi handlers:
            foreach (var action in HttpCommandHandlers)
            {
                Logger.Info("Register WebApi command handler '{0}'", action.Metadata.Url);
                listenerHandlers.Register(action.Metadata.Url, new WebApiListenerHandler(action.Value));
            }

            // register resource handlers:
            foreach (var plugin in Context.GetAllPlugins())
            {
                Type type = plugin.GetType();
                var attributes = type.GetCustomAttributes<HttpResourceAttribute>();

                foreach (var attribute in attributes)
                {
                    Logger.Info("Register HTTP resource handler: '{0}'", attribute.Url);
                    listenerHandlers.Register(attribute.Url, new ResourceListenerHandler(type.Assembly, attribute.ResourcePath, attribute.ContentType));
                }
            }
        }
        #endregion
    }
}
