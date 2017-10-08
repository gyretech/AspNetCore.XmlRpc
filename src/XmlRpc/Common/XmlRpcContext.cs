using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// Provides context information about the current Xml Rpc request
    /// </summary>
    public class XmlRpcContext
    {
        public XmlRpcContext(HttpContext httpContext, XmlRpcOptions options, Dictionary<string, string> values, IServiceProvider serviceProvider, params Type[] services)
        {
            HttpContext = httpContext;
            Options = options;
            Values = values;
            ServiceProvider = serviceProvider;
            Services = services;
        }

        public HttpContext HttpContext { get; }
        public XmlRpcOptions Options { get; }
        public Dictionary<string, string> Values { get; }
        public IServiceProvider ServiceProvider { get; }
        public Type[] Services { get; }
    }
}
