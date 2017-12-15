using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.HubPackages;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System.Linq;

namespace SmartHub.Plugins.Packages
{
    [AppSection("Пакеты", SectionType.System, "/webapp/packages/list.js", "SmartHub.Plugins.Packages.Resources.list.js")]
    [JavaScriptResource("/webapp/packages/list-model.js", "SmartHub.Plugins.Packages.Resources.list-model.js")]
    [JavaScriptResource("/webapp/packages/list-view.js", "SmartHub.Plugins.Packages.Resources.list-view.js")]
    [HttpResource("/webapp/packages/list-item.tpl", "SmartHub.Plugins.Packages.Resources.list-item.tpl")]
    [HttpResource("/webapp/packages/list.tpl", "SmartHub.Plugins.Packages.Resources.list.tpl")]
    [Plugin]
    public class PackagesPlugin : PluginBase
    {
        #region Web API
        [HttpCommand("/api/packages/list")]
        public object GetPackages(HttpRequestParams request)
        {
            string query = request.GetString("query");

            var list = Context.PackageManager.GetPackages(query);

            return list.Select(BuildModel).Where(x => x != null).ToArray();
        }

        [HttpCommand("/api/packages/installed")]
        public object GetInstalledPackages(HttpRequestParams request)
        {
            var list = Context.PackageManager.GetInstalledPackages();
            return list.Select(BuildModel).Where(x => x != null).ToArray();
        }

        [HttpCommand("/api/packages/install")]
        public object Install(HttpRequestParams request)
        {
            string packageId = request.GetRequiredString("packageId");

            Context.PackageManager.Install(packageId);
            return null;
        }

        [HttpCommand("/api/packages/update")]
        public object Update(HttpRequestParams request)
        {
            string packageId = request.GetRequiredString("packageId");

            Context.PackageManager.Update(packageId);
            return null;
        }

        [HttpCommand("/api/packages/uninstall")]
        public object UnInstall(HttpRequestParams request)
        {
            string packageId = request.GetRequiredString("packageId");

            Context.PackageManager.UnInstall(packageId);
            return null;
        }
        #endregion

        #region Private methods
        private static object BuildModel(HubPackageInfo packageInfo)
        {
            if (packageInfo == null)
                return null;

            return new
            {
                id = packageInfo.PackageId,
                version = packageInfo.PackageVersion,
                description = packageInfo.PackageDescription,
                installedVersion = packageInfo.InstalledVersion
            };
        }
        #endregion
    }
}
