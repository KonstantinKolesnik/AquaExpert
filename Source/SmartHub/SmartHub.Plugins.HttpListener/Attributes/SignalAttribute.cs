using System;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.HttpListener.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SignalAttribute : ExportAttribute
    {
        public SignalAttribute()
            : base("ADD7FADD-D706-4D5A-9103-09B26FD2FD9C", typeof(Action<string, string>))
        {
        }
    }
}
