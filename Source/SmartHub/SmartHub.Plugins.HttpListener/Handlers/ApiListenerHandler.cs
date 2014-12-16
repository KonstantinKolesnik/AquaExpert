using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public class ApiListenerHandler : ListenerHandler
    {
        private readonly Func<HttpRequestParams, object> action;

        public ApiListenerHandler(Func<HttpRequestParams, object> action)
        {
            if (action == null)
            {
                throw new NullReferenceException();
            }

            this.action = action;
        }

        public override bool CacheResponse
        {
            get { return false; }
        }

        public override HttpContent GetContent(HttpRequestParams parameters)
        {
            object result = action(parameters);
            string json = JsonConvert.SerializeObject(result);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }
    }
}
