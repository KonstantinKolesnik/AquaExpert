using NuGet;
using SmartHub.Core.Plugins.HubPackages;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartHub.Core.Infrastructure
{
    [Export(typeof(IHubPackageManager))]
    public class HubPackageManager : IHubPackageManager
    {
        private readonly PackageManager pManager;

        public HubPackageManager()
        {
            IPackageRepository repository = PackageRepositoryFactory.Default.CreateRepository(AppSettings.PluginsRepository);

            pManager = new PackageManager(repository, AppSettings.PluginsFolder);
        }

        #region Private
        private HubPackageInfo MapPackageInfo(IPackage p)
        {
            IPackage dummy = pManager.LocalRepository.FindPackage(p.Id);

            var installedVersion = dummy == null ? null : dummy.Version.ToString();

            return new HubPackageInfo
            {
                PackageId = p.Id,
                PackageVersion = p.Version.ToString(),
                PackageDescription = p.Description,
                InstalledVersion = installedVersion
            };
        }
        #endregion

        public List<HubPackageInfo> GetPackages(string name)
        {
            var query = pManager.SourceRepository.GetPackages();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.GetFullName().Contains(name));

            var packages = query.OrderBy(p => p.Id).ToList();

            var model = packages.Select(MapPackageInfo).ToList();

            return model;
        }
        public List<HubPackageInfo> GetInstalledPackages()
        {
            var packages = pManager.LocalRepository
                .GetPackages()
                .OrderBy(p => p.Id)
                .ToList();

            var model = packages.Select(MapPackageInfo).ToList();

            return model;
        }

        #region Installation
        public void Install(string packageId)
        {
            pManager.InstallPackage(packageId);
        }
        public void UnInstall(string packageId)
        {
            pManager.UninstallPackage(packageId);
        }
        public void Update(string packageId)
        {
            pManager.UpdatePackage(packageId, true, false);
        }
        #endregion
    }
}
