using HtmlTags;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using static HtmlTags.HtmlTextWriter;

namespace XmlRpcMvc
{
    /// <summary>
    /// HtmlTextWriter is not available in https://github.com/dotnet/corefx/issues/24169
    /// </summary>
    public class XmlRpcOverviewResult : ContentResult
    {
        private readonly bool _generateOverview;
        private readonly Type[] _services;

        public XmlRpcOverviewResult(
            bool generateOverview, params Type[] services)
        {
            _generateOverview = generateOverview;
            _services = services;

            ContentType = "text/html";
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (!_generateOverview)
            {
                new NotFoundResult().ExecuteResult(context);
                return;
            }

            var title = context.RouteData.Values["action"] ?? "";

            var methods = XmlRpcRequestParser.GetMethods(_services);

            using (var stringWriter = new StringWriter())
            using (var writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Html);
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Head);
                    {
                        // Version Info
                        writer.Write("<!--");
                        writer.Write("XmlRpcMvc {0}", Assembly.GetExecutingAssembly().GetName().Version);
                        writer.Write("-->");

                        writer.RenderBeginTag(HtmlTextWriterTag.Title);
                        {
                            writer.Write(title);
                        }
                        writer.RenderEndTag();

                        // <meta name="robots" content="noindex" />
                        writer.AddAttribute(HtmlTextWriterAttribute.Name, "robots");
                        writer.AddAttribute(HtmlTextWriterAttribute.Content, "noindex");
                        writer.RenderBeginTag(HtmlTextWriterTag.Meta);
                        writer.RenderEndTag();

                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                        writer.RenderBeginTag(HtmlTextWriterTag.Style);
                        {
                            writer.Write(@"
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
    width: 90%;
}

h1 {
    background-color: #1BA1E2;
    color: white;
    padding: 5px 20px;
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
}
");
                        }
                        writer.RenderEndTag();
                    }
                    writer.RenderEndTag();

                    writer.RenderBeginTag(HtmlTextWriterTag.Body);
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.H1);
                        {
                            writer.Write(title);
                        }
                        writer.RenderEndTag();

                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.P);
                            {
                                writer.Write("The following methods are supported:");
                            }
                            writer.RenderEndTag();

                            // Method Names
                            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                            {
                                foreach (var method in methods)
                                {
                                    // Method Name
                                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                                    {
                                        writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Concat("#", method.Value.Name));
                                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                                        {
                                            writer.Write(method.Value.Name);
                                        }
                                        writer.RenderEndTag();
                                    }
                                    writer.RenderEndTag();
                                }
                            }
                            writer.RenderEndTag();

                            foreach (var method in methods)
                            {
                                var mi = method.Value.MethodInfo;

                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                {
                                    // Method name
                                    writer.RenderBeginTag(HtmlTextWriterTag.H2);
                                    {
                                        writer.AddAttribute(HtmlTextWriterAttribute.Name, method.Value.Name);
                                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                                        {
                                            writer.Write(method.Value.Name);
                                        }
                                        writer.RenderEndTag();
                                    }
                                    writer.RenderEndTag();

                                    // "Parameters" headline
                                    writer.RenderBeginTag(HtmlTextWriterTag.H3);
                                    {
                                        writer.Write("Parameters");
                                    }
                                    writer.RenderEndTag();

                                    // "Parameters" table
                                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                                    {
                                        var parameters = mi.GetParameters();

                                        foreach (var parameter in parameters)
                                        {
                                            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                                            {
                                                writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:30%");
                                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                                {
                                                    writer.Write(parameter.ParameterType.Name);
                                                }
                                                writer.RenderEndTag();

                                                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                                {
                                                    writer.Write(parameter.Name);
                                                }
                                                writer.RenderEndTag();
                                            }
                                            writer.RenderEndTag();
                                        }
                                    }
                                    writer.RenderEndTag();

                                    // "Return Value" headline
                                    writer.RenderBeginTag(HtmlTextWriterTag.H3);
                                    {
                                        writer.Write("Return Value");
                                    }
                                    writer.RenderEndTag();

                                    // "Return Value" table
                                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                                    {
                                        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                                        {
                                            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:30%");
                                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                            {
                                                writer.Write(mi.ReturnType.Name);
                                            }
                                            writer.RenderEndTag();

                                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                                            {
                                                writer.Write(
                                                    !string.IsNullOrEmpty(method.Value.Description)
                                                        ? method.Value.Description
                                                        : "-");
                                            }
                                            writer.RenderEndTag();
                                        }
                                        writer.RenderEndTag();
                                    }
                                    writer.RenderEndTag();
                                }
                                writer.RenderEndTag();
                            }
                        }
                        writer.RenderEndTag();
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();

                Content = stringWriter.ToString();
                base.ExecuteResult(context);
            }
        }
    }
}
