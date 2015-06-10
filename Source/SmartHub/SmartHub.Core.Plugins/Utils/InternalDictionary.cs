﻿using System;
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
                {
                    var msg = string.Format("duplicated key {0} ({1})", key, obj);
                    throw new Exception(msg);
                }

                Add(key, obj);
            }
        }
    }
}
