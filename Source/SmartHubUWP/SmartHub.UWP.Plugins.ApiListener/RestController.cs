using SmartHub.UWP.Core.Communication.Http;
using Windows.Web.Http;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [RoutePrefix("api")]
    class RestController : Controller
    {
        [HttpGet]
        [Route]
        public HttpResponse Get()
        {
            return new HttpResponse(HttpStatusCode.Ok, "Test#1");
        }

        [HttpGet]
        [Route("routedget")]
        public HttpResponse Method()
        {
            return new HttpResponse(HttpStatusCode.Ok, "Test#2");
        }

        [HttpGet]
        [Route("getwithparam/{param1}")]
        public HttpResponse WithParam(string param1)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Test#3:{param1}");
        }

        [HttpPost]
        [Route]
        public HttpResponse Post([Body] string postContent)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Test#4:{postContent}");
        }

        [HttpPost]
        [Route("postwithparam/{param1}")]
        public HttpResponse Post(string param1, [Body] string postContent)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Test#5:{param1}:{postContent}");
        }

        [HttpPost]
        [Route("jsonbody")]
        public HttpResponse JsonPost([JsonBody]string body)
        {
            //return new HttpResponse(HttpStatusCode.Ok, $"received json: {body}");
            return new JsonResponse(body);
        }

        [HttpGet]
        [Route("jsonobject")]
        public HttpResponse JsonObject()
        {
            return new JsonResponse(new { id = 1337, child = new { childprop1 = "testprop", childprop2 = new int[] { 1, 2, 3, 4 } } });
        }

        [HttpGet]
        [Route("gettwoparams/{param1}/{param2}")]
        public HttpResponse TwoParams(string param2, string param1)
        {
            return new HttpResponse(HttpStatusCode.Ok, $"Test8:{param1}:{param2}");
        }
    }
}
