using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using AspNetCore.XmlRpc.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AspNetCore.XmlRpc
{
    public static class XmlRpcRequestParser
    {
        internal static readonly Type _s_rpca = typeof(XmlRpcMethodAttribute);
        internal static readonly Type _s_rpcs = typeof(IXmlRpcService);

        internal static readonly Func<Type, bool> _s_isRpcService =
            type => (type.IsClass || type.IsInterface) &&
                    _s_rpcs.IsAssignableFrom(type);

        internal static readonly Func<MethodInfo, XmlRpcMethodAttribute>
            _s_getRpcAttribute =
                method =>
                method
                    .GetCustomAttributes(_s_rpca, true)
                    .Cast<XmlRpcMethodAttribute>()
                    .FirstOrDefault();

        internal static readonly
            Func<MethodInfo[], IEnumerable<XmlRpcMethodDescriptor>>
            _s_getXmlRpcMethods =
                methods =>
                from method in methods
                let attribute = _s_getRpcAttribute(method)
                where attribute != null
                select
                    new XmlRpcMethodDescriptor(
                    attribute.MethodName,
                    attribute.MethodDescription,
                    attribute.ResponseType,
                    method);

        internal static readonly Func<Type, Type>
            _s_getImplementation =
                contract =>
                Directory.EnumerateFiles(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"), "*.dll")
                    .Where(x => filterAssemblies(x))
                    .SelectMany(path => Assembly.LoadFrom(path).GetTypes())
                    .Where(type => !type.IsInterface)
                    .FirstOrDefault(type => contract.IsAssignableFrom(type));

        internal static readonly string[] s_blacklist =
            new[]
                {
                    "System.", "Microsoft.", "NHibernate."
                };

        internal static readonly Func<string, bool> filterAssemblies =
            assembly =>
            !s_blacklist.Any(
                x =>
                Path.GetFileName(assembly)
                    .StartsWith(x, StringComparison.OrdinalIgnoreCase));

        public static Dictionary<string, XmlRpcMethodDescriptor> GetMethods(
            Type[] services)
        {
            var types =
                services != null && services.Length > 0
                    ? services
                    : Directory.EnumerateFiles(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"), "*.dll")
                          .Where(x => filterAssemblies(x))
                          .SelectMany(path => Assembly.LoadFrom(path).GetTypes());

            return
                types
                    .Where(_s_isRpcService)
                    .Select(type => type.GetMethods())
                    .SelectMany(_s_getXmlRpcMethods)
                    .ToDictionary(desc => desc.Name, desc => desc);
        }

        internal static XmlRpcMethodDescriptor GetRequestedMethod(
            XmlRpcRequest request, Type[] services)
        {
            var methods = GetMethods(services);

            XmlRpcMethodDescriptor descriptor;
            methods.TryGetValue(request.MethodName, out descriptor);
            return descriptor;
        }

        internal static object ExecuteRequestedMethod(
            XmlRpcRequest request,
            XmlRpcMethodDescriptor methodDescriptor, XmlRpcContext context)
        {
            var parameters = new List<object>();
            var method = methodDescriptor.MethodInfo;
            var requiredParameters = method.GetParameters();

            // Validate we have the right amount of parameters

            for (var i = 0; i < requiredParameters.Length; i++)
            {
                var parameter = requiredParameters[i];
                var type = parameter.ParameterType;

                if (type.IsPrimitive())
                {
                    try
                    {
                        var obj = Convert.ChangeType(request.Parameters[i], type);
                        parameters.Add(obj);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Add the default value for the requested type.
                        parameters.Add(Activator.CreateInstance(type));
                    }
                    catch (Exception)
                    {
                        parameters.Add(request.Parameters[i]);
                    }
                }
                else if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    object[] arrayParameters;
                    
                    try
                    {
                        arrayParameters = (object[])request.Parameters[i];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        var defaultArrayType = Array.CreateInstance(elementType,1);;

                        arrayParameters = (object[]) defaultArrayType;
                    }
                    catch (Exception)
                    {
                        arrayParameters = (object[])request.Parameters[i];
                    }
                    
                    var listType = typeof(List<>);
                    var genericListType = listType.MakeGenericType(elementType);
                    var list = Activator.CreateInstance(genericListType);

                    foreach (Dictionary<string, object> values in arrayParameters)
                    {
                        object element;

                        try
                        {
                            element = Activator.CreateInstance(elementType);
                        }
                        catch (MissingMethodException)
                        {
                            list.GetType()
                                .GetMethod("Add")
                                .Invoke(
                                    list,
                                    new[] { "" });

                            continue;
                        }
                        
                        foreach (var property in element.GetType().GetProperties())
                        {
                            var nameKey = property.GetSerializationName();

                            if (values.TryGetValue(nameKey, out object value))
                            {
                                property.SetValue(element, value);
                            }
                        }

                        list.GetType()
                            .GetMethod("Add")
                            .Invoke(
                                list,
                                new[] { element });
                    }

                    parameters.Add(
                        list.GetType()
                            .GetMethod("ToArray")
                            .Invoke(list, null));
                }
                else
                {
                    Dictionary<string, object> complexInstanceParameters;

                    try
                    {
                        complexInstanceParameters = (Dictionary<string, object>) request.Parameters[i];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        var defaultType = Activator.CreateInstance(type);
                        var dic = defaultType.AsDictionary();
                        //complexInstanceParameters = (Dictionary<string, object>) Activator.CreateInstance(type);
                        complexInstanceParameters = defaultType.AsDictionary();
                    }
                    catch (Exception)
                    {
                        complexInstanceParameters = (Dictionary<string, object>) request.Parameters[i];
                    }

                    var complexInstance = Activator.CreateInstance(type);

                    foreach (var property in complexInstance.GetType().GetProperties())
                    {
                        var nameKey = property.GetSerializationName();

                        if (!complexInstanceParameters.TryGetValue(
                            nameKey,
                            out object value)) continue;
                        //TODO: to make it more generic, the whole function needs to be rewritten using similar approach as above. Not changing as the whole function needs to be refactored.
                        var propertyType = property.PropertyType;
                        if (propertyType.IsArray)
                        {
                            property.SetValue(complexInstance, ConvertValues(propertyType, value));
                        }
                        else
                            property.SetValue(complexInstance, value);
                    }

                    parameters.Add(complexInstance);
                }
            }

            var instanceType = method.DeclaringType;

            if (method.DeclaringType != null &&
                method.DeclaringType.IsInterface)
            {
                instanceType = _s_getImplementation(method.DeclaringType);
            }

            // ControllerContext is not avalilable in ASP.NET Core and all instance should be instantiated directly through instanceType though controller name and action name can be retrieved through context.
            using (var scope = context.ServiceScopeFactory.CreateScope())
            {
                var instance = scope.ServiceProvider.GetRequiredService(instanceType);
                try
                {
                    Trace.TraceInformation(
                        "XmlRpcMvc: Invoking method {0} with '{1}'",
                        method.Name,
                        string.Join(", ", parameters));

                    return method.Invoke(instance, parameters.ToArray());
                }
                catch (XmlRpcFaultException exception)
                {
                    return exception;
                }
            }
        }

        /// <summary>
        /// Convert arrays
        /// </summary>
        /// <param name="arrayPropertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static dynamic ConvertValues(Type arrayPropertyType, object value)
        {
            var values = value as IEnumerable;

            var elementType = arrayPropertyType.GetElementType();

            var listType = typeof(List<>);
            var genericListType = listType.MakeGenericType(elementType);

            var list = Activator.CreateInstance(genericListType);
            foreach (var obj in values)
            {
                list.GetType().GetMethod("Add").Invoke(list, new[] { obj });
            }

            return list.GetType().GetMethod("ToArray").Invoke(list, null);
        }

        internal static XmlRpcRequest GetRequestInformation(Stream xml)
        {
            var request = new XmlRpcRequest();

            // Get the document
            var _doc = XDocument.LoadAsync(xml, LoadOptions.PreserveWhitespace, default).GetAwaiter().GetResult();

            //  Get the method
            var mName = _doc.XPathSelectElement("methodCall/methodName");

            if(mName != null)
            {
                request.MethodName = mName.Value;
                Trace.TraceInformation("Incoming XML-RPC call for method: {0}", request.MethodName);
            }

            var mParameters = _doc.XPathSelectElements("methodCall/params/param/value/*");
            if(mParameters != null)
            {
                request.Parameters = new List<object>();
                foreach(var node in mParameters)
                {
                    request.Parameters.Add(GetMethodMember(request, node));
                }
            }

            return request;
            
        }

        internal static object GetMethodMember(XmlRpcRequest request, XElement node)
        {
            switch (node.Name.LocalName)
            {
                case "array":
                    return GetMethodArrayMember(request, node);

                case "struct":
                    return GetMethodStructMember(request, node);

                default:
                    return node.Value.ConvertTo(node.Name.LocalName);
            }
        }

        internal static object GetMethodArrayMember(
            XmlRpcRequest request,
            XElement node)
        {
            var xpath = node.GetXPath();
            var values = node.XPathSelectElements(string.Concat(xpath, "/data/value"));

            var results = new List<object>();

            if (values != null)
            {
                results.AddRange(
                    values.Cast<XElement>().Select(
                        value =>
                        value.Name.LocalName.Equals("struct")
                            ? GetMethodMember(request, value)
                            : value.Value.ConvertTo(value.Name.LocalName)));

                return results.ToArray();
            }

            return null;
        }

        internal static Dictionary<string, object> GetMethodStructMember(
            XmlRpcRequest request,
            XElement node)
        {
            var xpath = node.GetXPath();
            var values = node.XPathSelectElements(string.Concat(xpath, "/member"));

            if (values != null)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (XElement value in values)
                {
                    var memberNameNode = value.Name;
                    var memberValueNode = value;

                    if (memberNameNode == null || memberValueNode == null)
                    {
                        continue;
                    }

                    var memberName = memberNameNode.LocalName;

                    dictionary.Add(
                        memberName,
                        GetMethodMember(
                            request,
                            memberValueNode));
                }

                return dictionary;
            }

            return null;
        }


        #region OldCode

        internal static object GetMethodMember(
            XmlRpcRequest request,
            XmlNode node)
        {
            switch (node.Name)
            {
                case "array":
                    return GetMethodArrayMember(request, node);

                case "struct":
                    return GetMethodStructMember(request, node);

                default:
                    return node.InnerText.ConvertTo(node.Name);
            }
        }

        internal static Dictionary<string, object> GetMethodStructMember(
            XmlRpcRequest request,
            XmlNode node)
        {
            var xpath = node.GetXPath();
            var values = node.SelectNodes(string.Concat(xpath, "/member"));

            if (values != null)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (XmlNode value in values)
                {
                    var memberNameNode = value["name"];
                    var memberValueNode = value["value"];

                    if (memberNameNode == null || memberValueNode == null)
                    {
                        continue;
                    }

                    var memberName = memberNameNode.InnerText;

                    dictionary.Add(
                        memberName,
                        GetMethodMember(
                            request,
                            memberValueNode.FirstChild));
                }

                return dictionary;
            }

            return null;
        }

        internal static object GetMethodArrayMember(
            XmlRpcRequest request,
            XmlNode node)
        {
            var xpath = node.GetXPath();
            var values = node.SelectNodes(string.Concat(xpath, "/data/value"));

            var results = new List<object>();

            if (values != null)
            {
                results.AddRange(
                    values.Cast<XmlNode>().Select(
                        value =>
                        value.FirstChild.Name.Equals("struct")
                            ? GetMethodMember(request, value.FirstChild)
                            : value.InnerText.ConvertTo(value.FirstChild.Name)));

                //if (values
                //    .Cast<XmlNode>()
                //    .Any(x => x.FirstChild.Name.Equals("struct")))
                //{
                //    return values
                //        .Cast<XmlNode>()
                //        .Select(
                //            value =>
                //            GetMethodStructMember(
                //                request,
                //                value.FirstChild))
                //        .ToArray();
                //}

                //return values.Cast<XmlNode>()
                //    .Select(
                //        value =>
                //        (string) value.InnerText.ConvertTo(value.FirstChild.Name))
                //    .ToArray();


                // This works!!!
                //return values.Cast<XmlNode>()
                //    .Select(
                //        value =>
                //        (string)value.InnerText.ConvertTo(value.FirstChild.Name))
                //    .ToArray();

                return results.ToArray();
            }

            return null;
        }

        #endregion
    }
}