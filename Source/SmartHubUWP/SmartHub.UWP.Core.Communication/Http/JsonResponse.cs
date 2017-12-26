using Newtonsoft.Json;
using System;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http
{
    public class JsonResponse : HttpResponse
    {
        #region Constructors
        protected JsonResponse()
            : base()
        {
            Headers.Add("Content-Type", "application/json");
            Headers.Add("Accept", "application/json");
        }
        public JsonResponse(String jsonString, HttpStatusCode statusCode = HttpStatusCode.Ok)
            : this()
        {
            var jsonObject = JsonConvert.DeserializeObject(jsonString);

            StatusCode = statusCode;
            Content = jsonObject.ToString();
        }
        public JsonResponse(object jsonObject, HttpStatusCode statusCode = HttpStatusCode.Ok)
            : this()
        {
            var jsonString = JsonConvert.SerializeObject(jsonObject);

            StatusCode = statusCode;
            Content = jsonString;
        }
        #endregion
    }
}
