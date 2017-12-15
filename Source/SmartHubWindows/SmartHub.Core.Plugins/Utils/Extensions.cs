using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;

namespace SmartHub.Core.Plugins.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Сериализация в JSON
        /// </summary>
        public static string ToJson(this object obj, string defaultValue = "")
        {
            return obj == null ? defaultValue : JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Десериализация
        /// </summary>
        /// <typeparam name="T">Тип</typeparam>
        /// <param name="json">Строка JSON</param>
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Десериализация
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="json">Строка JSON</param>
        public static object FromJson(Type type, string json)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static dynamic FromJson(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static TResult GetValueOrDefault<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return obj == null ? default(TResult) : func(obj);
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
