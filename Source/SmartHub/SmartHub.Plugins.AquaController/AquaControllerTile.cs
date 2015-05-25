using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.AquaController
{
    [Tile]
    public class AquaControllerTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel webTile, dynamic options)
        {
            try
            {
                webTile.title = "Аква-контроллер";
                webTile.url = "webapp/aquacontroller/module";
                webTile.className = "btn-info th-tile-icon th-tile-icon-fa fa-tachometer";
            }
            catch (Exception ex)
            {
                webTile.content = ex.Message;
            }
        }
    }
}
