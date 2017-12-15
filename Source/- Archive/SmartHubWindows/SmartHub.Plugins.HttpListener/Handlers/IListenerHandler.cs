using Microsoft.Owin;
using System.Threading.Tasks;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public interface IListenerHandler
    {
        Task ProcessRequest(OwinRequest request);
    }
}
