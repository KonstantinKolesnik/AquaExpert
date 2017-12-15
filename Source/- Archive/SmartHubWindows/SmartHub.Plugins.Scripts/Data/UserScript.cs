using System;

namespace SmartHub.Plugins.Scripts.Data
{
    public class UserScript
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Body { get; set; }
    }
}
