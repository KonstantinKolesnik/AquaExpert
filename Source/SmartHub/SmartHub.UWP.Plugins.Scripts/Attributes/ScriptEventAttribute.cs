using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ScriptEventAttribute : ImportManyAttribute
    {
        public const string ContractID = nameof(ScriptEventAttribute);

        public string EventAlias
        {
            get; protected set;
        }

        public ScriptEventAttribute(string eventAlias)
            : base(ContractID)//, typeof(ScriptEventHandlerDelegate))
        {
            EventAlias = eventAlias;
        }
    }
}
