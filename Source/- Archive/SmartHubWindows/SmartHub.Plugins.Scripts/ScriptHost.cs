using NLog;
using SmartHub.Core.Plugins.Utils;
using System;

namespace SmartHub.Plugins.Scripts
{
    public class ScriptHost
    {
        private readonly InternalDictionary<Delegate> methods;
        private readonly Logger logger;
        private readonly Action<string, object[]> scriptRunner;

        public ScriptHost(InternalDictionary<Delegate> methods, Logger logger, Action<string, object[]> scriptRunner)
        {
            this.methods = methods;
            this.logger = logger;
            this.scriptRunner = scriptRunner;
        }

        public object executeMethod(string method, params object[] args)
        {
            return methods[method].DynamicInvoke(args);
        }
        public void logInfo(object message, params object[] args)
        {
            logger.Log(LogLevel.Info, message.ToString(), args);
        }
        public void logError(object message, params object[] args)
        {
            logger.Log(LogLevel.Error, message.ToString(), args);
        }
        public void runScript(string name, params object[] args)
        {
            scriptRunner(name, args);
        }
    }
}
