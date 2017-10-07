using Microsoft.AspNetCore.Http;
using System;

namespace AspNetCore.XmlRpc.Extensions
{
    public static class XmlRpcContextExtensions
    {
        /// <summary>
        /// Extract blog id
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ExtractBlogId(this PathString path)
        {
            var value = path.Value;
            var index = value.LastIndexOf('/');
            var blogId = "";
            if (index >= 0)
            {
                blogId = value.Substring(index + 1, value.Length - index - 1);
            }
            return blogId;
        }

        public static string GenerateRsdUrl(this XmlRpcContext context)
        {
            string url = string.Concat(context.Options.RsdEndpoint, '/', context.Values[context.Options.BlogIdTokenName]);
            return GenerateAbsoluteUrl(context, url);
        }

        public static string GenerateEndpointUrl(this XmlRpcContext context)
        {
            string url = string.Concat(context.Options.Endpoint, '/', context.Values[context.Options.BlogIdTokenName]);
            return GenerateAbsoluteUrl(context, url);
        }

        public static string GenerateSummaryUrl(this XmlRpcContext context)
        {
            string url = string.Concat(context.Options.SummaryEndpoint, '/', context.Values[context.Options.BlogIdTokenName]);
            return GenerateAbsoluteUrl(context, url);
        }

        public static string GenerateHomepageUrl(this XmlRpcContext context)
        {
            string url = ReplaceTokens(context, context.Options.HomePageEndpointPattern);
            return GenerateAbsoluteUrl(context, url);
        }

        private static string ReplaceTokens(XmlRpcContext context, string rawUrl)
        {
            var url = rawUrl;
            foreach (var pair in context.Values)
            {
                url = url.Replace($"{{{pair.Key}}}", pair.Value);
            }
            return url;
        }

        /// <summary>
        /// Generate absolute Url 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        private static string GenerateAbsoluteUrl(XmlRpcContext context, string relativeUrl)
        {
            var builder = new UriBuilder();
            var request = context.HttpContext.Request;
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Host;
            if (request.Host.Port.HasValue)
                builder.Port = request.Host.Port.Value;
            builder.Path = relativeUrl;

            return builder.ToString();
        }
    }
}
