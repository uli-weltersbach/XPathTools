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
            if (document == null)
                return;
            if (document.Root == null)
                return;
            foreach (var element in document.Root.DescendantsAndSelf())
            {
                if (!_namespaces.ContainsKey(element.Name.NamespaceName))
                    _namespaces.Add(element.Name.Namespace.ToString(), GetNamespacePrefix(element));
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
    }
}