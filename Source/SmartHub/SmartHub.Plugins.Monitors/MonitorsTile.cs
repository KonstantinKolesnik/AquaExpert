using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Monitors
{
    [Tile]
    public class MonitorsTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Мониторы";
                tileWebModel.url = "webapp/monitors/settings";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-gear";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
