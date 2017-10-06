using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XmlRpcMvc.Common;

namespace XmlRpcMvc.MetaWeblog
{
    public static class MetaWeblogExtensions
    {
        /// <summary>
        /// Register services
        /// </summary>
        /// <typeparam name="TRpcXmlService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMetaWeblog<TRpcXmlService, TMetaWeblogEndpointService>(this IServiceCollection services) where TRpcXmlService : class, IXmlRpcService where TMetaWeblogEndpointService : class, IMetaWeblogEndpointService
        {
            return services.AddScoped<IXmlRpcService, TRpcXmlService>()
                .AddScoped<IMetaWeblogEndpointService, TMetaWeblogEndpointService>();

        }

        /// <summary>
        /// Use Meta Weblog
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMetaWeblog(this IApplicationBuilder builder, string apiUri)
        {
            return builder.UseMiddleware<MetaWeblogMiddleware>(apiUri);
        }
    }
}
