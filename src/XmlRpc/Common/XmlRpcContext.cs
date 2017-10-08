using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// Provides context information about the current Xml Rpc request
    /// </summary>
    public class XmlRpcContext
    {
        public XmlRpcContext(HttpContext httpContext, XmlRpcOptions options, Dictionary<string, string> values, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, params Type[] services)
        {
            HttpContext = httpContext;
            Options = options;
            Values = values;
            ServiceProvider = serviceProvider;
            ServiceScopeFactory = serviceScopeFactory;
            Services = services;
        }

        public HttpContext HttpContext { get; }
        public XmlRpcOptions Options { get; }
        public Dictionary<string, string> Values { get; }
        public IServiceProvider ServiceProvider { get; }
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public Type[] Services { get; }
    }
}
