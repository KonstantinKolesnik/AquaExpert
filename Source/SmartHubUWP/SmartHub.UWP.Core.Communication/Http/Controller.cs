using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http
{
    public abstract class Controller
    {
        public string Prefix = "";
        private List<MethodInfo> RoutingMethods = new List<MethodInfo>();

        public Controller()
        {
            var routePrefix = (RoutePrefix)GetType().GetTypeInfo().GetCustomAttribute(typeof(RoutePrefix));
            Prefix = routePrefix.Path;

            var methods = GetType().GetMethods().Where(m => m.GetCustomAttribute(typeof(Route)) != null).ToList();
            RoutingMethods.AddRange(methods);
        }


        public HttpResponse BadRequest(string message = "")
        {
            return new HttpResponse(HttpStatusCode.BadRequest, message);
        }
        public HttpResponse NotFound(string message = "")
        {
            return new HttpResponse(HttpStatusCode.NotFound, message);
        }

        public async Task<HttpResponse> Handle(HttpRequest request)
        {
            var url = request.Path;

            foreach (var route in RoutingMethods)
            {
                var routHttpMethod = ((HttpRequestMethod)route.GetCustomAttribute(typeof(HttpRequestMethod)))?.Method ?? HttpMethod.Get;
                var routPath = RESTPath.Combine(Prefix, ((Route)route.GetCustomAttribute(typeof(Route))).Path);

                bool sameHttpMethod = String.Equals(routHttpMethod.Method, request.Method);
                bool samePath = routPath.Matches(url.AbsolutePath);

                if (sameHttpMethod && samePath)
                {
                    var method = route;
                    var parameters = ExtractParameters(method, routPath, request);

                    if (method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null)
                        return await (Task<HttpResponse>)method.Invoke(this, parameters.ToArray());
                    else
                        return (HttpResponse)method.Invoke(this, parameters.ToArray());
                }
            }

            return NotFound($"Couldn't find a fitting method on the on matched controller '{ GetType().Name }' for path '{ url }'");
        }


        private List<object> ExtractParameters(MethodInfo method, RESTPath path, HttpRequest request)
        {
            var parameters = new List<object>();
            var methodParams = method.GetParameters();
            var requestSegments = request.Path.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var param in methodParams)
            {
                if (param.GetCustomAttribute(typeof(Body)) != null)
                    parameters.Add(request.Content);
                else
                    parameters.Add(requestSegments[path.Parameters.Single(p => p.Value.Equals(param.Name)).Key]);
            }

            return parameters;
        }
    }
}
