using SmartHub.Plugins.HttpListener.Api;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public abstract class ListenerHandlerBase
    {
        public virtual bool CacheResponse
        {
            get { return true; }
        }

        public abstract HttpContent GetResponseContent(HttpRequestParams parameters);

        public HttpResponseMessage ProcessRequest(HttpRequestParams parameters)
        {
            var content = GetResponseContent(parameters);

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
