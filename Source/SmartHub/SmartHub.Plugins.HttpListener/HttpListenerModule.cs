using Microsoft.Owin;
using NLog;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHub.Plugins.HttpListener
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class HttpListenerModule
    {
        #region Fields
		private readonly InternalDictionary<IListenerHandler> handlers;
		private readonly Logger logger;
		private readonly AppFunc next;
        #endregion

        #region Constructor
        public HttpListenerModule(AppFunc next, InternalDictionary<IListenerHandler> handlers, Logger logger)
        {
            if (next == null)
                throw new ArgumentNullException("next");

            this.next = next;
            this.handlers = handlers;
            this.logger = logger;
        }
        #endregion

        #region Public methods
        public Task Invoke(IDictionary<string, object> env)
        {
            try
            {
                var request = new OwinRequest(env);
                var path = request.Path.ToString();

                logger.Info("Execute action: {0};", path);

                IListenerHandler handler;

                if (handlers.TryGetValue(path, out handler))
                {
                    //var message = string.Format("handler for url '{0}' is not found", localPath);
                    return handler.ProcessRequest(request);
                }
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetException(ex);
                logger.ErrorException("", ex);
                return tcs.Task;
            }

            return next(env);
        }
        #endregion
    }
}
