using System;
using System.Composition;
using System.Linq;

namespace SmartHub.UWP.Core.Plugins
{
    [Plugin]
    public /*abstract*/ class PluginBase
    {
        #region Properties
        [Import]
        protected IServiceContext Context
        {
            get; set;
        }
        //protected Logger Logger
        //{
        //    get; private set;
        //}
        #endregion

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            var a = 0;
            var b = a;
        }


        #region Constructor
        //[ImportingConstructor]

        //protected PluginBase()
        public PluginBase()
        {
            //Logger = LogManager.GetLogger(GetType().FullName);

            //var configuration = new ContainerConfiguration().WithPart<IServiceContext>();
            //using (CompositionHost host = configuration.CreateContainer())
            //    host.SatisfyImports(this);

            var a = 0;
            var b = a;
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
