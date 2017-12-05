using SmartHub.UWP.Core;
using SmartHub.UWP.Core.Plugins;
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
    }
}
