using NLog;
using SmartHub.Core.Plugins;
using System.ComponentModel.Composition;

namespace SmartHub.Plugins.WebUI.Tiles
{
    public abstract class TileBase
    {
        #region Fields
        [Import(typeof(IServiceContext))]
        private IServiceContext context;
        private readonly Logger logger;
        #endregion

        #region Properties
        protected IServiceContext Context
        {
            get { return context; }
        }
        protected Logger Logger
        {
            get { return logger; }
        }
        #endregion

        #region Constructor
        protected TileBase()
        {
            logger = LogManager.GetLogger(typeof(WebUITilesPlugin).FullName);
        }
        #endregion

        #region Public methods
        public abstract void FillModel(TileWeb model, dynamic options);
        public virtual string ExecuteAction(object options)
        {
            return null;
        }
        #endregion
    }
}
