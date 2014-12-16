using NLog;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Handlers;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SmartHub.Plugins.HttpListener
{
    public class CommonController : ApiController
    {
        private readonly InternalDictionary<ListenerHandler> handlers;
        private readonly Logger logger;

        public CommonController(InternalDictionary<ListenerHandler> handlers, Logger logger)
        {
            this.handlers = handlers;
            this.logger = logger;
        }

        [HttpGet, HttpPost, HttpPut, HttpDelete]
        public HttpResponseMessage Index()
        {
            try
            {
                // Debugger.Launch();
                string localPath = Request.RequestUri.LocalPath;

                logger.Info("execute action: {0};", localPath);

                ListenerHandler handler;

                if (!handlers.TryGetValue(localPath, out handler))
                {
                    var message = string.Format("handler for url '{0}' is not found", localPath);
                    throw new Exception(message);
                }

                HttpRequestParams parameters = GetRequestParams(Request);
                HttpResponseMessage response = handler.ProcessRequest(parameters);

                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
        }

        private static HttpRequestParams GetRequestParams(HttpRequestMessage request)
        {
            var urlData = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var formData = new NameValueCollection();

            if (request.Content.IsFormData())
            {
                var task = request.Content.ReadAsFormDataAsync();
                task.Wait();
                formData = task.Result;
            }

            return new HttpRequestParams(urlData, formData);
        }
    }
}
