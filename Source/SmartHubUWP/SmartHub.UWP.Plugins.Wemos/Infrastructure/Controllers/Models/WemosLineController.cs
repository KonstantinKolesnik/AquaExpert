using SmartHub.UWP.Core.Plugins;
using SQLite.Net.Attributes;
using System;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models
{
    public class WemosLineController
    {
        #region Fields
        protected IServiceContext context;
        protected WemosPlugin host;
        #endregion

        #region Properties
        [PrimaryKey, NotNull]
        public string ID
        {
            get; set;
        } = Guid.NewGuid().ToString();
        [NotNull]
        public string LineID
        {
            get; set;
        }
        [NotNull, Default]
        public bool IsEnabled
        {
            get; set;
        }
        [NotNull, Default()]
        public float Value
        {
            get; set;
        }
        #endregion

        #region Public methods
        public void Init(IServiceContext context)
        {
            this.context = context;
            host = context?.GetPlugin<WemosPlugin>();
        }

        public async Task ProcessAsync()
        {
            var line = host.GetLine(LineID);
            var lastValue = host.GetLineLastValue(LineID);
            if (lastValue == null)
                await host.RequestLineValueAsync(line);
            else if (lastValue.Value != Value)
                await host.SetLineValueAsync(line, Value);
        }
        #endregion
    }
}
