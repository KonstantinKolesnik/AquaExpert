using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.ApiListener.Attributes
{
    //public delegate object ApiCommandMethod(params object[] parameters);
    public delegate object ApiCommandMethod(object parameters);

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiCommandAttribute : ExportAttribute
    {
        public const string ContractID = nameof(ApiCommandAttribute);

        public string Command
        {
            get;
            private set;
        }

        public ApiCommandAttribute() { }
        public ApiCommandAttribute(string command)
            //: base(ContractID, typeof(Func<object[], object>))
            : base(ContractID, typeof(ApiCommandMethod))
        {
            Command = command;
        }
    }
}
