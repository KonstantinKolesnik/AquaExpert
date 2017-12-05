using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ScriptCommandAttribute : ExportAttribute, IScriptCommandAttribute
    {
        public const string ContractID = nameof(ScriptCommandAttribute);

        public string Alias
        {
            get; private set;
        }

        public ScriptCommandAttribute(string alias)
            : base(ContractID, typeof(Delegate))
        {
            Alias = alias;
        }
    }
}
