using NLog;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Handlers;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace SmartHub.Plugins.HttpListener.Api
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly InternalDictionary<ListenerHandlerBase> handlers;
        private readonly Logger logger;

        public DependencyResolver(InternalDictionary<ListenerHandlerBase> handlers, Logger logger)
        {
            this.handlers = handlers;
            this.logger = logger;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return (serviceType == typeof(CommonController)) ? new CommonController(handlers, logger) : null;
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
        }
    }
}
