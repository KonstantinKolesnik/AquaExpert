using System;
using System.Composition;
using System.Linq;

namespace SmartHub.UWP.Core.Plugins
{
    public abstract class PluginBase
    {
        #region Properties
        [Import]
        public IServiceContext Context
        {
            get; set;
        }
        //protected Logger Logger
        //{
        //    get; private set;
        //}
        #endregion

        #region Constructor
        protected PluginBase()
        {
            //Logger = LogManager.GetLogger(GetType().FullName);
        }
        #endregion

        #region Plugin virtuals
        public virtual void InitPlugin()
        {
        }
        public virtual void StartPlugin()
        {
        }
        public virtual void StopPlugin()
        {
        }
        #endregion

        #region Public methods
        public void Run<T>(T[] actions, Action<T> task)
        {
            if (actions != null && actions.Any())
                foreach (var action in actions)
                {
                    try
                    {
                        task(action);
                    }
                    catch (Exception ex)
                    {
                        //Logger.Error(ex, ex.Message);
                    }
                }
        }
        #endregion
    }
}
