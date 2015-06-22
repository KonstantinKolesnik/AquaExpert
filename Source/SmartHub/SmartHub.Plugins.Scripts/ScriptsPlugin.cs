using Microsoft.ClearScript.Windows;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NLog;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Scripts.Attributes;
using SmartHub.Plugins.Scripts.Data;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace SmartHub.Plugins.Scripts
{
    [AppSection("Обработчики событий", SectionType.System, "/webapp/scripts/subscriptions.js", "SmartHub.Plugins.Scripts.Resources.subscriptions.js")]
    [JavaScriptResource("/webapp/scripts/subscriptions-model.js", "SmartHub.Plugins.Scripts.Resources.subscriptions-model.js")]
    [JavaScriptResource("/webapp/scripts/subscriptions-view.js", "SmartHub.Plugins.Scripts.Resources.subscriptions-view.js")]
    [HttpResource("/webapp/scripts/subscriptions-layout.tpl", "SmartHub.Plugins.Scripts.Resources.subscriptions-layout.tpl")]
    [HttpResource("/webapp/scripts/subscriptions-form.tpl", "SmartHub.Plugins.Scripts.Resources.subscriptions-form.tpl")]
    [HttpResource("/webapp/scripts/subscriptions-list.tpl", "SmartHub.Plugins.Scripts.Resources.subscriptions-list.tpl")]
    [HttpResource("/webapp/scripts/subscriptions-list-item.tpl", "SmartHub.Plugins.Scripts.Resources.subscriptions-list-item.tpl")]

    [AppSection("Скрипты", SectionType.System, "/webapp/scripts/script-list.js", "SmartHub.Plugins.Scripts.Resources.script-list.js")]
    [JavaScriptResource("/webapp/scripts/script-list-model.js", "SmartHub.Plugins.Scripts.Resources.script-list-model.js")]
    [JavaScriptResource("/webapp/scripts/script-list-view.js", "SmartHub.Plugins.Scripts.Resources.script-list-view.js")]
    [HttpResource("/webapp/scripts/script-list-item.tpl", "SmartHub.Plugins.Scripts.Resources.script-list-item.tpl")]
    [HttpResource("/webapp/scripts/script-list.tpl", "SmartHub.Plugins.Scripts.Resources.script-list.tpl")]

    // editor
    [JavaScriptResource("/webapp/scripts/script-editor.js", "SmartHub.Plugins.Scripts.Resources.script-editor.js")]
    [JavaScriptResource("/webapp/scripts/script-editor-model.js", "SmartHub.Plugins.Scripts.Resources.script-editor-model.js")]
    [JavaScriptResource("/webapp/scripts/script-editor-view.js", "SmartHub.Plugins.Scripts.Resources.script-editor-view.js")]
    [HttpResource("/webapp/scripts/script-editor.tpl", "SmartHub.Plugins.Scripts.Resources.script-editor.tpl")]

    [Plugin]
    public class ScriptsPlugin : PluginBase
    {
        #region Fields
        private ScriptHost scriptHost;
        private HashSet<string> scriptEvents;
        #endregion

        #region Import
        /// <summary>
        /// Методы плагинов, доступные для скриптов
        /// </summary>
        [ImportMany("41AAE5E9-50CE-46E9-AE54-5A4DF4049846")]
        public Lazy<Delegate, IScriptCommandAttribute>[] ScriptCommands { get; set; }
        #endregion

        #region Export
        /// <summary>
        /// Запуск скриптов, подписанных на события
        /// </summary>
        [Export("BE10460E-0E9E-4169-99BB-B1DE43B150FC", typeof(ScriptEventHandlerDelegate))]
        public void OnScriptEvent(string eventAlias, object[] args)
        {
            using (var session = Context.OpenSession())
            {
                var scripts = session.Query<ScriptEventHandler>()
                    .Where(s => s.EventAlias == eventAlias)
                    .Select(x => x.UserScript)
                    .ToList();

                foreach (var script in scripts)
                    ExecuteScript(script, scriptHost, Logger, args);
            }
        }
        #endregion

        #region Pligin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<UserScript>(cfg => cfg.Table("Scripts_UserScript"));
            mapper.Class<ScriptEventHandler>(cfg => cfg.Table("Scripts_EventHandler"));
        }
        public override void InitPlugin()
        {
            var actions = RegisterScriptCommands();

            scriptHost = new ScriptHost(actions, Logger, ExecuteScriptByName);
            scriptEvents = RegisterScriptEvents(Context.GetAllPlugins(), Logger);
        }
        #endregion

        #region Public methods
        public ReadOnlyCollection<string> ScriptEvents
        {
            get { return scriptEvents.ToList().AsReadOnly(); }
        }
        
        public List<UserScript> GetScripts()
        {
            using (var session = Context.OpenSession())
                return session.Query<UserScript>().ToList();
        }
        public UserScript GetScript(Guid id)
        {
            using (var session = Context.OpenSession())
                return session.Query<UserScript>().FirstOrDefault(x => x.Id == id);
        }

        public object BuildScriptRichWebModel(UserScript script)
        {
            if (script == null)
                return null;

            return new {
                Id = script.Id,
                Name = script.Name,
                Body = script.Body
            };
        }

        /// <summary>
        /// Запуск скриптов (для плагинов)
        /// </summary>
        /// <param name="script"></param>
        /// <param name="args"></param>
        public void ExecuteScript(UserScript script, params object[] args)
        {
            ExecuteScript(script, scriptHost, Logger, args);
        }
        #endregion

        #region Private methods
        private InternalDictionary<Delegate> RegisterScriptCommands()
        {
            var actions = new InternalDictionary<Delegate>();

            foreach (var action in ScriptCommands)
                actions.Register(action.Metadata.Alias, action.Value);

            return actions;
        }
        private static HashSet<string> RegisterScriptEvents(IEnumerable<PluginBase> plugins, Logger logger)
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
                        logger.Info("Register script event '{0}' ({1})", eventInfo.EventAlias, member);

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
            using (var session = Context.OpenSession())
            {
                var script = session.Query<UserScript>().First(s => s.Name == scriptName);
                ExecuteScript(script, args);
            }
        }
        /// <summary>
        /// Запуск скрипта
        /// </summary>
        private static void ExecuteScript(UserScript script, ScriptHost scriptHost, Logger logger, object[] args)
        {
            //Debugger.Launch();
            try
            {
                //var engine = new JScriptEngine(WindowsScriptEngineFlags.EnableDebugging);
                var engine = new JScriptEngine();
                engine.AddHostObject("host", scriptHost);

                string initArgsScript = string.Format("var arguments = {0};", args.ToJson("[]"));
                engine.Execute(initArgsScript);
                engine.Execute(script.Body);
            }
            catch (Exception ex)
            {
                var messge = string.Format("Error in user script {0}", script.Name);
                logger.Error(ex, messge);
            }
        }

        private object BuildScriptWebModel(UserScript script)
        {
            if (script == null)
                return null;

            return new
            {
                Id = script.Id,
                Name = script.Name
            };
        }
        #endregion

        #region Web API
        #region Scripts
        [HttpCommand("/api/scripts/list")]
        private object apiGetScripts(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                return GetScripts()
                    .Select(BuildScriptWebModel)
                    .ToArray();
            }
        }
        [HttpCommand("/api/scripts/get")]
        private object apiGetScript(HttpRequestParams request)
        {
            Guid id = request.GetRequiredGuid("id");
            return BuildScriptRichWebModel(GetScript(id));
        }
        [HttpCommand("/api/scripts/save")]
        private object apiSaveScript(HttpRequestParams request)
        {
            Guid? id = request.GetGuid("id");
            string name = request.GetRequiredString("name");
            string body = request.GetString("body");

            using (var session = Context.OpenSession())
            {
                var script = id.HasValue ? session.Get<UserScript>(id.Value) : new UserScript { Id = Guid.NewGuid() };
                script.Name = name;
                script.Body = body;

                session.SaveOrUpdate(script);
                session.Flush();
            }

            return null;
        }
        [HttpCommand("/api/scripts/delete")]
        private object apiDeleteScript(HttpRequestParams request)
        {
            Guid scriptId = request.GetRequiredGuid("scriptId");

            using (var session = Context.OpenSession())
            {
                var subscription = session.Load<UserScript>(scriptId);
                session.Delete(subscription);
                session.Flush();
            }

            return null;
        }
        [HttpCommand("/api/scripts/run")]
        private object apiRunScript(HttpRequestParams request)
        {
            Guid scriptId = request.GetRequiredGuid("scriptId");

            ExecuteScript(GetScript(scriptId), new object[0]);

            return null;
        }
        #endregion

        #region Subscriptions
        [HttpCommand("/api/scripts/subscription/form")]
        public object GetSubscriptionForm(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                var events = Context.GetPlugin<ScriptsPlugin>().ScriptEvents
                    .Select(eventAlias => new { id = eventAlias, name = eventAlias })
                    .ToList();

                var scripts = session.Query<UserScript>()
                    .Select(x => new { id = x.Id, name = x.Name })
                    .ToArray();

                var selectedEventAlias = events.Any() ? events.First().id : null;
                var selectedScriptId = scripts.Any() ? (Guid?)scripts.First().id : null;

                return new
                {
                    eventList = events,
                    scriptList = scripts,
                    selectedEventAlias,
                    selectedScriptId
                };
            }
        }

        [HttpCommand("/api/scripts/subscription/list")]
        public object GetSubscriptionList(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                return session.Query<ScriptEventHandler>()
                    .Select(x => new
                    {
                        id = x.Id,
                        scriptId = x.UserScript.Id,
                        scriptName = x.UserScript.Name,
                        eventAlias = x.EventAlias
                    })
                    .ToList();
            }
        }

        [HttpCommand("/api/scripts/subscription/add")]
        public object AddSubscription(HttpRequestParams request)
        {
            string eventAlias = request.GetRequiredString("eventAlias");
            Guid scriptId = request.GetRequiredGuid("scriptId");

            using (var session = Context.OpenSession())
            {
                var guid = Guid.NewGuid();

                var script = session.Load<UserScript>(scriptId);

                var subscription = new ScriptEventHandler
                {
                    Id = guid,
                    EventAlias = eventAlias,
                    UserScript = script
                };

                session.Save(subscription);
                session.Flush();

                return guid;
            }
        }

        [HttpCommand("/api/scripts/subscription/delete")]
        public object DeleteSubscription(HttpRequestParams request)
        {
            Guid subscriptionId = request.GetRequiredGuid("subscriptionId");

            using (var session = Context.OpenSession())
            {
                var subscription = session.Load<ScriptEventHandler>(subscriptionId);
                session.Delete(subscription);
                session.Flush();
            }

            return null;
        }
        #endregion
        #endregion
    }
}
