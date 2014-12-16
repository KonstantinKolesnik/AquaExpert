using System.Collections.Generic;

namespace SmartHub.Core.Plugins.Packages
{
    public interface IControllerPackageManager
    {
        List<ControllerPackageInfo> GetPackages(string name);
        List<ControllerPackageInfo> GetInstalledPackages();

        void Install(string packageId);
        void UnInstall(string packageId);
        void Update(string packageId);
    }
}
