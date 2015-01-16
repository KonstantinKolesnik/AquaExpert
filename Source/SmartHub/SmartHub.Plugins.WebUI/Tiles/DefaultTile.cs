using SmartHub.Plugins.WebUI.Attributes;

namespace SmartHub.Plugins.WebUI.Tiles
{
    [Tile]
    public class DefaultTile : TileBase
    {
        public override void FillModel(TileWeb webTile, dynamic options)
        {
            webTile.url = options.url;
            webTile.title = options.title;
            webTile.className = "btn-primary th-tile-icon th-tile-icon-fa fa-arrow-circle-right";
        }
    }
}
