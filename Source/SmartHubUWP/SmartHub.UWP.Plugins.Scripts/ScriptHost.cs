using System;
using System.Collections.Generic;

namespace SmartHub.UWP.Plugins.Scripts
{
    public class ScriptHost
    {
        private readonly Dictionary<string, ScriptCommand> scriptMethods;
        private readonly Action<string, object[]> scriptRunnerMethod;

        public ScriptHost(Dictionary<string, ScriptCommand> scriptMethods, Action<string, object[]> scriptRunnerMethod)
        {
            this.scriptMethods = scriptMethods;
            this.scriptRunnerMethod = scriptRunnerMethod;
        }

        public object executeMethod(string methodName, params object[] args)
        {
            try
            {
                return scriptMethods.ContainsKey(methodName) ? scriptMethods[methodName].Invoke(args) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void runScript(string scriptName, params object[] args)
        {
            scriptRunnerMethod(scriptName, args);
        }
    }
}
