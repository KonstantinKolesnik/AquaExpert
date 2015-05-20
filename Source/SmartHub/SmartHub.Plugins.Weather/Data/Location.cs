using System;

namespace SmartHub.Plugins.Weather.Data
{
    public class Location
    {
        public virtual Guid Id { get; set; }

        public virtual string Query { get; set; }

        public virtual string DisplayName { get; set; }

        public override string ToString()
        {
            return string.Format("{0} (q: {1})", DisplayName, Query);
        }
    }
}
