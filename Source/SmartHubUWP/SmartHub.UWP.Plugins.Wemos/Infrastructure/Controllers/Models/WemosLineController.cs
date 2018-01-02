using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Lines;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using SQLite.Net.Attributes;
using System;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Wemos.Infrastructure.Controllers.Models
{
    public class WemosLineController
    {
        #region Fields
        private IServiceContext context;
        private WemosPlugin host;
        private WemosLine line;
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

        // list of setters

        // may be set from:
        // - UI (switch/slider/color picker etc.)
        // - script (programmaticaly)
        // - time source
        // - other line value
        // after set, if other line is available, set back its value
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

            line = host.GetLine(LineID);
            host.RequestLineValueAsync(line).Wait();
        }
        public async Task ProcessAsync()
        {
            if (IsEnabled)
            {
                var lastValue = context.GetPlugin<LinesPlugin>().GetLineLastValue(LineID);

                if (lastValue == null)
                    await host.RequestLineValueAsync(line);
                else if (lastValue.Value != Value)
                    await host.SetLineValueAsync(line, Value);
            }
        }
        #endregion
    }
}
