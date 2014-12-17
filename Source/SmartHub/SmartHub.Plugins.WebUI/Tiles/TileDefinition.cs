using NLog;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.WebUI.Model;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public abstract class TileDefinition
    {
        #region fields

        [Import(typeof(IServiceContext))]
        private IServiceContext context;

        private readonly Logger logger;

        #endregion

        protected TileDefinition()
        {
            logger = LogManager.GetLogger(typeof(WebUiTilesPlugin).FullName);
        }

        protected Logger Logger
        {
            get { return logger; }
        }

        protected IServiceContext Context
        {
            get { return context; }
        }

        #region public

        public abstract void FillModel(TileModel model, dynamic options);

        public virtual string ExecuteAction(object options)
        {
            return null;
        }

        #endregion
    }
}
