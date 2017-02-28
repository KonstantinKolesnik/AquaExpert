using Newtonsoft.Json;
using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.StringResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace SmartHub.UWP.Core
{
    public static class Utils
    {
        #region UI
        public static string GetLabelValue(string labelId)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(labelId))
            {
                var ctx = new ResourceContext() { Languages = new string[] { AppManager.AppData.Language } };
                ResourceMap rmap = ResourceManager.Current.MainResourceMap.GetSubtree("SmartHub.UWP.Core.StringResources.Labels/");
                if (rmap.ContainsKey(labelId))
                    result = rmap.GetValue(labelId, ctx).ValueAsString;
                else
                    result = labelId;
            }

            return result;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T) child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
        public static T FindFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            return FindVisualChildren<T>(depObj).FirstOrDefault();
        }
        #endregion

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
            dlg.Commands.Add(new UICommand(Labels.OK) { Id = "btnOK", Invoked = onOK });
            dlg.Commands.Add(new UICommand(Labels.Cancel) { Id = "btnCancel", Invoked = onCancel });
            dlg.DefaultCommandIndex = 1;
            dlg.CancelCommandIndex = 0;

            return await dlg.ShowAsync();
        }
        public static async Task<IUICommand> MessageBoxYesNo(string msg, UICommandInvokedHandler onYes = null, UICommandInvokedHandler onNo = null)
        {
            var dlg = new MessageDialog(msg, AppManager.AppName);
            dlg.Commands.Add(new UICommand(Labels.Yes) { Id = "btnYes", Invoked = onYes });
            dlg.Commands.Add(new UICommand(Labels.No) { Id = "btnNo", Invoked = onNo });
            dlg.DefaultCommandIndex = 1;
            dlg.CancelCommandIndex = 0;

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

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion

        #region Application Data
        public static T GetAppData<T>(string keyName, T defaultValue, bool isRoaming = true)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                var settings = isRoaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
                var data = settings.Values[keyName];

                if (data != null)
                {
                    try { return (T) FromJson<T>((string) data); }
                    catch { }

                    try { return (T) data; }
                    catch { }
                }
            }

            return defaultValue;
        }
        public static void SetAppData(string keyName, object value, bool isRoaming = true)
        {
            if (!string.IsNullOrEmpty(keyName))
            {
                var settings = isRoaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
                settings.Values[keyName] = value.ToJson();

                if (isRoaming)
                    ApplicationData.Current.SignalDataChanged();
            }
        }
        #endregion

        #region Composition
        public static async Task<List<Assembly>> GetSatelliteAssembliesAsync(Func<StorageFile, bool> filter)
        {
            //var files = await Package.Current.InstalledLocation.GetFilesAsync();
            //return files
            //    .Where(f => f.DisplayName.StartsWith("SmartHub"))
            //    .Select(f => Assembly.Load(new AssemblyName(f.DisplayName))).ToList();

            List<Assembly> assemblies = new List<Assembly>();

            var files = await Package.Current.InstalledLocation.GetFilesAsync();
            if (files != null)
                foreach (var file in files.Where(filter))
                {
                    try
                    {
                        assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                    }
                    catch (Exception ex)
                    {
                        //System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

            return assemblies;
        }
        public static List<Assembly> GetSatelliteAssemblies(Func<StorageFile, bool> filter)
        {
            List<Assembly> assemblies = new List<Assembly>();

            var files = Package.Current.InstalledLocation.GetFilesAsync();
            files.AsTask().Wait();
            if (files != null)
                foreach (var file in files.GetResults().Where(filter))
                {
                    try
                    {
                        assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                    }
                    catch (Exception ex)
                    {
                        //System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

            return assemblies;
        }
        #endregion

        #region Stream Client
        public static async Task<T> RequestAsync<T>(string commandName, params object[] parameters)
        {
            return await StreamClient.RequestAsync<T>(AppManager.RemoteUrl, AppManager.RemoteServiceName, commandName, parameters);
        }
        public static async Task RequestAsync(string commandName, params object[] parameters)
        {
            await RequestAsync<string>(AppManager.RemoteUrl, AppManager.RemoteServiceName, commandName, parameters);
        }
        #endregion






        public static TResult GetValueOrDefault<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return obj == null ? default(TResult) : func(obj);
        }

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

        public static string ExecutablePath
        {
            get
            {
                var exePath = Package.Current.InstalledLocation.Path;
                return Path.GetDirectoryName(exePath);
            }
        }

        #region Toast notification
        public static void ShowToast(ToastTemplateType templateType, string text)
        {
            //var templateType = ToastTemplateType.ToastText02;
            var xml = ToastNotificationManager.GetTemplateContent(templateType);
            xml.DocumentElement.SetAttribute("launch", "Args");

            var node = xml.CreateTextNode(text);
            var elements = xml.GetElementsByTagName("text");
            elements[0].AppendChild(node);

            var notifier = ToastNotificationManager.CreateToastNotifier();
            var toast = new ToastNotification(xml);
            notifier.Show(toast);
        }
        #endregion
    }
}
