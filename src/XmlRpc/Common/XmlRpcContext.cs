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
        public XmlRpcContext(HttpContext httpContext, XmlRpcOptions options, Dictionary<string, string> values, params Type[] services)
        {
            HttpContext = httpContext;
            Options = options;
            Values = values;
            Services = services;
        }

        public HttpContext HttpContext { get; }
        public XmlRpcOptions Options { get; }
        public Dictionary<string, string> Values { get; }
        public Type[] Services { get; }
    }
}
