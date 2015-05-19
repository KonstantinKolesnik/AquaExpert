using SmartHub.Plugins.WebUI.Attributes;

namespace SmartHub.Plugins.WebUI.Tiles
{
    [Tile]
    public class DefaultTile : TileBase
    {
        public override void PopulateModel(TileWeb webTile, dynamic options)
        {
            webTile.title = options.title;
            webTile.url = options.url;
            webTile.className = "btn-primary th-tile-icon th-tile-icon-fa fa-arrow-circle-right";
        }
    }
}
