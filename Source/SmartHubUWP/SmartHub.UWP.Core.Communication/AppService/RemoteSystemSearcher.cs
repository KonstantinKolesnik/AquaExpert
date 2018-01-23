using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.System.RemoteSystems;

namespace SmartHub.UWP.Core.Communication.AppService
{
    public class RemoteSystemSearcher
    {
        public static async Task<RemoteSystem> SearchByHostNameAsync(string hostName)
        {
            if (!string.IsNullOrWhiteSpace(hostName))
                if (await RemoteSystem.RequestAccessAsync() == RemoteSystemAccessStatus.Allowed)
                    return await RemoteSystem.FindByHostNameAsync(new HostName(hostName));

            return null;
        }

        //public async void SearchByRemoteSystemWatcher()
        //{
        //    if (await RemoteSystem.RequestAccessAsync() == RemoteSystemAccessStatus.Allowed)
        //    {
        //        RemoteSystemWatcher m_remoteSystemWatcher;

        //        if (FilterSearch.IsChecked.Value)
        //        {
        //            // Build a watcher to continuously monitor for filtered remote systems.
        //            m_remoteSystemWatcher = RemoteSystem.CreateWatcher(BuildFilters());
        //        }
        //        else
        //        {
        //            // Build a watcher to continuously monitor for all remote systems.
        //            m_remoteSystemWatcher = RemoteSystem.CreateWatcher();
        //        }

        //        m_remoteSystemWatcher.RemoteSystemAdded += RemoteSystemWatcher_RemoteSystemAdded;
        //        m_remoteSystemWatcher.RemoteSystemRemoved += RemoteSystemWatcher_RemoteSystemRemoved;
        //        m_remoteSystemWatcher.RemoteSystemUpdated += RemoteSystemWatcher_RemoteSystemUpdated;

        //        m_remoteSystemWatcher.Start();
        //    }
        //}
        //private async void RemoteSystemWatcher_RemoteSystemUpdated(RemoteSystemWatcher sender, RemoteSystemUpdatedEventArgs args)
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        if (m_rootPage.deviceMap.ContainsKey(args.RemoteSystem.Id))
        //        {
        //            m_rootPage.deviceList.Remove(m_rootPage.deviceMap[args.RemoteSystem.Id]);
        //            m_rootPage.deviceMap.Remove(args.RemoteSystem.Id);
        //        }
        //        m_rootPage.deviceList.Add(args.RemoteSystem);
        //        m_rootPage.deviceMap.Add(args.RemoteSystem.Id, args.RemoteSystem);
        //        UpdateStatus("Device updated with Id = " + args.RemoteSystem.Id, NotifyType.StatusMessage);
        //    });
        //}

        //private async void RemoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher sender, RemoteSystemRemovedEventArgs args)
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        if (m_rootPage.deviceMap.ContainsKey(args.RemoteSystemId))
        //        {
        //            m_rootPage.deviceList.Remove(m_rootPage.deviceMap[args.RemoteSystemId]);
        //            UpdateStatus(m_rootPage.deviceMap[args.RemoteSystemId].DisplayName + " removed.", NotifyType.StatusMessage);
        //            m_rootPage.deviceMap.Remove(args.RemoteSystemId);
        //        }
        //    });
        //}

        //private async void RemoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher sender, RemoteSystemAddedEventArgs args)
        //{
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        m_rootPage.deviceList.Add(args.RemoteSystem);
        //        m_rootPage.deviceMap.Add(args.RemoteSystem.Id, args.RemoteSystem);
        //        UpdateStatus(string.Format("Found {0} devices.", m_rootPage.deviceList.Count), NotifyType.StatusMessage);
        //    });
        //}

        //private List<IRemoteSystemFilter> BuildFilters()
        //{
        //    List<IRemoteSystemFilter> filters = new List<IRemoteSystemFilter>();
        //    RemoteSystemDiscoveryTypeFilter discoveryFilter;
        //    List<string> kinds = new List<string>();
        //    RemoteSystemStatusTypeFilter statusFilter;

        //    if (DiscoveryTypeOptions.IsChecked.Value)
        //    {
        //        // Build discovery type filters
        //        if (ProximalRadioButton.IsChecked.Value)
        //        {
        //            discoveryFilter = new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Proximal);
        //        }
        //        else if (CloudRadioButton.IsChecked.Value)
        //        {
        //            discoveryFilter = new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Cloud);
        //        }
        //        else
        //        {
        //            discoveryFilter = new RemoteSystemDiscoveryTypeFilter(RemoteSystemDiscoveryType.Any);
        //        }
        //        filters.Add(discoveryFilter);
        //    }

        //    if (DeviceTypeOptions.IsChecked.Value)
        //    {
        //        // Build device type filters
        //        if (DesktopCheckBox.IsChecked.Value)
        //        {
        //            kinds.Add(RemoteSystemKinds.Desktop);
        //        }
        //        if (HolographicCheckBox.IsChecked.Value)
        //        {
        //            kinds.Add(RemoteSystemKinds.Holographic);
        //        }
        //        if (HubCheckBox.IsChecked.Value)
        //        {
        //            kinds.Add(RemoteSystemKinds.Hub);
        //        }
        //        if (PhoneCheckBox.IsChecked.Value)
        //        {
        //            kinds.Add(RemoteSystemKinds.Phone);
        //        }
        //        if (XboxCheckBox.IsChecked.Value)
        //        {
        //            kinds.Add(RemoteSystemKinds.Xbox);
        //        }
        //        if (kinds.Count == 0)
        //        {
        //            UpdateStatus("Select a Device type filter.", NotifyType.ErrorMessage);
        //        }
        //        else
        //        {
        //            RemoteSystemKindFilter kindFilter = new RemoteSystemKindFilter(kinds);
        //            filters.Add(kindFilter);
        //        }
        //    }

        //    if (StatusTypeOptions.IsChecked.Value)
        //    {
        //        // Build status type filters
        //        if (AvailableRadioButton.IsChecked.Value)
        //        {
        //            statusFilter = new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Available);
        //        }
        //        else
        //        {
        //            statusFilter = new RemoteSystemStatusTypeFilter(RemoteSystemStatusType.Any);
        //        }
        //        filters.Add(statusFilter);
        //    }

        //    return filters;
        //}
    }
}
