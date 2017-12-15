using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Scripts.Attributes;
using SmartHub.UWP.Plugins.Scripts.Models;
using SmartHub.UWP.Plugins.Scripts.UI;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;

namespace SmartHub.UWP.Plugins.Scripts
{
    [Plugin]
    [AppSectionItem("Scripts", AppSectionType.Applications, typeof(MainPage), "Scripts")]
    [AppSectionItem("Scripts", AppSectionType.System, typeof(SettingsPage), "Scripts")]
    public class ScriptsPlugin : PluginBase
    {
        #region Fields
        private ScriptHost scriptHost;
        private HashSet<string> scriptEvents;
        #endregion

        #region Import
        /// <summary>
        /// Методы плагинов, доступные в скриптах
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<ScriptCommand, ScriptCommandAttribute>> ScriptCommands
        {
            get; set;
        }
        //[OnImportsSatisfied]
        //public void OnImportsSatisfied()
        //{
        //    int a = 0;
        //    int b = a;
        //}
        #endregion

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            var db = Context.StorageGet();
            db.CreateTable<UserScript>();
            db.CreateTable<ScriptEventHandler>();
        }
        public override void InitPlugin()
        {
            var scriptCommands = new Dictionary<string, ScriptCommand>();
            foreach (var scriptCommand in ScriptCommands)
                scriptCommands.Add(scriptCommand.Metadata.MethodName, scriptCommand.Value);

            scriptHost = new ScriptHost(scriptCommands, ExecuteScriptByName);
            scriptEvents = RegisterScriptEvents(Context.GetAllPlugins());
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Запуск скриптов (для плагинов)
        /// </summary>
        public void ExecuteScript(UserScript script, params object[] args)
        {
            ExecuteScript(script, scriptHost, args);
        }

        public List<UserScript> GetScripts()
        {
            return Context.StorageGet().Table<UserScript>().ToList();
        }
        public UserScript GetScript(string id)
        {
            return Context.StorageGet().Table<UserScript>().FirstOrDefault(x => x.ID == id);
        }
        #endregion

        #region Private methods
        private static HashSet<string> RegisterScriptEvents(IEnumerable<PluginBase> plugins)
        {
            var scriptEvents = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var plugin in plugins)
            {
                var properties = plugin.GetType()
                    .GetProperties()
                    .Where(m => m.PropertyType == typeof(ScriptEventHandlerDelegate[]))
                    .ToList();

                foreach (var member in properties)
                {
                    var eventInfo = member.GetCustomAttributes<ScriptEventAttribute>().SingleOrDefault();

                    if (eventInfo != null)
                    {
                        //logger.Info("Register script event '{0}' ({1})", eventInfo.EventAlias, member);

                        if (scriptEvents.Contains(eventInfo.EventAlias))
                        {
                            var message = string.Format("Duplicate event alias: '{0}'", eventInfo.EventAlias);
                            throw new Exception(message);
                        }

                        scriptEvents.Add(eventInfo.EventAlias);
                    }
                }
            }

            return scriptEvents;
        }

        /// <summary>
        /// Запуск скриптов по имени (из других скриптов)
        /// </summary>
        private void ExecuteScriptByName(string scriptName, object[] args)
        {
            var script = Context.StorageGet().Table<UserScript>().FirstOrDefault(n => n.Name == scriptName);
            ExecuteScript(script, scriptHost, args);
        }
        /// <summary>
        /// Запуск скрипта
        /// </summary>
        private static void ExecuteScript(UserScript script, ScriptHost scriptHost, object[] args)
        {
            //if (script != null)
            //    try
            //    {
            //        //var engine = new JScriptEngine(WindowsScriptEngineFlags.EnableDebugging);
            //        var engine = new JScriptEngine();
            //        engine.AddHostObject("host", scriptHost);

            //        string initArgsScript = string.Format("var arguments = {0};", args.ToJson());// "[]"));
            //        engine.Execute(initArgsScript);
            //        engine.Execute(script.Body);
            //    }
            //    catch (Exception ex)
            //    {
            //        var message = string.Format("Error in user script {0}", script.Name);
            //        //logger.Error(ex, message);
            //    }
        }
        #endregion

        #region Remote API
        [ApiMethod(MethodName = "/api/scripts"), Export(typeof(ApiMethod))]
        public ApiMethod apiGetNodes => ((args) =>
        {
            return Context.GetPlugin<ScriptsPlugin>().GetScripts();
        });

        [ApiMethod(MethodName = "/api/scripts/setname"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetScriptName => ((args) =>
        {
            var id = args[0] as string;
            var name = args[1] as string;

            var item = Context.GetPlugin<ScriptsPlugin>().GetScript(id);
            item.Name = !string.IsNullOrEmpty(name) ? name : "Script";
            Context.StorageSaveOrUpdate(item);

            //NotifyForSignalR(new { MsgId = "NodeNameChanged", Data = new { Id = id, Name = name } });

            return true;
        });

        [ApiMethod(MethodName = "/api/scripts/setbody"), Export(typeof(ApiMethod))]
        public ApiMethod apiSetScriptBody => ((args) =>
        {
            var id = args[0] as string;
            var body = args[1] as string;

            var item = Context.GetPlugin<ScriptsPlugin>().GetScript(id);
            item.Body = !string.IsNullOrEmpty(body) ? body : "alert('No code!');";
            Context.StorageSaveOrUpdate(item);

            return true;
        });

        [ApiMethod(MethodName = "/api/scripts/add"), Export(typeof(ApiMethod))]
        public ApiMethod apiAddScript => ((args) =>
        {
            var name = args[0] as string;

            var model = new UserScript()
            {
                Name = name,
                Body = "alert('No code!');"
            };

            Context.StorageSave(model);

            //NotifyForSignalR(new { MsgId = "MonitorAdded", Data = BuildMonitorWebModel(ctrl) });

            return model;
        });

        [ApiMethod(MethodName = "/api/scripts/delete"), Export(typeof(ApiMethod))]
        public ApiMethod apiDeleteScript => ((args) =>
        {
            var id = args[0] as string;

            Context.StorageDelete(Context.GetPlugin<ScriptsPlugin>().GetScript(id));

            return true;
        });

        [ApiMethod(MethodName = "/api/scripts/run"), Export(typeof(ApiMethod))]
        public ApiMethod apiRunScript => ((args) =>
        {
            var id = args[0] as string;

            var script = Context.GetPlugin<ScriptsPlugin>().GetScript(id);
            ExecuteScript(script, null);

            return true;
        });
        #endregion
    }
}
