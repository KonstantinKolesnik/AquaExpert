using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;

namespace SmartHub.Plugins.HttpListener
{
    [Plugin]
    public class HttpListenerPlugin : PluginBase
    {
        #region Fields
        private const string BASE_URL_HTTP = "http://localhost:41831";
        private HttpSelfHostServer server;
        #endregion


        [ImportMany("5D358D8E-2310-49FE-A660-FB3ED7003B4C")]
        public Lazy<Func<HttpRequestParams, object>, IHttpCommandAttribute>[] RequestReceived { get; set; }


        #region Plugin overrides
        public override void InitPlugin()
        {
            var handlers = RegisterHandlers();
            var dependencyResolver = new DependencyResolver(handlers, Logger);
            var config = BuildConfiguration(dependencyResolver);

            server = new HttpSelfHostServer(config);
        }
        public override void StartPlugin()
        {
            server.OpenAsync();
        }
        public override void StopPlugin()
        {
            server.Dispose();
            server = null;
        }
        #endregion

        #region Private methods
        private InternalDictionary<ListenerHandler> RegisterHandlers()
        {
            var handlers = new InternalDictionary<ListenerHandler>();

            // регистрируем обработчики для методов плагинов
            foreach (var action in RequestReceived)
            {
                Logger.Info("Register HTTP handler (API): '{0}'", action.Metadata.Url);

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
                    Logger.Info("Register HTTP handler (resource): '{0}'", attribute.Url);

                    var resHandler = new ResourceListenerHandler(
                        type.Assembly, attribute.ResourcePath, attribute.ContentType);

                    handlers.Register(attribute.Url, resHandler);
                }
            }

            return handlers;
        }

        #endregion
    }
}
