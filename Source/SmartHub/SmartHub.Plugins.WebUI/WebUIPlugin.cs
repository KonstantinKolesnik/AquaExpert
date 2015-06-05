using SmartHub.Core.Plugins;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmartHub.Plugins.WebUI
{
    #region Vendor resources
    [JavaScriptResource("/vendor/js/require.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.require.min.js")]
    [JavaScriptResource("/vendor/js/require-text.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.require-text.js")]
    [JavaScriptResource("/vendor/js/require-json.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.require-json.js")]

    [JavaScriptResource("/vendor/js/json2.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.json2.min.js")]
    [JavaScriptResource("/vendor/js/jquery.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.jquery.min.js")]

    [JavaScriptResource("/vendor/js/underscore.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.underscore.min.js")]
    [HttpResource("/vendor/js/underscore.min.js.map", "SmartHub.Plugins.WebUI.Resources.Vendor.js.underscore.min.js.map", "application/json")]

    [JavaScriptResource("/vendor/js/backbone.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.backbone.min.js")]
    [HttpResource("/vendor/js/backbone.min.js.map", "SmartHub.Plugins.WebUI.Resources.Vendor.js.backbone.min.js.map", "application/json")]

    [JavaScriptResource("/vendor/js/backbone.marionette.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.backbone.marionette.min.js")]
    [HttpResource("/vendor/js/backbone.marionette.min.js.map", "SmartHub.Plugins.WebUI.Resources.Vendor.js.backbone.marionette.min.js.map", "application/json")]

    [JavaScriptResource("/vendor/js/backbone.syphon.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.backbone.syphon.js")]
    [JavaScriptResource("/vendor/js/bootstrap.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.bootstrap.min.js")]
    [JavaScriptResource("/vendor/js/moment.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.moment.min.js")]

    [JavaScriptResource("/vendor/js/codemirror-all.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror-all.js")]
    [JavaScriptResource("/vendor/js/codemirror.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror.js")]
    [JavaScriptResource("/vendor/js/codemirror-javascript.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror-javascript.js")]
    [JavaScriptResource("/vendor/js/codemirror-closebrackets.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror-closebrackets.js")]
    [JavaScriptResource("/vendor/js/codemirror-matchbrackets.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror-matchbrackets.js")]
    [JavaScriptResource("/vendor/js/codemirror-fullscreen.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.codemirror-fullscreen.js")]

    [JavaScriptResource("/vendor/js/chart.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.chart.min.js")]
    [HttpResource("/vendor/js/chart.min.js.map", "SmartHub.Plugins.WebUI.Resources.Vendor.js.chart.min.js.map", "application/json")]
    [JavaScriptResource("/vendor/js/chart.scatter.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.chart.scatter.min.js")]
    [HttpResource("/vendor/js/chart.scatter.min.js.map", "SmartHub.Plugins.WebUI.Resources.Vendor.js.chart.scatter.min.js.map", "application/json")]

    [JavaScriptResource("/vendor/js/kendo.all.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.kendo.all.min.js")]
    [JavaScriptResource("/vendor/js/jquery.signalR-2.1.2.min.js", "SmartHub.Plugins.WebUI.Resources.Vendor.js.jquery.signalR-2.1.2.min.js")]

    // fonts
    [HttpResource("/vendor/fonts/glyphicons-halflings-regular.eot", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.glyphicons-halflings-regular.eot", "application/vnd.ms-fontobject")]
    [HttpResource("/vendor/fonts/glyphicons-halflings-regular.svg", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.glyphicons-halflings-regular.svg", "image/svg+xml")]
    [HttpResource("/vendor/fonts/glyphicons-halflings-regular.ttf", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.glyphicons-halflings-regular.ttf", "application/x-font-truetype")]
    [HttpResource("/vendor/fonts/glyphicons-halflings-regular.woff", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.glyphicons-halflings-regular.woff", "application/font-woff")]
    [HttpResource("/vendor/fonts/glyphicons-halflings-regular.woff2", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.glyphicons-halflings-regular.woff2", "application/font-woff2")]

    [HttpResource("/vendor/fonts/fontawesome-webfont.eot", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.fontawesome-webfont.eot", "application/vnd.ms-fontobject")]
    [HttpResource("/vendor/fonts/fontawesome-webfont.svg", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.fontawesome-webfont.svg", "image/svg+xml")]
    [HttpResource("/vendor/fonts/fontawesome-webfont.ttf", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.fontawesome-webfont.ttf", "application/x-font-truetype")]
    [HttpResource("/vendor/fonts/fontawesome-webfont.woff", "SmartHub.Plugins.WebUI.Resources.Vendor.fonts.fontawesome-webfont.woff", "application/font-woff")]

    // css
    [CssResource("/vendor/css/bootstrap.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.bootstrap.min.css")]
    [CssResource("/vendor/css/font-awesome.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.font-awesome.min.css")]
    [CssResource("/vendor/css/site.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.site.css")]
    [CssResource("/vendor/css/codemirror.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.codemirror.css", AutoLoad = true)]



    // kendo
    [CssResource("/vendor/css/kendo.common.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.kendo.common.min.css")]
    [CssResource("/vendor/css/kendo.dataviz.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.kendo.dataviz.min.css")]

    [HttpResource("/vendor/css/images/back.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.back.png")]
    [HttpResource("/vendor/css/images/kendoui.svg", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.kendoui.svg")]
    [HttpResource("/vendor/css/images/kendoui.ttf", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.kendoui.ttf")]
    [HttpResource("/vendor/css/images/kendoui.woff", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.kendoui.woff")]
    [HttpResource("/vendor/css/images/wp8_icons.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.wp8_icons.png")]
    [HttpResource("/vendor/css/images/wp8_inverseicons.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.images.wp8_inverseicons.png")]

    [CssResource("/vendor/css/kendo.default.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.kendo.default.min.css")]
    [HttpResource("/vendor/css/Default/editor.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.editor.png")]
    [HttpResource("/vendor/css/Default/imagebrowser.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.imagebrowser.png")]
    [HttpResource("/vendor/css/Default/indeterminate.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.indeterminate.gif")]
    [HttpResource("/vendor/css/Default/loading-image.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.loading-image.gif")]
    [HttpResource("/vendor/css/Default/loading.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.loading.gif")]
    [HttpResource("/vendor/css/Default/loading_2x.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.loading_2x.gif")]
    [HttpResource("/vendor/css/Default/markers.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.markers.png")]
    [HttpResource("/vendor/css/Default/markers_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.markers_2x.png")]
    [HttpResource("/vendor/css/Default/slider-h.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.slider-h.gif")]
    [HttpResource("/vendor/css/Default/slider-v.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.slider-v.gif")]
    [HttpResource("/vendor/css/Default/sprite.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.sprite.png")]
    [HttpResource("/vendor/css/Default/sprite_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.sprite_2x.png")]
    [HttpResource("/vendor/css/Default/sprite_kpi.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.sprite_kpi.png")]
    [HttpResource("/vendor/css/Default/sprite_kpi_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Default.sprite_kpi_2x.png")]

    [CssResource("/vendor/css/kendo.black.min.css", "SmartHub.Plugins.WebUI.Resources.Vendor.css.kendo.black.min.css")]
    [HttpResource("/vendor/css/Black/editor.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.editor.png")]
    [HttpResource("/vendor/css/Black/imagebrowser.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.imagebrowser.png")]
    [HttpResource("/vendor/css/Black/indeterminate.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.indeterminate.gif")]
    [HttpResource("/vendor/css/Black/loading-image.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.loading-image.gif")]
    [HttpResource("/vendor/css/Black/loading.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.loading.gif")]
    [HttpResource("/vendor/css/Black/loading_2x.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.loading_2x.gif")]
    [HttpResource("/vendor/css/Black/markers.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.markers.png")]
    [HttpResource("/vendor/css/Black/markers_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.markers_2x.png")]
    [HttpResource("/vendor/css/Black/slider-h.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.slider-h.gif")]
    [HttpResource("/vendor/css/Black/slider-v.gif", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.slider-v.gif")]
    [HttpResource("/vendor/css/Black/sprite.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.sprite.png")]
    [HttpResource("/vendor/css/Black/sprite_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.sprite_2x.png")]
    [HttpResource("/vendor/css/Black/sprite_kpi.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.sprite_kpi.png")]
    [HttpResource("/vendor/css/Black/sprite_kpi_2x.png", "SmartHub.Plugins.WebUI.Resources.Vendor.css.Black.sprite_kpi_2x.png")]
    #endregion

    #region Application resources
    // html
    [HttpResource("/", "SmartHub.Plugins.WebUI.Resources.Application.index.html", "text/html")]
    [HttpResource("/favicon.ico", "SmartHub.Plugins.WebUI.Resources.Application.favicon.ico", "image/x-icon")]

    // webapp: main
    [JavaScriptResource("/application/index.js", "SmartHub.Plugins.WebUI.Resources.Application.index.js")]
    [JavaScriptResource("/application/app.js", "SmartHub.Plugins.WebUI.Resources.Application.app.js")]
    [JavaScriptResource("/application/common.js", "SmartHub.Plugins.WebUI.Resources.Application.common.js")]
    [JavaScriptResource("/application/lib.js", "SmartHub.Plugins.WebUI.Resources.Application.lib.js")]

    [JavaScriptResource("/application/common/sortable-view.js", "SmartHub.Plugins.WebUI.Resources.Application.common.sortable-view.js")]
    [JavaScriptResource("/application/common/complex-view.js", "SmartHub.Plugins.WebUI.Resources.Application.common.complex-view.js")]
    [JavaScriptResource("/application/common/form-view.js", "SmartHub.Plugins.WebUI.Resources.Application.common.form-view.js")]
    [JavaScriptResource("/application/common/utils.js", "SmartHub.Plugins.WebUI.Resources.Application.common.utils.js")]

    // webapp: sections
    [JavaScriptResource("/application/sections/system.js", "SmartHub.Plugins.WebUI.Resources.Application.sections.system.js")]
    [JavaScriptResource("/application/sections/user.js", "SmartHub.Plugins.WebUI.Resources.Application.sections.user.js")]

    [JavaScriptResource("/application/sections/list.js", "SmartHub.Plugins.WebUI.Resources.Application.sections.list.js")]
    [JavaScriptResource("/application/sections/list-model.js", "SmartHub.Plugins.WebUI.Resources.Application.sections.list-model.js")]
    [JavaScriptResource("/application/sections/list-view.js", "SmartHub.Plugins.WebUI.Resources.Application.sections.list-view.js")]
    [HttpResource("/application/sections/list.tpl", "SmartHub.Plugins.WebUI.Resources.Application.sections.list.tpl")]
    [HttpResource("/application/sections/list-item.tpl", "SmartHub.Plugins.WebUI.Resources.Application.sections.list-item.tpl")]
    #endregion

    [Plugin]
    public class WebUIPlugin : PluginBase
    {
        #region Fields
        private readonly List<AppSectionAttribute> sectionItems = new List<AppSectionAttribute>();
        private readonly HashSet<string> cssFiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            foreach (var plugin in Context.GetAllPlugins())
            {
                var type = plugin.GetType();

                // элементы разделов
                sectionItems.AddRange(type.GetCustomAttributes<AppSectionAttribute>());
                // стили
                cssFiles.UnionWith(type.GetCustomAttributes<CssResourceAttribute>().Where(attr => attr.AutoLoad).Select(attr => attr.Url));
            }
        }
        #endregion

        #region Web API
        [HttpCommand("/api/webui/sections/common")]
        public object GetCommonSectionItems(HttpRequestParams request)
        {
            return GetSectionItems(SectionType.Common);
        }
        [HttpCommand("/api/webui/sections/system")]
        public object GetSystemSectionItems(HttpRequestParams request)
        {
            return GetSectionItems(SectionType.System);
        }
        [HttpCommand("/api/webui/styles.json")]
        public object GetStylesBundle(HttpRequestParams request)
        {
            return cssFiles;
        }
        #endregion

        #region Private methods
        private object GetSectionItems(SectionType sectionType)
        {
            return sectionItems
                .Where(sectionItem => sectionItem.Type == sectionType)
                .OrderBy(sectionItem => sectionItem.Title)
                .Select(sectionItem => new
                    {
                        id = Guid.NewGuid(),
                        name = sectionItem.Title,
                        path = sectionItem.GetModulePath(),
                        sortOrder = sectionItem.SortOrder,
                        tileTypeFullName = sectionItem.TileTypeFullName
                    }
                )
                .ToArray();
        }
        #endregion
    }
}
