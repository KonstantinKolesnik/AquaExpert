using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartHub.Dashboard.Common
{
    public static class Utils
    {
        #region Fields
        private static JsonSerializerSettings dtoSettings = new JsonSerializerSettings()
        {
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,

            //TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };
        #endregion

        public static string DtoSerialize(object data)
        {
            return JsonConvert.SerializeObject(data, typeof(object), dtoSettings);
        }
        public static T DtoDeserialize<T>(string data)
        {
            T result = default(T);

            if (!string.IsNullOrEmpty(data))
                try
                {
                    result = JsonConvert.DeserializeObject<T>(data, dtoSettings);
                }
                catch (Exception ex)
                {
                }

            return result;
        }


        public static async Task<string> GETRequest(string uri)
        {
            //var geturi = new Uri("http://api.openweathermap.org/data/2.5/weather?q=London");
            var geturi = new Uri(uri);

            var client = new HttpClient();
            var responseGet = await client.GetAsync(geturi);
            return await responseGet.Content.ReadAsStringAsync();
        }
        public static async Task<string> POSTRequest(string uri, object data)
        {
            //var requestUri = new Uri("https://www.userauth");
            var requestUri = new Uri(uri);

            //dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.username = "sureshmit55@gmail.com".ToString();
            //dynamicJson.password = "9442921025";
            //string json = JsonConvert.SerializeObject(dynamicJson);

            string json = JsonConvert.SerializeObject(data);

            var client = new HttpClient();
            var response = await client.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
