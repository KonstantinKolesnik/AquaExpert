using Newtonsoft.Json;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http
{
    public class JsonResponse : HttpResponse
    {
        #region Constructors
        protected JsonResponse()
            : base()
        {
            Headers.Add("Content-Type", "application/json; charset=utf-8");
            Headers.Add("Accept", "application/json");
        }
        public JsonResponse(string jsonString, HttpStatusCode statusCode = HttpStatusCode.Ok)
            : this()
        {
            StatusCode = statusCode;
            Content = jsonString;
        }
        public JsonResponse(object jsonObject, HttpStatusCode statusCode = HttpStatusCode.Ok)
            : this(JsonConvert.SerializeObject(jsonObject), statusCode)
        {
        }
        #endregion
    }
}
