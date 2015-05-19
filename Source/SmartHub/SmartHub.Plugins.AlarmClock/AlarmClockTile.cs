using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Linq;

namespace SmartHub.Plugins.AlarmClock
{
    [Tile]
    public class AlarmClockTile : TileBase
    {
        public override void PopulateModel(TileWeb webTile, dynamic options)
        {
            webTile.title = "Оповещения";
            webTile.url = "webapp/alarm-clock/list";
            webTile.content = GetAlarmTileContent();
            webTile.cssClassName = "btn-primary th-tile-icon th-tile-icon-fa fa-bell";
        }

        private string GetAlarmTileContent()
        {
            var now = DateTime.Now;

            //var times = Context.GetPlugin<AlarmClockPlugin>().GetNextAlarmTimes(now).Take(4);
            //var strTimes = times.Select(t => t.ToShortTimeString()).ToArray();

            var strTimes = Context.GetPlugin<AlarmClockPlugin>().GetNextAlarmsTimesAndNames(now).Take(6);
            
            return strTimes.Any() ? string.Join(Environment.NewLine, strTimes) : "Нет активных оповещений";
        }
    }
}
