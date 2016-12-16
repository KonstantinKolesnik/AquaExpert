//using SmartHub.Plugins.WebUI.Tiles;
using System;
using System.Composition;

namespace SmartHub.UWP.Plugins.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TileAttribute : ExportAttribute
    {
        public const string ContractID = nameof(TileAttribute);

        //public TileAttribute()
        //    : base(ContractID, typeof(TileBase))
        //{
        //}
    }
}
