using SmartHub.Plugins.HttpListener.Api;
using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.HttpListener.Attributes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpCommandAttribute : ExportAttribute, IHttpCommandAttribute
    {
        public HttpCommandAttribute(string url)
            : base("5D358D8E-2310-49FE-A660-FB3ED7003B4C", typeof(Func<HttpRequestParams, object>))
        {
            Url = url;
        }

        public string Url { get; private set; }
    }
}
