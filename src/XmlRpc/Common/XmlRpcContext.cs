using Microsoft.AspNetCore.Http;
using System;

namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// Provides context information about the current Xml Rpc request
    /// </summary>
    public class XmlRpcContext
    {
        public XmlRpcContext(HttpContext httpContext, XmlRpcOptions options, params Type[] services)
        {
            HttpContext = httpContext;
            Options = options;
            Services = services;
        }

        public HttpContext HttpContext { get; }
        public XmlRpcOptions Options { get; }
        public Type[] Services { get; }
    }
}
