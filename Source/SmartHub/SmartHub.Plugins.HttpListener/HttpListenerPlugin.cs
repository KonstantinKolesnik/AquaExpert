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

namespace SmartHub.Plugins.HttpListener
{
    [Plugin]
    public class HttpListenerPlugin : PluginBase
    {
        #region Fields
        private const string BASE_URL_HTTP = "http://+:55555";

        private IDisposable server;
        private InternalDictionary<IListenerHandler> registeredHandlers;
        #endregion

        #region Import
        [ImportMany("5D358D8E-2310-49FE-A660-FB3ED7003B4C")]
        public Lazy<Func<HttpRequestParams, object>, IHttpCommandAttribute>[] HttpCommandHandlers { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            registeredHandlers = RegisterAllHandlers();
        }
        public override void StartPlugin()
        {
            server = WebApp.Start(BASE_URL_HTTP, ConfigureModules);
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
        private InternalDictionary<IListenerHandler> RegisterAllHandlers()
        {
            var result = new InternalDictionary<IListenerHandler>();

            // регистрируем обработчики для методов плагинов
            foreach (var action in HttpCommandHandlers)
            {
                Logger.Info("Register WebApi command handler '{0}'", action.Metadata.Url);
                result.Register(action.Metadata.Url, new WebApiListenerHandler(action.Value));
            }

            // регистрируем обработчики для ресурсов
            foreach (var plugin in Context.GetAllPlugins())
            {
                Type type = plugin.GetType();
                var attributes = type.GetCustomAttributes<HttpResourceAttribute>();

                foreach (var attribute in attributes)
                {
                    Logger.Info("Register HTTP resource handler: '{0}'", attribute.Url);
                    result.Register(attribute.Url, new ResourceListenerHandler(type.Assembly, attribute.ResourcePath, attribute.ContentType));
                }
            }

            return result;
        }
        private void ConfigureModules(IAppBuilder appBuilder)
        {
            appBuilder
                .Use<HttpListenerModule>(registeredHandlers, Logger)
                .UseErrorPage();
        }
        #endregion
    }
}
