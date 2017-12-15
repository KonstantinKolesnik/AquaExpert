using System.Collections.Generic;

namespace SmartHub.Core.Plugins.HubPackages
{
    public interface IHubPackageManager
    {
        List<HubPackageInfo> GetPackages(string name);
        List<HubPackageInfo> GetInstalledPackages();

        void Install(string packageId);
        void UnInstall(string packageId);
        void Update(string packageId);
    }
}
