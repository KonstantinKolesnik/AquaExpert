using SmartHub.Plugins.HttpListener.Attributes;

namespace SmartHub.Plugins.WebUI.Attributes
{
    public class CssResourceAttribute : HttpResourceAttribute
    {
        public bool AutoLoad { get; set; }

        public CssResourceAttribute(string url, string resourcePath)
            : base(url, resourcePath, "text/css")
        {
        }
    }
}
