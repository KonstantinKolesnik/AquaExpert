using Newtonsoft.Json;
using SmartHub.UWP.Core.Communication.Tcp;
using SmartHub.UWP.Core.StringResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace SmartHub.UWP.Core
{
    public static class CoreUtils
    {
        public static void SetAppLanguage(string languageID)
        {
            //languageID = "en-US";
            //languageID = "de-DE";
            //languageID = "ru-RU";
            //languageID = "uk-UA";

            ApplicationLanguages.PrimaryLanguageOverride = languageID;
            ResourceContext.GetForCurrentView().Languages = new List<string>() { languageID };
        }

        #region Message boxes
        public static async Task<IUICommand> MessageBox(string msg)
        {
            var dlg = new MessageDialog(msg, AppManager.AppName);
            dlg.DefaultCommandIndex = 1;
            return await dlg.ShowAsync();
        }
        //public static async Task<IUICommand> MessageBoxOKCancel(string msg, UICommandInvokedHandler onOK = null, UICommandInvokedHandler onCancel = null)
        //{
        //    var dlg = new MessageDialog(msg, AppManager.AppName);
        //    dlg.Commands.Add(new UICommand(Labels.OK) { Id = "btnOK", Invoked = onOK });
        //    dlg.Commands.Add(new UICommand(Labels.Cancel) { Id = "btnCancel", Invoked = onCancel });
        //    dlg.DefaultCommandIndex = 1;
        //    dlg.CancelCommandIndex = 0;

        //    return await dlg.ShowAsync();
        //}
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

            var assemblies = new List<Assembly>();

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
            var assemblies = new List<Assembly>();

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
            return await StreamClient.RequestAsync<T>(AppManager.RemoteUrl, AppManager.RemoteTcpServiceName, commandName, parameters);
        }
        #endregion

        public static TResult GetValueOrDefault<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return obj == null ? default(TResult) : func(obj);
        }

        public static string ExecutablePath
        {
            get
            {
                var exePath = Package.Current.InstalledLocation.Path;
                return Path.GetDirectoryName(exePath);
            }
        }
        public static async Task<BasicProperties> GetFileBasicPropertiesAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            return await file.GetBasicPropertiesAsync();
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
