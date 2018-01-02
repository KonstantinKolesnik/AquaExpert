using SmartHub.UWP.Plugins.Lines.Models;
using SmartHub.UWP.Plugins.Wemos.Infrastructure.Monitors.Models;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Monitors
{
    public class WemosLineMonitorDto : WemosLineMonitor
    {
        public string LineName
        {
            get; set;
        }
        public LineType LineType
        {
            get; set;
        }

        public WemosLineMonitorDto(WemosLineMonitor model)
        {
            if (model != null)
            {
                ID = model.ID;
                LineID = model.LineID;
                Factor = model.Factor;
                Offset = model.Offset;
                Units = model.Units;
                Min = model.Min;
                Max = model.Max;
                Precision = model.Precision;
                ValuesCount = model.ValuesCount;
            }
        }
    }
}
