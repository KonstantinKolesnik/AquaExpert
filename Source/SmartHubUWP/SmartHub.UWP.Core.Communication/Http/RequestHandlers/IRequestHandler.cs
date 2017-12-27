using System.Threading.Tasks;

namespace SmartHub.UWP.Core.Communication.Http.RequestHandlers
{
    public interface IRequestHandler
    {
        Task<HttpResponse> Handle(HttpRequest request);
    }
}
