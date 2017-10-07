using AspNetCore.XmlRpc.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace AspNetCore.XmlRpc.MetaWeblog
{
    /// <summary>
    /// HtmlTextWriter is not available in https://github.com/dotnet/corefx/issues/24169
    /// </summary>
    public class XmlRpcRsdHandler : IXmlRpcHandler
    {
        public XmlRpcRsdHandler()
        {
        }

        public bool CanProcess(XmlRpcContext context)
        {
            return context.HttpContext.Request.Path.StartsWithSegments(context.Options.RsdEndpoint);
        }

        public async Task ProcessRequestAsync(XmlRpcContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Method.Equals(
                    HttpVerbs.Get.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException();
            }

            using (var ms = new MemoryStream())
            {
                var xmlWriter = XmlWriter.Create(ms);

                xmlWriter.WriteStartDocument();
                {
                    xmlWriter.WriteStartElement("manifest", "http://schemas.microsoft.com/wlw/manifest/weblog");
                    {
                        xmlWriter.WriteStartElement("service", string.Empty);
                        {
                            xmlWriter.WriteStartElement("engineName");
                            xmlWriter.WriteString(context.Options.EngineName);
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("homePageLink");
                            xmlWriter.WriteString(context.GenerateHomepageUrl());
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("apis");
                            {
                                xmlWriter.WriteStartElement("api");
                                xmlWriter.WriteAttributeString("name", "MetaWeblog");
                                xmlWriter.WriteAttributeString("preferred", "true");
                                xmlWriter.WriteAttributeString("apiLink", context.GenerateEndpointUrl());
                                xmlWriter.WriteAttributeString("blogID", context.Values[context.Options.BlogIdTokenName] ?? string.Empty);
                            }
                            xmlWriter.WriteEndElement();

                        }
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteStartElement("options", string.Empty);
                    {
                        //<clientType>Metaweblog</clientType>
                        xmlWriter.WriteStartElement("clientType");
                        xmlWriter.WriteString("Metaweblog");
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("supportsSlug");
                        xmlWriter.WriteString("Yes");
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("supportsExcerpt");
                        xmlWriter.WriteString("Yes");
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndDocument();

                xmlWriter.Flush();
                ms.Position = 0;

                context.HttpContext.Response.ContentType = "text/xml";

                await ms.CopyToAsync(context.HttpContext.Response.Body);
            }
        }
    }
}
