﻿using NHibernate.Linq;
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
    public class WebUiTilesPlugin : PluginBase
    {
        #region Fields
        private InternalDictionary<TileBase> tiles;
        #endregion

        #region Import
        [ImportMany("FA4F97A0-41A0-4A72-BEF3-6DB579D909F4")]
        public TileBase[] Tiles { get; set; }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<Tile>(cfg => cfg.Table("WebUI_Tile"));
        }
        public override void InitPlugin()
        {
            tiles = new InternalDictionary<TileBase>();

            // регистрируем типы плитки
            foreach (var tile in Tiles)
            {
                var key = tile.GetType().FullName;
                tiles.Register(key, tile);
                Logger.Info("Register tile: '{0}'", key);
            }
        }
        #endregion

        #region http api
        [HttpCommand("/api/webui/tiles")]
        public object GetTiles(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                var result = new List<TileWeb>();

                var dbTiles = session.Query<Tile>().OrderBy(t => t.SortOrder).ToList();

                foreach (var obj in dbTiles)
                {
                    TileBase def;
                    if (tiles.TryGetValue(obj.HandlerKey, out def))
                    {
                        var model = new TileWeb(obj.Id);

                        try
                        {
                            var options = obj.GetParameters();
                            def.FillModel(model, options);
                        }
                        catch (Exception ex)
                        {
                            model.content = ex.Message;
                        }

                        result.Add(model);
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
                var tile = session.Load<Tile>(id);
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
                var tile = session.Get<Tile>(id);
                TileBase def;

                if (tiles.TryGetValue(tile.HandlerKey, out def))
                {
                    var options = tile.GetParameters();
                    return def.ExecuteAction(options);
                }
            }

            return null;
        }

        [HttpCommand("/api/webui/tiles/add")]
        public object AddTile(HttpRequestParams request)
        {
            var strDef = request.GetRequiredString("def");
            var strOptions = request.GetString("options");

            AddTile(strDef, strOptions);

            return null;
        }

        [HttpCommand("/api/webui/tiles/sort")]
        public object UpdateSortOrder(HttpRequestParams request)
        {
            var json = request.GetRequiredString("data");
            var ids = Extensions.FromJson<Guid[]>(json);

            using (var session = Context.OpenSession())
            {
                var tiles = session.Query<Tile>().ToList();

                for (int i = 0; i < ids.Length; i++)
                {
                    var tile = tiles.FirstOrDefault(t => t.Id == ids[i]);

                    if (tile != null)
                    {
                        tile.SortOrder = i;
                    }
                }

                session.Flush();
            }

            return null;
        }
        #endregion

        #region api
        public void AddTile<TDef>(object options)
        {
            AddTile(typeof(TDef), options);
        }
        public void AddTile(Type defType, object options)
        {
            var key = defType.FullName;
            var strOptions = options.ToJson();
            AddTile(key, strOptions);
        }
        internal void AddTile(string key, string strOptions)
        {
            TileBase def;

            if (!tiles.TryGetValue(key, out def))
                throw new Exception(string.Format("Invalid tile definition: {0}", key));

            using (var session = Context.OpenSession())
            {
                var lastTile = session.Query<Tile>()
                    .OrderByDescending(t => t.SortOrder)
                    .FirstOrDefault();

                int sortOrder = lastTile == null ? 0 : lastTile.SortOrder + 1;

                var tile = new Tile
                {
                    Id = Guid.NewGuid(),
                    HandlerKey = key,
                    SortOrder = sortOrder,
                    SerializedParameters = strOptions
                };

                session.Save(tile);
                session.Flush();
            }
        }
        #endregion
    }
}
