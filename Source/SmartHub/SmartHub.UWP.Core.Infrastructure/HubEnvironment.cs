using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;

namespace SmartHub.UWP.Core.Infrastructure
{
    public static class HubEnvironment
    {
        public static void Init()
        {
            //InitCurrentDirectory();
            InitApplicationCulture();
        }

        private static void InitCurrentDirectory()
        {
            var path = Package.Current.InstalledLocation.Path;
            var currentDirectory = Path.GetDirectoryName(path);

            if (string.IsNullOrWhiteSpace(currentDirectory))
                throw new Exception("Current directory is empty");

            Directory.SetCurrentDirectory(currentDirectory);
        }
        private static void InitApplicationCulture()
        {
            //Thread.CurrentThread.CurrentCulture =
            //Thread.CurrentThread.CurrentUICulture =
            CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            string id = "en-US";
            ApplicationLanguages.PrimaryLanguageOverride = id;

            var lang = new List<string>();
            lang.Add(id);
            ResourceContext.GetForCurrentView().Languages = lang;
        }
    }
}
