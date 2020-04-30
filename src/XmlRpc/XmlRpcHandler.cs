using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using AspNetCore.XmlRpc.Extensions;

namespace AspNetCore.XmlRpc
{
    public class XmlRpcHandler : IXmlRpcHandler
    {
        public XmlRpcHandler()
        {

        }

        public bool CanProcess(XmlRpcContext context)
        {
            return context.HttpContext.Request.Path.StartsWithSegments(context.Options.Endpoint);
        }

        public Task ProcessRequestAsync(XmlRpcContext context)
        {
            var request = context.HttpContext.Request;

            if (!request.Method.Equals(
                    HttpVerbs.Post.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
                //throw new InvalidOperationException();
            }

            var requestInfo =
                XmlRpcRequestParser.GetRequestInformation(
                    request.Body);


            if (string.IsNullOrWhiteSpace(requestInfo.MethodName))
            {
                throw new InvalidOperationException(
                    "XmlRpc call does not contain a method.");
            }

            var methodInfo =
                XmlRpcRequestParser.GetRequestedMethod(requestInfo, context.Services);

            if (methodInfo == null)
            {
                throw new InvalidOperationException(
                    string.Concat(
                        "There was no implementation of IXmlRpcService ",
                        "found, that contains a method decorated with ",
                        " the XmlRpcMethodAttribute value'",
                        requestInfo.MethodName,
                        "'."));
            }

            var result =
                XmlRpcRequestParser.ExecuteRequestedMethod(
                    requestInfo, methodInfo, context);

            var response = context.HttpContext.Response;
            response.ContentType = "text/xml";

            var settings =
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    Encoding = new UTF8Encoding(false), // Get rid of BOM
                    Indent = true,
                    Async = true
                };

            var writer = XmlWriter.Create(response.Body, settings);

            if (methodInfo.ResponseType == XmlRpcResponseType.Wrapped)
            {
                WriteWrappedResponseAsync(writer, result).GetAwaiter().GetResult();

                writer.FlushAsync().ConfigureAwait(false);

                return Task.CompletedTask;
            }

            WriteRawResponseAsync(writer, result).GetAwaiter().GetResult();

            writer.FlushAsync().ConfigureAwait(false);

            return Task.CompletedTask;

        }

        private bool _generateServiceOverview = true;
        public bool GenerateServiceOverview
        {
            get { return _generateServiceOverview; }
            set { _generateServiceOverview = value; }
        }

        private async Task WriteRawResponseAsync(
            XmlWriter output,
            dynamic result)
        {
            await output.WriteStartDocumentAsync();
            {
                await output.WriteStartElementAsync("","response","");
                {
                    WriteObject(output, result);
                }
                output.WriteEndElement();
            }
            await output.WriteEndDocumentAsync();
        }

        private async Task WriteWrappedResponseAsync(
            XmlWriter output,
            dynamic result)
        {
            await output.WriteStartDocumentAsync();
            {
                await output.WriteStartElementAsync("","methodResponse","");
                {
                    await output.WriteStartElementAsync("","params","");
                    {
                        await output.WriteStartElementAsync("","param","");
                        {
                            await output.WriteStartElementAsync("","value","");
                            {
                                WriteObject(output, result);
                            }
                            await output.WriteEndElementAsync();
                        }
                        await output.WriteEndElementAsync();
                    }
                    await output.WriteEndElementAsync();
                }
                await output.WriteEndElementAsync();
            }
            await output.WriteEndDocumentAsync();
        }

        private void WriteObject(
            XmlWriter xmlWriter,
            dynamic result)
        {
            Type type = result.GetType();
            if (type.IsPrimitive())
            {
                xmlWriter.WrapOutgoingType((object)result);
            }
            else if (type.IsArray)
            {
                WriteArray(xmlWriter, result);
            }
            else if (!type.IsPrimitive && type.IsClass)
            {
                WriteClass(xmlWriter, type, result);
            }
        }

        private void WriteClass(
            XmlWriter xmlWriter,
            Type type,
            object obj)
        {
            xmlWriter.WriteStartElement("struct");

            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(obj, null);
                if (value == null)
                    continue;

                xmlWriter.WriteStartElement("member");
                {
                    xmlWriter.WriteStartElement("name");
                    {
                        xmlWriter.WriteString(property.GetSerializationName());
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("value");
                    {
                        WriteObject(xmlWriter, value);
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }

            // struct
            xmlWriter.WriteEndElement();
        }

        private void WriteArray(XmlWriter xmlWriter, dynamic obj)
        {
            xmlWriter.WriteStartElement("array");
            {
                xmlWriter.WriteStartElement("data");
                {
                    foreach (var resultEntry in obj)
                    {
                        xmlWriter.WriteStartElement("value");
                        {
                            WriteObject(xmlWriter, resultEntry);
                        }
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
    }
}