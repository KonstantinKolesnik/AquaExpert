using Newtonsoft.Json;
using System;

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
    }
}
