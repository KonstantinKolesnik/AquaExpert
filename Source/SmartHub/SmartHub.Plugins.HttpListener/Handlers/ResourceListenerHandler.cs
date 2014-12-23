﻿using SmartHub.Plugins.HttpListener.Api;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace SmartHub.Plugins.HttpListener.Handlers
{
    public class ResourceListenerHandler : ListenerHandlerBase
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

        public override HttpContent GetResponseContent(HttpRequestParams parameters)
        {
            var resource = PrepareResource();

            var content = new ByteArrayContent(resource);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return content;
        }

        private byte[] PrepareResource()
        {
            byte[] result;

            if (resourceReference == null || !resourceReference.TryGetTarget(out result))
            {
                lock (lockObject)
                {
                    if (resourceReference == null || !resourceReference.TryGetTarget(out result))
                    {
                        result = LoadResource();
                        resourceReference = new WeakReference<byte[]>(result);
                    }
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
                {
                    var message = string.Format("Resource {0} is not found", path);
                    throw new FileNotFoundException(message);
                }
            }

            return result;
        }
    }
}
