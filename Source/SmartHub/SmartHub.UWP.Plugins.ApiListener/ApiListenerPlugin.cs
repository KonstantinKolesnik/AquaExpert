using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [Plugin]
    public class ApiListenerPlugin : PluginBase
    {
        public const string ServiceName = "11111";

        #region Fields
        private StreamServer server = new StreamServer();
        private readonly Dictionary<string, ApiCommand> apiHandlers = new Dictionary<string, ApiCommand>();
        #endregion

        #region Imports
        /// <summary>
        /// API handlers from all plugins
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<ApiCommand, ApiCommandAttribute>> ApiCommands
        {
            get; set;
        }
        #endregion

        #region Plugin ovverrides
        public override void InitPlugin()
        {
            foreach (var handler in ApiCommands)
                apiHandlers.Add(handler.Metadata.CommandName, handler.Value);

            server.CommandProcessor = CommandProcessor;
        }
        public async override void StartPlugin()
        {
            await server.Start(ServiceName);
        }
        public async override void StopPlugin()
        {
            await server.Stop();
        }
        #endregion

        #region Event handlers
        private object CommandProcessor(string name, params object[] parameters)
        {
            try
            {
                return apiHandlers.ContainsKey(name) ? apiHandlers[name](parameters) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
