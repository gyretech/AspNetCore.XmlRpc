using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.XmlRpc.Extensions
{
    internal static class ObjectExtensions
    {
        public static object ConvertTo(this object value, string typeName)
        {
            switch (typeName)
            {
                case "int":
                case "i4":
                    try
                    {
                        value = Convert.ToInt32(value);
                    }
                    catch (Exception)
                    {
                        value = default(int);
                    }
                    break;
                case "double":
                    try
                    {
                        value = Convert.ToDouble(value);
                    }
                    catch (Exception)
                    {
                        value = default(double);
                    }
                    break;
                case "boolean":
                    try
                    {
                        value = bool.Parse(value.ToString());
                    }
                    catch (FormatException)
                    {
                        value = (string)value == "1";
                    }
                    break;
                case "dateTime.iso8601":
                    value = ((string) value).ConvertToDateTime();
                    break;
                case "base64":
                    value = Convert.FromBase64String((string)value);
                    break;
                default:
                    value = Convert.ToString(value);
                    break;
            }

            return value;
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static Dictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
