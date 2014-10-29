using Griffin.WebServer;
using Griffin.WebServer.Modules;
using System;

namespace MySensors.Core.Services.Web
{
    class RootModule : IWorkerModule
    {
        public void BeginRequest(IHttpContext context)
        {

        }
        public void EndRequest(IHttpContext context)
        {

        }

        public void HandleRequestAsync(IHttpContext context, Action<IAsyncModuleResult> callback)
        {
            // Since this module only supports sync
            callback(new AsyncModuleResult(context, HandleRequest(context)));
        }

        public ModuleResult HandleRequest(IHttpContext context)
        {
            if (context.Request.Uri.LocalPath == "/")
                context.Request.Uri = new Uri(context.Request.Uri, "index.html");
                //context.Request.Uri = new Uri(context.Request.Uri, "desktop.html");

            //context.Response.AddHeader("Expires:", "1");

            return ModuleResult.Continue;
        }
    }
}
