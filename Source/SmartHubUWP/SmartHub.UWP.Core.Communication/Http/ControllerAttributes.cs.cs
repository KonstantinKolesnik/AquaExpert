﻿using System;
using Windows.Web.Http;

namespace SmartHub.UWP.Core.Communication.Http
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class Body : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JsonBody : Body
    {
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RoutePrefix : Attribute
    {
        private string path;

        public virtual string Path
        {
            get { return path; }
        }

        public RoutePrefix(string path)
        {
            this.path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Route : Attribute
    {
        private string path;

        public virtual string Path
        {
            get { return path; }
        }

        public Route()
            : this(String.Empty)
        {
        }
        public Route(string path)
        {
            this.path = path;
        }
    }

    #region Http methods
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class HttpRequestMethod : Attribute
    {
        public virtual HttpMethod Method { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpGet : HttpRequestMethod
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPost : HttpRequestMethod
    {
        public override HttpMethod Method { get; } = HttpMethod.Post;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpDelete : HttpRequestMethod
    {
        public override HttpMethod Method { get; } = HttpMethod.Delete;
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPut : HttpRequestMethod
    {
        public override HttpMethod Method { get; } = HttpMethod.Put;
    }
    #endregion
}
