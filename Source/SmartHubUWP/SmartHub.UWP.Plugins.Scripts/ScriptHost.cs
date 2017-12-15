using System;
using System.Collections.Generic;

namespace SmartHub.UWP.Plugins.Scripts
{
    public class ScriptHost
    {
        private readonly Dictionary<string, ScriptCommand> methods;
        private readonly Action<string, object[]> scriptRunnerMethod;

        public ScriptHost(Dictionary<string, ScriptCommand> methods, Action<string, object[]> scriptRunnerMethod)
        {
            this.methods = methods;
            this.scriptRunnerMethod = scriptRunnerMethod;
        }

        public object executeMethod(string methodName, params object[] args)
        {
            try
            {
                return methods.ContainsKey(methodName) ? methods[methodName].Invoke(args) : null;
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
