using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace XmlRpcMvc.MetaWeblog
{
    public class MetaWeblogMiddleware
    {
        private ILogger logger;
        private readonly RequestDelegate next;
        private string _urlEndpoint;

        public MetaWeblogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, string urlEndpoint)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger<MetaWeblogMiddleware>(); ;
            _urlEndpoint = urlEndpoint;
        }

        /// <summary>
        /// Response to the request
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            //TODO: add details of using 
            await next.Invoke(context);
        }

    }
}