using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    public class SimpleXmlNamespaceResolver : IXmlNamespaceResolver
    {
        private readonly IDictionary<string, string> _namespaces = new Dictionary<string, string>();

        public SimpleXmlNamespaceResolver(XDocument document)
        {
            if(document == null)
                return;
            if(document.Root == null)
                return;
            foreach(var element in document.Root.DescendantsAndSelf())
            {
                if (!string.IsNullOrWhiteSpace(element.Name.NamespaceName) && !_namespaces.ContainsKey(element.Name.Namespace.ToString()))
                {
                    var elementPrefix = GetNamespacePrefix(element);
                    _namespaces.Add(element.Name.Namespace.ToString(), elementPrefix);
                }

                foreach(var attribute in element.Attributes())
                {
                    var prefix = GetNamespacePrefix(attribute);
                    if(string.IsNullOrEmpty(prefix))
                        continue;

                    string namespaceName = null;
                    if(attribute.IsNamespaceDeclaration)
                    {
                        namespaceName = attribute.Value;
                    }
                    else
                    {
                        namespaceName = attribute.Name.NamespaceName;
                    }
                    if (!_namespaces.ContainsKey(namespaceName))
                        _namespaces.Add(namespaceName, GetNamespacePrefix(attribute));
                }
            }
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

        private string GetNamespacePrefix(XElement element)
        {
            var match = Regex.Match(element.ToString(SaveOptions.DisableFormatting), @"^<(?'NamespacePrefix'\w+):");
            return match.Success ? match.Groups["NamespacePrefix"].Value : string.Empty;
        }

        private string GetNamespacePrefix(XAttribute attribute)
        {
            var attr = attribute.ToString();
            var match = Regex.Match(attr, @"^xmlns:(?'NamespacePrefix'\w+)=");
            return match.Success ? match.Groups["NamespacePrefix"].Value : string.Empty;
        }
    }
}