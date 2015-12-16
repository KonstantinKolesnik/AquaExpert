using Microsoft.Owin;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public class ResourceListenerHandler : IListenerHandler
    {
        private readonly object lockObject = new object();
        private WeakReference<byte[]> resourceReference;

        private readonly Assembly assembly;
        private readonly string path;
        private readonly string contentType;

        public ResourceListenerHandler(Assembly assembly, string path, string contentType)
        {
            this.assembly = assembly;
            this.path = path;
            this.contentType = contentType;
        }

        public Task ProcessRequest(OwinRequest request)
        {
            byte[] resource = GetResource();

            var response = new OwinResponse(request.Environment)
            {
                ContentType = contentType,
                ContentLength = resource.Length
            };

            return response.WriteAsync(resource);
        }

        private byte[] GetResource()
        {
            byte[] result;

            if (resourceReference == null || !resourceReference.TryGetTarget(out result))
                lock (lockObject)
                {
                    if (resourceReference == null || !resourceReference.TryGetTarget(out result))
                    {
                        result = LoadResource();
                        resourceReference = new WeakReference<byte[]>(result);
                    }
                }

            return result;
        }
        private byte[] LoadResource()
        {
            byte[] result;

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                if (stream != null)
                {
                    result = new byte[stream.Length];
                    stream.Read(result, 0, result.Length);
                }
                else
                    throw new FileNotFoundException(string.Format("Resource {0} is not found", path));
            }

            return result;
        }
    }
}
