using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SmartHub.UWP.Plugins.Wemos.Monitors.Models;

namespace SmartHub.UWP.Plugins.Wemos.Monitors
{
    public class WemosMonitorDto : WemosMonitor
    {
        public string LineName { get; set; }
        public WemosLineType LineType { get; set; }

        public WemosMonitorDto(WemosMonitor monitor)
        {
            if (monitor != null)
            {
                ID = monitor.ID;
                Name = monitor.Name;
                NameForInformer = monitor.NameForInformer;
                LineID = monitor.LineID;
                Configuration = monitor.Configuration;
            }
        }
    }
}
