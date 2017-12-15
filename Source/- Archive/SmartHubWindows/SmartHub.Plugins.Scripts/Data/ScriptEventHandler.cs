using System;

namespace SmartHub.Plugins.Scripts.Data
{
    public class ScriptEventHandler
    {
        public virtual Guid Id { get; set; }

        public virtual string EventAlias { get; set; }

        public virtual UserScript UserScript { get; set; }
    }
}
