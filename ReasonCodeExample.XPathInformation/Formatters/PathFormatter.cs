using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class PathFormatter
    {
        /// <summary>
        /// Returns the XPath of the element. E.g. "/configuration/ns:settings".
        /// </summary>
        public virtual string Format(XElement element)
        {
            if (element == null)
                return string.Empty;
            return GetElementXPath(element);
        }

        protected virtual string GetElementXPath(XElement element)
        {
            return element.AncestorsAndSelf().Reverse().Select(GetElementName).Aggregate(string.Empty, ConcatenateXPath);
        }

        protected virtual string GetElementName(XElement element)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
                return element.Name.LocalName;

            string namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return element.Name.LocalName;

            return namespacePrefix + ":" + element.Name.LocalName;
        }

        protected virtual string ConcatenateXPath(string current, string next)
        {
            return current + "/" + next;
        }

        /// <summary>
        /// Returns the local XPath of the attribute. E.g. "[@ns:name]".
        /// </summary>
        public virtual string Format(XAttribute attribute)
        {
            if (attribute == null)
                return string.Empty;

            if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                return string.Format("[@{0}]", attribute.Name.LocalName);

            if (attribute.Parent == null)
                throw new XmlException(string.Format("Unable to determine namespace prefix for attribute \"{0}\". Parent is null.", attribute.Name));

            string namespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return string.Format("[@{0}]", attribute.Name.LocalName);

            return string.Format("[@{0}:{1}]", namespacePrefix, attribute.Name.LocalName);
        }
    }
}