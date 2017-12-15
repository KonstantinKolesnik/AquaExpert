using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Tiles;
using System;

namespace SmartHub.Plugins.Controllers
{
    [Tile]
    public class ManagementTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel tileWebModel, dynamic options)
        {
            try
            {
                tileWebModel.title = "Контроллеры";
                tileWebModel.url = "webapp/management/settings";
                tileWebModel.className = "btn-info th-tile-icon th-tile-icon-fa fa-gear";
            }
            catch (Exception ex)
            {
                tileWebModel.content = ex.Message;
            }
        }
    }
}
