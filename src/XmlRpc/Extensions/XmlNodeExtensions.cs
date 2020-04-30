using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AspNetCore.XmlRpc.Extensions
{
    public static class XmlNodeExtensions
    {
        // http://stackoverflow.com/questions/241238/how-to-get-xpath-from-an-xmlnode-instance-c
        public static string GetXPath(this XmlNode instance)
        {
            var builder = new StringBuilder();
            while (instance != null)
            {
                switch (instance.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + instance.Name);
                        instance = ((XmlAttribute)instance).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)instance);
                        builder.Insert(0, "/" + instance.Name + "[" + index + "]");
                        instance = instance.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        private static int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }

            var parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }


        // https://stackoverflow.com/questions/451950/get-the-xpath-to-an-xelement/23541182#23541182
        /// <summary>
        /// Get the absolute XPath to a given XElement, including the namespace.
        /// (e.g. "/a:people/b:person[6]/c:name[1]/d:last[1]").
        /// </summary>
        public static string GetXPath(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }


            Func<XElement, string> relativeXPath = e =>
            {
                int index = e.IndexPosition();

                var currentNamespace = e.Name.Namespace;

                string name;
                if (String.IsNullOrEmpty(currentNamespace.ToString()))
                {
                    name = e.Name.LocalName;
                }
                else
                {
                    name = "*[local-name()='" + e.Name.LocalName + "']";
                        //string namespacePrefix = e.GetPrefixOfNamespace(currentNamespace);
                        //name = namespacePrefix + ":" + e.Name.LocalName;
                    }

                    // If the element is the root or has no sibling elements, no index is required
                    return ((index == -1) || (index == -2)) ? "/" + name : string.Format
                            (
                                "/{0}[{1}]",
                                name,
                                index.ToString()
                            );
            };

            var ancestors = from e in element.Ancestors()
                            select relativeXPath(e);

            return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPath(element);
        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned or -2 if element has no sibling elements.
        /// </summary>
        /// <param name="element">
        /// The element to get the index of.
        /// </param>
        public static int IndexPosition(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Parent == null)
            {
                // Element is root
                return -1;
            }

            if (element.Parent.Elements(element.Name).Count() == 1)
            {
                // Element has no sibling elements
                return -2;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            foreach (var sibling in element.Parent.Elements(element.Name))
            {
                if (sibling == element)
                {
                    return i;
                }

                i++;
            }

            throw new InvalidOperationException
                ("element has been removed from its parent.");
        }
    }
}
