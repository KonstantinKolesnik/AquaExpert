using System.Threading.Tasks;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http.RequestHandlers
{
    public class PlainHandler : IRequestHandler
    {
        public ApiRequestHandler ApiRequestHandler
        {
            get; set;
        }

        public async Task<HttpResponse> Handle(HttpRequest request)
        {
            var args = CommunucationUtils.DtoDeserialize<object[]>(request.Content);
            var apiRequest = new ApiRequest(request.Path.AbsolutePath, args);

            var response = ApiRequestHandler?.Invoke(apiRequest);
            if (response != null)
            {
                var responseDto = CommunucationUtils.DtoSerialize(response);
                return new JsonResponse(responseDto);
            }
            else
                return new HttpResponse(HttpStatusCode.NotFound, $"No api method found that supports this path: '{ request.Path }'.");
        }
    }
}
