using SmartHub.Plugins.WebUI.Model;
using SmartHub.Plugins.WebUI.Tiles;

namespace SmartHub.Plugins.WebUI
{
    [Tile]
    public class AppSectionShortcutTileDefinition : TileDefinition
    {
        public override void FillModel(TileModel model, dynamic options)
        {
            model.url = options.url;
            model.title = options.title;
            model.className = "btn-primary th-tile-icon th-tile-icon-fa fa-arrow-circle-right";
        }
    }
}
