using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.MeteoStation
{
    [Tile]
    public class MeteoStationTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel webTile, dynamic options)
        {
            try
            {
                webTile.title = "Метеостанция";
                webTile.url = "/webapp/meteostation/module-main.js";
                webTile.className = "btn-info th-tile-icon th-tile-icon-fa fa-tachometer";
            }
            catch (Exception ex)
            {
                webTile.content = ex.Message;
            }
        }

    }
}
