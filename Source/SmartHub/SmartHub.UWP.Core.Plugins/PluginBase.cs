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
        public void Run<T>(T[] parameters, Action<T> method)
        {
            if (parameters != null && parameters.Any())
                foreach (var parameter in parameters)
                {
                    try
                    {
                        method(parameter);
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
