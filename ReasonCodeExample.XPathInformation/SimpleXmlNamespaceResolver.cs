using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathTools
{
    public class SimpleXmlNamespaceResolver : IXmlNamespaceResolver
    {
        private readonly IDictionary<string, string> _namespaces = new Dictionary<string, string>();

        public SimpleXmlNamespaceResolver(XDocument document)
        {
            if (document?.Root == null)
            {
                return;
            }

            foreach (var element in document.Root.DescendantsAndSelf())
            {
                var elementNamespace = element.Name.Namespace.ToString();
                if(!string.IsNullOrWhiteSpace(element.Name.NamespaceName) && !_namespaces.ContainsKey(elementNamespace))
                {
                    var elementNamespacePrefix = GetNamespacePrefix(element);
                    _namespaces.Add(elementNamespace, elementNamespacePrefix);
                }

                foreach(var attribute in element.Attributes())
                {
                    string attributeNamespace;
                    string attributeNamespacePrefix;
                    if(attribute.IsNamespaceDeclaration)
                    {
                        attributeNamespace = attribute.Value;
                        attributeNamespacePrefix = GetNamespacePrefixFromXmlnsDeclaration(attribute);
                    }
                    else
                    {
                        attributeNamespace = attribute.Name.Namespace.ToString();
                        attributeNamespacePrefix = GetNamespacePrefix(attribute);
                    }
                    if(!string.IsNullOrEmpty(attributeNamespace) && !_namespaces.ContainsKey(attributeNamespace))
                    {
                        _namespaces.Add(attributeNamespace, attributeNamespacePrefix);
                    }
                }
            }
        }

        private string GetNamespacePrefix(XElement element)
        {
            var match = Regex.Match(element.ToString(SaveOptions.DisableFormatting), @"^<(?'NamespacePrefix'\w+):");
            return match.Success ? match.Groups["NamespacePrefix"].Value : string.Empty;
        }

        private string GetNamespacePrefixFromXmlnsDeclaration(XAttribute attribute)
        {
            var attr = attribute.ToString();
            var match = Regex.Match(attr, @"^xmlns:(?'NamespacePrefix'\w+)=");
            return match.Success ? match.Groups["NamespacePrefix"].Value : string.Empty;
        }

        private string GetNamespacePrefix(XAttribute attribute)
        {
            var attr = attribute.ToString();
            var match = Regex.Match(attr, @"^(?'NamespacePrefix'\w+):");
            return match.Success ? match.Groups["NamespacePrefix"].Value : string.Empty;
        }

        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            return _namespaces;
        }

        public string LookupNamespace(string prefix)
        {
            return _namespaces.Where(kvp => kvp.Value == prefix).Select(kvp => kvp.Key).FirstOrDefault();
        }

        public string LookupPrefix(string namespaceName)
        {
            return _namespaces.ContainsKey(namespaceName) ? _namespaces[namespaceName] : null;
        }
    }
}