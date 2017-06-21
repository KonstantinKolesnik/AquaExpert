using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.UI.Models;
using SmartHub.UWP.Plugins.UI.Tiles;
using System.Collections.Generic;
using System.Composition;

namespace SmartHub.UWP.Plugins.UI
{
    [Plugin]
    public class UITilesPlugin : PluginBase
    {
        #region Fields
        private readonly Dictionary<string, TileBase> registeredTiles = new Dictionary<string, TileBase>();
        #endregion

        #region Import
        [ImportMany]
        public IEnumerable<TileBase> PluginsTiles
        {
            get; set;
        }
        #endregion

        #region Plugin overrides
        public override void InitDbModel()
        {
            //using (var db = Context.OpenConnection())
            //    db.CreateTable<TileDB>();
        }
        public override void InitPlugin()
        {
            foreach (var tile in PluginsTiles)
                registeredTiles.Add(tile.GetType().FullName, tile);
        }
        #endregion

        #region DB
        //public void AddTile<TDef>(object parameters)
        //{
        //    AddTile(typeof(TDef), parameters);
        //}
        //public void AddTile(Type type, object parameters)
        //{
        //    AddTile(type.FullName, parameters.ToJson());
        //}
        private void AddTile(string typeFullName, string parameters)
        {
            if (registeredTiles.ContainsKey(typeFullName))
                using (var db = Context.StorageOpen())
                {
                    var lastDbTile = db.Table<TileDB>().OrderByDescending(t => t.SortOrder).FirstOrDefault();
                    int nextSortOrder = lastDbTile == null ? 0 : lastDbTile.SortOrder + 1;

                    var dbTile = new TileDB
                    {
                        TypeFullName = typeFullName,
                        SortOrder = nextSortOrder,
                        SerializedParameters = parameters
                    };

                    db.InsertOrReplace(dbTile);
                }
        }
        #endregion
    }
}
