using System.Threading.Tasks;

namespace SmartHub.UWP.Core.Communication.Http
{
    public abstract class RequestHandler
    {
        public abstract Task<HttpResponse> Handle(HttpRequest request);
    }
}
