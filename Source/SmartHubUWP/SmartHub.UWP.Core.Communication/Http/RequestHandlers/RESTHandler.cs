using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http.RequestHandlers
{
    public class RESTHandler : IRequestHandler
    {
        private List<Controller> controllers = new List<Controller>();

        public void RegisterController(Controller controller)
        {
            controllers.Add(controller);
        }

        public async Task<HttpResponse> Handle(HttpRequest request)
        {
            var url = request.Path;
            var controller = controllers.SingleOrDefault(c => url.PathAndQuery.Contains(c.Prefix));

            if (controller != null)
                return await controller.Handle(request);
            else
                return new HttpResponse(HttpStatusCode.NotFound, $"No controllers found that support this path: '{ url }'.");
        }
    }
}
