using Newtonsoft.Json;
using SmartHub.UWP.Core;
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
        private HashSet<string> scriptEventNames;
        #endregion

        #region Import
        /// <summary>
        /// Методы плагинов для использования в скриптах
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

            scriptEventNames = RegisterScriptEventNames();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Запуск скриптов (для плагинов)
        /// </summary>
        public void ExecuteScript(UserScript script, params object[] args)
        {
            if (script != null)
                try
                {
                    var lines = script.Body.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var p = lines[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                    var method = p[0];
                    var methodName = p[1];
                    var pp = p.Skip(2).ToArray();

                    switch (method)
                    {
                        case "executeMethod": scriptHost.executeMethod(methodName, pp); break;
                        case "runScript": scriptHost.runScript(methodName, pp); break;
                    }





                    //var engine = new JScriptEngine(/*WindowsScriptEngineFlags.EnableDebugging*/);
                    //engine.AddHostObject("host", scriptHost);

                    //string initArgsScript = string.Format("var arguments = {0};", args.ToJson());// "[]"));
                    //engine.Execute(initArgsScript);
                    //engine.Execute(script.Body);
                }
                catch (Exception ex)
                {
                    //logger.Error(ex, string.Format("Error in user script {0}", script.Name));
                }
        }
        /// <summary>
        /// Запуск скриптов по имени (из других скриптов)
        /// </summary>
        public void ExecuteScriptByName(string scriptName, params object[] args)
        {
            var script = Context.StorageGet().Table<UserScript>().FirstOrDefault(n => n.Name == scriptName);
            ExecuteScript(script, args);
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
        private HashSet<string> RegisterScriptEventNames()
        {
            var scriptEvents = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

            foreach (var plugin in Context.GetAllPlugins())
            {
                var methods = plugin.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(ScriptEventHandlerDelegate[])).ToList();

                foreach (var method in methods)
                {
                    var eventInfo = method.GetCustomAttributes<ScriptEventAttribute>().SingleOrDefault();
                    if (eventInfo != null)
                    {
                        //logger.Info("Register script event '{0}' ({1})", eventInfo.EventAlias, member);

                        if (scriptEvents.Contains(eventInfo.EventAlias))
                            throw new Exception(string.Format("Duplicated event alias: '{0}'", eventInfo.EventAlias));

                        scriptEvents.Add(eventInfo.EventAlias);
                    }
                }
            }

            return scriptEvents;
        }
        #endregion

        #region Remote API
        [ApiMethod("/api/scripts")]
        public ApiMethod apiGetScripts => (args =>
        {
            return Context.GetPlugin<ScriptsPlugin>().GetScripts();
        });

        [ApiMethod("/api/scripts/add")]
        public ApiMethod apiAddScript => (args =>
        {
            var name = args[0].ToString();

            var model = new UserScript()
            {
                Name = name,
                Body = "alert('Not implemented!');"
            };

            Context.StorageSave(model);

            return model;
        });

        [ApiMethod("/api/scripts/update")]
        public ApiMethod apiUpdateScript => (args =>
        {
            var item = JsonConvert.DeserializeObject<UserScript>(args[0].ToString());

            if (item != null)
            {
                item.Name = item.Name ?? "Script";
                item.Body = item.Body ?? "alert('Not implemented!');";

                Context.StorageSaveOrUpdate(item);
                return true;
            }

            return false;
        });

        [ApiMethod("/api/scripts/delete")]
        public ApiMethod apiDeleteScript => (args =>
        {
            var id = args[0].ToString();

            var item = Context.GetPlugin<ScriptsPlugin>().GetScript(id);

            if (item != null)
            {
                Context.StorageDelete(item);
                return true;
            }

            return false;
        });

        [ApiMethod("/api/scripts/run")]
        public ApiMethod apiRunScript => (args =>
        {
            var id = args[0].ToString();

            var script = Context.GetPlugin<ScriptsPlugin>().GetScript(id);
            if (script != null)
            {
                Context.GetPlugin<ScriptsPlugin>().ExecuteScript(script, null);
                return true;
            }

            return false;
        });
        #endregion
    }
}
