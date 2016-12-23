using Newtonsoft.Json;

namespace SmartHub.UWP.Core.Communication.Transporting
{
    public static class Transport
    {
        private static JsonSerializerSettings jset = new JsonSerializerSettings()
        {
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Full,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None
        };

        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, typeof(object), jset);
        }
        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, jset);

            //List<JsonConverter> converters = new List<JsonConverter>();
            //converters.Add(new ToolConverter());
            //converters.Add(new CLSToolOperationConverter());
            //return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() { Converters = converters });
        }
    }
}
