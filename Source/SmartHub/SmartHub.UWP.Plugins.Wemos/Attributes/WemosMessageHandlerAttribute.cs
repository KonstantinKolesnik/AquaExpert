using SmartHub.UWP.Plugins.Wemos.Core;
using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.Wemos.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WemosMessageHandlerAttribute : ExportAttribute
    {
        public const string ContractID = nameof(WemosMessageHandlerAttribute);

        public WemosMessageHandlerAttribute()
            : base(ContractID, typeof(Action<WemosMessage>))
        {
        }
    }
}
