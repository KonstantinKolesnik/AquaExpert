﻿using System.Configuration;
using System.IO;
using System.Reflection;

namespace SmartNetwork.Core.Infrastructure
{
    public static class AppSettings
    {
        private static string GetStringValue(string name, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[name] ?? defaultValue;
        }

        public static string ShadowedPluginsFolder
        {
            get { return GetStringValue("shadowedPluginsFolder", "ShadowedPlugins"); }
        }
        public static string ShadowedPluginsFullPath
        {
            get { return Path.Combine(ExecutablePath, ShadowedPluginsFolder); }
        }
        public static string PluginsFolder
        {
            get { return GetStringValue("pluginsFolder", "Plugins"); }
        }
        public static string PluginsRepository
        {
            get { return GetStringValue("pluginsRepository", @"D:\nuget"); }
        }
        public static string PluginsFullPath
        {
            get
            {
                return Path.IsPathRooted(PluginsFolder) ? PluginsFolder : Path.Combine(ExecutablePath, PluginsFolder);
            }
        }
        public static string ExecutablePath
        {
            get
            {
                var exePath = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(exePath);
            }
        }
    }
}
