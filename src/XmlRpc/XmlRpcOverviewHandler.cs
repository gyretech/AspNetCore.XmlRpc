using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AspNetCore.XmlRpc
{
    /// <summary>
    /// HtmlTextWriter is not available in https://github.com/dotnet/corefx/issues/24169
    /// </summary>
    public class XmlRpcOverviewHandler : IXmlRpcHandler
    {
        public XmlRpcOverviewHandler()
        {
        }

        public bool CanProcess(XmlRpcContext context)
        {
            return context.Options.GenerateSummary && context.HttpContext.Request.Path.StartsWithSegments(context.Options.SummaryEndpoint);
        }

        public async Task ProcessRequestAsync(XmlRpcContext context)
        {
            if (!context.Options.GenerateSummary)
            {
                // Not found
                context.HttpContext.Response.StatusCode = 404;
                return;
            }

            var title = string.Concat("XML-RPC Methods for ", string.Join(",", context.Services.Select(s => s.FullName)));

            var methods = XmlRpcRequestParser.GetMethods(context.Services);

            using (var ms = new MemoryStream())
            {
                var writer = XmlWriter.Create(ms, new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.UTF8 });
                
                writer.WriteStartDocument();
                {
                    writer.WriteDocType("html", "PUBLIC", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd");
                    writer.WriteStartElement("html");
                    {
                        writer.WriteStartElement("head");
                        {
                            //Version info
                            writer.WriteComment($"AspNetCore.XmlRpc {Assembly.GetExecutingAssembly().GetName().Version}");

                            writer.WriteElementString("title", title);
                            // <meta name="robots" content="noindex" />
                            writer.WriteStartElement("meta");
                            {
                                writer.WriteAttributeString("name", "robots");
                                writer.WriteAttributeString("content", "noindex");
                            }
                            writer.WriteEndElement();

                            //Style
                            writer.WriteStartElement("style");
                            {
                                writer.WriteAttributeString("type", "text/css");
                                writer.WriteRaw(@"
body {
    font-family: Segoe UI Light, Segoe WP Light, Segoe UI, Helvetica, sans-serif;
    padding: 0;
    margin: 0;
}

body > div {
    padding: 0 20px;
}

body > div > div {
    margin-bottom: 50px;
    border-top: 1px solid #CCCCCC;
    width: 50%;
}

h1 {
    background-color: #1BA1E2;
    color: white;
    padding: 5px 20px;
    text-wrap: normal;
    word-break: break-all;
}

h2 {
    color: #1BA1E2;
}

ul {
    margin-bottom: 30px;
}

li {
    margin-bottom: 10px;
}

li > a {
    color: #000000;
}

table {
    width: 100%;
}

tr:nth-child(even) {
    background: #CCCCCC
}

tr:nth-child(odd) {
    background: #FFFFFF
}

td {
    height: 40px;
    vertical-align: middle;
    padding: 0 10px;
}"
                                    );
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("body");
                        {
                            writer.WriteElementString("h1", title);
                            writer.WriteStartElement("div");
                            {
                                writer.WriteElementString("p", "The following methods are supported: ");

                                // Method Names
                                writer.WriteStartElement("ul");
                                {
                                    foreach (var method in methods)
                                    {
                                        // Method Name
                                        writer.WriteStartElement("li");
                                        {
                                            writer.WriteStartElement("a");
                                            writer.WriteAttributeString("href", $"#{method.Value.Name}");
                                            writer.WriteString(method.Value.Name);
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            {
                                foreach (var method in methods)
                                {
                                    var mi = method.Value.MethodInfo;
                                    writer.WriteStartElement("h2");
                                    {
                                        writer.WriteStartElement("a");
                                        {
                                            writer.WriteAttributeString("name", method.Value.Name);
                                            writer.WriteString(method.Value.Name);
                                        }
                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();

                                    // "Parameters" headline
                                    writer.WriteElementString("h3", "Parameters");

                                    // "Parameters" table
                                    writer.WriteStartElement("table");
                                    {
                                        var parameters = mi.GetParameters();
                                        foreach (var parameter in parameters)
                                        {
                                            writer.WriteStartElement("tr");
                                            {

                                                writer.WriteStartElement("td");
                                                {
                                                    writer.WriteAttributeString("style", "width:30%");
                                                    writer.WriteString(parameter.ParameterType.Name);
                                                }
                                                writer.WriteEndElement();
                                                writer.WriteElementString("td", "");
                                                writer.WriteElementString("td", parameter.Name);
                                            }
                                            writer.WriteEndElement();
                                        }
                                    }
                                    writer.WriteEndElement();

                                    // "Return Value" headline
                                    writer.WriteElementString("h3", "Return Value");

                                    // "Return Value" table
                                    writer.WriteStartElement("table");
                                    {
                                        writer.WriteStartElement("tr");
                                        {

                                            writer.WriteStartElement("td");
                                            {
                                                writer.WriteAttributeString("style", "width:30%");
                                                writer.WriteString(mi.ReturnType.Name);
                                            }
                                            writer.WriteEndElement();

                                            writer.WriteStartElement("td");
                                            {
                                                writer.WriteString(
                                                    !string.IsNullOrEmpty(method.Value.Description)
                                                        ? method.Value.Description
                                                        : "-");
                                            }
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();

                writer.Flush();
                ms.Position = 0;

                context.HttpContext.Response.ContentType = "text/html";

                await ms.CopyToAsync(context.HttpContext.Response.Body);
            }

        }
    }
}
