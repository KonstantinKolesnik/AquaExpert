using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public abstract class ListenerHandler
    {
        public virtual bool CacheResponse
        {
            get { return true; }
        }

        public abstract HttpContent GetContent(HttpRequestParams parameters);

        public HttpResponseMessage ProcessRequest(HttpRequestParams parameters)
        {
            var content = GetContent(parameters);

            var response = new HttpResponseMessage { Content = content };

            if (!CacheResponse)
            {
                response.Headers.CacheControl = new CacheControlHeaderValue { NoStore = true, NoCache = true };
                response.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
            }

            return response;
        }
    }
}
