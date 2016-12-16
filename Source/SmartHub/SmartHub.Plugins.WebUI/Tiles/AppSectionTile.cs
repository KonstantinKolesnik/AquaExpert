using SmartHub.Plugins.WebUI.Attributes;

namespace SmartHub.Plugins.WebUI.Tiles
{
    /// <summary>
    /// A Tile for section item when its AppSection::TileTypeFullName not defined.
    /// It's created at client side by clicking a section item's shortcut icon.
    /// Has options with the only fields: "title"=sectionItem.Title & "url"=sectionItem.GetModulePath() automatically populated while creating.
    /// See SmartHub.Plugins.WebUI\Resources\Application\sections\list.js\api.addTile method.
    /// </summary>
    [Tile]
    public class AppSectionTile : TileBase
    {
        public override void PopulateWebModel(TileWebModel webTile, dynamic options)
        {
            webTile.title = options.title;
            webTile.url = options.url;
            webTile.className = "btn-primary th-tile-icon th-tile-icon-fa fa-arrow-circle-right";
        }
    }
}
