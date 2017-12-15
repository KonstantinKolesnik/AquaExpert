using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Zones
{
    [Tile]
    public class ZonesTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Зоны";
                tileWebModel.url = "webapp/zones/dashboard";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-th";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
