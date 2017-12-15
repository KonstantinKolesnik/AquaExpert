using System;
using System.Collections.Generic;

namespace SmartHub.Core.Plugins.Utils
{
    [Serializable]
    public class InternalDictionary<T> : Dictionary<string, T> where T : class
    {
        private readonly object lockObject = new object();

        public InternalDictionary()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public void Register(string key, T obj)
        {
            if (string.IsNullOrWhiteSpace(key) || obj == null)
                return;

            lock (lockObject)
            {
                if (ContainsKey(key))
                    throw new Exception(string.Format("Duplicated key {0} ({1})", key, obj));

                Add(key, obj);
            }
        }
    }
}
