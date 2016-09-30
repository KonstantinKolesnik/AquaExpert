using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SmartHub.UWP.Core
{
    public static class Utils
    {
        #region Message boxes
        public static async Task<IUICommand> MessageBox(string msg)
        {
            var dlg = new MessageDialog(msg, AppManager.AppName);
            dlg.DefaultCommandIndex = 1;
            return await dlg.ShowAsync();
        }
        public static async Task<IUICommand> MessageBoxOKCancel(string msg, UICommandInvokedHandler onOK = null, UICommandInvokedHandler onCancel = null)
        {
            var dlg = new MessageDialog(msg, AppManager.AppName);
            dlg.DefaultCommandIndex = 1;

            //UICommand btnOK = new UICommand(Labels.OK) { Id = "btnOK" };
            UICommand btnOK = new UICommand("OK") { Id = "btnOK" };
            if (onOK != null)
                btnOK.Invoked = onOK;
            dlg.Commands.Add(btnOK);

            //UICommand btnCancel = new UICommand(Labels.Cancel) { Id = "btnCancel" };
            UICommand btnCancel = new UICommand("Cancel") { Id = "btnOK" };
            if (onCancel != null)
                btnCancel.Invoked = onCancel;
            dlg.Commands.Add(btnCancel);

            return await dlg.ShowAsync();
        }
        public static async Task<IUICommand> MessageBoxYesNo(string msg, UICommandInvokedHandler onYes = null, UICommandInvokedHandler onNo = null)
        {
            var dlg = new MessageDialog(msg, AppManager.AppName);
            dlg.DefaultCommandIndex = 1;

            //UICommand btnYes = new UICommand(Labels.Yes) { Id = "btnYes" };
            UICommand btnYes = new UICommand("Yes") { Id = "btnYes" };
            if (onYes != null)
                btnYes.Invoked = onYes;
            dlg.Commands.Add(btnYes);

            //UICommand btnNo = new UICommand(Labels.No) { Id = "btnNo" };
            UICommand btnNo = new UICommand("No") { Id = "btnNo" };
            if (onNo != null)
                btnNo.Invoked = onNo;
            dlg.Commands.Add(btnNo);

            return await dlg.ShowAsync();
        }
        #endregion

        #region JSON serialization
        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static object FromJson(Type type, string json)
        {
            return JsonConvert.DeserializeObject(json, type);
        }
        public static dynamic FromJson(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static string ToJson(this object obj, string defaultValue = "")
        {
            return obj == null ? defaultValue : JsonConvert.SerializeObject(obj);
        }
        #endregion

        #region Application Data
        //public static T GetAppData<T>(string keyName, T defaultValue, bool isRoaming = true)
        //{
        //    if (!string.IsNullOrEmpty(keyName))
        //    {
        //        var settings = isRoaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
        //        var setting = settings.Values[keyName];

        //        if (setting != null)
        //        {
        //            try
        //            {
        //                return (T) (FromJson(typeof(T), (string) setting));
        //            }
        //            catch (InvalidCastException) { }
        //        }
        //    }

        //    return defaultValue;
        //}
        //public static void SetAppData(string keyName, object value, bool isRoaming = true)
        //{
        //    if (!string.IsNullOrEmpty(keyName))
        //    {
        //        var settings = isRoaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
        //        settings.Values[keyName] = value.ToJson();
        //    }
        //}
        #endregion

        public static TResult GetValueOrDefault<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return obj == null ? default(TResult) : func(obj);
        }
        //public static string GetEnumDescription(this Enum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());
        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        //    if (attributes != null && attributes.Length > 0)
        //        return attributes[0].Description;
        //    else
        //        return value.ToString();
        //}



        public static async Task<string> GETRequest(string uri)
        {
            //Uri geturi = new Uri("http://api.openweathermap.org/data/2.5/weather?q=London");
            Uri geturi = new Uri(uri);

            HttpClient client = new HttpClient();
            HttpResponseMessage responseGet = await client.GetAsync(geturi);
            return await responseGet.Content.ReadAsStringAsync();
        }
        public static async Task<string> POSTRequest(string uri, object data)
        {
            //Uri requestUri = new Uri("https://www.userauth");
            Uri requestUri = new Uri(uri);

            //dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.username = "sureshmit55@gmail.com".ToString();
            //dynamicJson.password = "9442921025";
            //string json = JsonConvert.SerializeObject(dynamicJson);

            string json = JsonConvert.SerializeObject(data);

            var client = new HttpClient();
            HttpResponseMessage respon = await client.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json"));
            return await respon.Content.ReadAsStringAsync();
        }





        //public static IEnumerable<Assembly> GetAssemblies(string path)
        //{
        //    IEnumerable<string> files = Directory.EnumerateFiles(path, "*.dll");
        //    var assemblies = new List<Assembly>();
        //    foreach (var file in files)
        //    {
        //        Assembly assembly = Assembly.LoadFile(file);
        //        assemblies.Add(assembly);
        //    }
        //    return assemblies;
        //}


        //public static string ExecutablePath
        //{
        //    get
        //    {
        //        var exePath = Package.Current.InstalledLocation.Path;
        //        return Path.GetDirectoryName(exePath);
        //    }
        //}

        //public static string ShadowedPluginsFolder
        //{
        //    get { return "ShadowedPlugins"; }
        //}
        //public static string ShadowedPluginsFullPath
        //{
        //    get { return Path.Combine(ExecutablePath, ShadowedPluginsFolder); }
        //}

        //public static string PluginsFolder
        //{
        //    get { return "Plugins"; }
        //}
        //public static string PluginsFullPath
        //{
        //    get { return Path.IsPathRooted(PluginsFolder) ? PluginsFolder : Path.Combine(ExecutablePath, PluginsFolder); }
        //}
    }
}
