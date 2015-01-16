using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NLog;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using SmartHub.Plugins.WebUI.Data;
using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartHub.Plugins.WebUI
{
    // webapp: tiles
    [JavaScriptResource("/application/tiles/tiles.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles.js")]
    [JavaScriptResource("/application/tiles/tiles-view.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-view.js")]
    [JavaScriptResource("/application/tiles/tiles-model.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-model.js")]
    [HttpResource("/application/tiles/tile.tpl", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tile.tpl")]
    [HttpResource("/application/tiles/tiles.tpl", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles.tpl")]

    // webapp: tiles-edit-mode
    [JavaScriptResource("/application/tiles/tiles-edit-mode.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-edit-mode.js")]
    [JavaScriptResource("/application/tiles/tiles-edit-mode-view.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-edit-mode-view.js")]
    [JavaScriptResource("/application/tiles/tiles-edit-mode-model.js", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-edit-mode-model.js")]
    [HttpResource("/application/tiles/tile-edit-mode.tpl", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tile-edit-mode.tpl")]
    [HttpResource("/application/tiles/tiles-edit-mode.tpl", "SmartHub.Plugins.WebUI.Resources.Application.tiles.tiles-edit-mode.tpl")]

    [Plugin]
    public class WebUITilesPlugin : PluginBase
    {
        #region Fields
        private InternalDictionary<TileBase> registeredTiles;
        #endregion

        #region Import
        [ImportMany("FA4F97A0-41A0-4A72-BEF3-6DB579D909F4")]
        public TileBase[] Tiles { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<TileDB>(cfg => cfg.Table("WebUI_Tiles"));
        }
        public override void InitPlugin()
        {
            registeredTiles = new InternalDictionary<TileBase>();

            // регистрируем типы плиток
            foreach (var tile in Tiles)
            {
                var typeFullName = tile.GetType().FullName;
                registeredTiles.Register(typeFullName, tile);
                
                Logger.Info("Register tile: '{0}'", typeFullName);
            }
        }
        #endregion

        #region Web API
        [HttpCommand("/api/webui/tiles")]
        public object GetWebTiles(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                var result = new List<TileWeb>();

                var dbTiles = session.Query<TileDB>().OrderBy(t => t.SortOrder).ToList();
                foreach (var dbTile in dbTiles)
                {
                    TileBase tile;
                    if (registeredTiles.TryGetValue(dbTile.HandlerKey, out tile))
                    {
                        var webTile = new TileWeb(dbTile.Id);

                        try
                        {
                            var parameters = dbTile.GetParameters();
                            tile.FillModel(webTile, parameters);
                        }
                        catch (Exception ex)
                        {
                            webTile.content = ex.Message;
                        }

                        result.Add(webTile);
                    }
                }

                return result.ToArray();
            }
        }

        [HttpCommand("/api/webui/tiles/delete")]
        public object DeleteTile(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
            {
                var tile = session.Load<TileDB>(id);
                session.Delete(tile);
                session.Flush();
            }

            return null;
        }

        [HttpCommand("/api/webui/tiles/action")]
        public object RunTileAction(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
            {
                var dbTile = session.Get<TileDB>(id);
                
                TileBase tile;
                if (registeredTiles.TryGetValue(dbTile.HandlerKey, out tile))
                {
                    var options = dbTile.GetParameters();
                    return tile.ExecuteAction(options);
                }
            }

            return null;
        }

        [HttpCommand("/api/webui/tiles/add")]
        public object AddTile(HttpRequestParams request)
        {
            var typeFullName = request.GetRequiredString("typeFullName");
            var parameters = request.GetString("parameters");

            AddTile(typeFullName, parameters);

            return null;
        }

        [HttpCommand("/api/webui/tiles/sort")]
        public object UpdateSortOrder(HttpRequestParams request)
        {
            var json = request.GetRequiredString("data");
            var ids = Extensions.FromJson<Guid[]>(json);

            using (var session = Context.OpenSession())
            {
                var dbTiles = session.Query<TileDB>().ToList();

                for (int i = 0; i < ids.Length; i++)
                {
                    var dbTile = dbTiles.FirstOrDefault(t => t.Id == ids[i]);
                    if (dbTile != null)
                        dbTile.SortOrder = i;
                }

                session.Flush();
            }

            return null;
        }
        #endregion

        #region DB API
        public void AddTile<TDef>(object parameters)
        {
            AddTile(typeof(TDef), parameters);
        }
        public void AddTile(Type type, object parameters)
        {
            AddTile(type.FullName, parameters.ToJson());
        }
        internal void AddTile(string typeFullName, string parameters)
        {
            TileBase tile;
            if (!registeredTiles.TryGetValue(typeFullName, out tile))
                throw new Exception(string.Format("Invalid tile type name: {0}", typeFullName));

            using (var session = Context.OpenSession())
            {
                var lastDbTile = session.Query<TileDB>().OrderByDescending(t => t.SortOrder).FirstOrDefault();
                int sortOrder = lastDbTile == null ? 0 : lastDbTile.SortOrder + 1;

                var dbTile = new TileDB
                {
                    Id = Guid.NewGuid(),
                    HandlerKey = typeFullName,
                    SortOrder = sortOrder,
                    SerializedParameters = parameters
                };

                session.Save(dbTile);
                session.Flush();
            }
        }
        #endregion
    }
}
