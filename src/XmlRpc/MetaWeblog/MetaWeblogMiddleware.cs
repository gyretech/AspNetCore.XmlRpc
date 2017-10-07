using AspNetCore.XmlRpc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.XmlRpc.MetaWeblog
{
    public class MetaWeblogMiddleware
    {
        private ILogger logger;
        private readonly RequestDelegate next;

        public MetaWeblogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<XmlRpcOptions> options)
        {
            this.next = next;
            Options = options;
            logger = loggerFactory.CreateLogger<MetaWeblogMiddleware>();
        }

        public IOptions<XmlRpcOptions> Options { get; }

        /// <summary>
        /// Response to the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rpcService"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IXmlRpcService rpcService, IMetaWeblogEndpointProvider provider)
        {
            var xmlRpcContext = new XmlRpcContext(context, Options?.Value, new Dictionary<string, string>(), new Type[] { rpcService.GetType() });
            if (context.Request.Path.StartsWithSegments(xmlRpcContext.Options.SummaryEndpoint)
                || context.Request.Path.StartsWithSegments(xmlRpcContext.Options.Endpoint)
                || context.Request.Path.StartsWithSegments(xmlRpcContext.Options.RsdEndpoint))
            {
                // Add blog id into the context.
                xmlRpcContext.Values.Add(Options?.Value.BlogIdTokenName, context.Request.Path.ExtractBlogId());

                var result = await provider.ProcessAsync(xmlRpcContext);
                if (result)
                    return;
            }
            await next.Invoke(context);
        }
    }
}