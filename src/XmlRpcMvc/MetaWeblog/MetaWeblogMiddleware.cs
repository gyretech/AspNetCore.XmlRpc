using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace XmlRpcMvc.MetaWeblog
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
            var xmlRpcContext = new XmlRpcContext(context, Options?.Value, new Type[] { rpcService.GetType() });
            var result = await provider.ProcessAsync(xmlRpcContext);
            if (result)
                return;
            await next.Invoke(context);
        }

    }
}