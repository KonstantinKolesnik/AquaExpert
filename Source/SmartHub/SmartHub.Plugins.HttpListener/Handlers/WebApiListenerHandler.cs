using Newtonsoft.Json;
using SmartHub.Plugins.HttpListener.Api;
using System;
using System.Net.Http;
using System.Text;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public class WebApiListenerHandler : ListenerHandlerBase
    {
        private readonly Func<HttpRequestParams, object> action;

        public WebApiListenerHandler(Func<HttpRequestParams, object> action)
        {
            if (action == null)
                throw new NullReferenceException("API handler action");

            this.action = action;
        }

        public override bool CacheResponse
        {
            get { return false; }
        }

        public override HttpContent GetResponseContent(HttpRequestParams parameters)
        {
            object result = action(parameters);
            string json = JsonConvert.SerializeObject(result);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
