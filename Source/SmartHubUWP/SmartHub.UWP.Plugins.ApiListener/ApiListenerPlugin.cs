﻿using SmartHub.UWP.Core.Communication.Stream;
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
        private readonly Dictionary<string, ApiMethod> apiMethods = new Dictionary<string, ApiMethod>();
        #endregion

        #region Imports
        /// <summary>
        /// API methods from all plugins
        /// </summary>
        [ImportMany]
        public IEnumerable<Lazy<ApiMethod, ApiMethodAttribute>> ApiMethods
        {
            get; set;
        }
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            foreach (var apiMethod in ApiMethods)
                apiMethods.Add(apiMethod.Metadata.MethodName, apiMethod.Value);

            server.CommandProcessor = (name, args) =>
            {
                try
                {
                    return apiMethods.ContainsKey(name) ? apiMethods[name].Invoke(args) : null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            };
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
    }
}
