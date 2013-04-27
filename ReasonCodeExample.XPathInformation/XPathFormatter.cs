using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathFormatter
    {
        public string Format(XElement element)
        {
            return element == null ? string.Empty : GetXPath(element);
        }

        private string GetXPath(XElement element)
        {
            return element.AncestorsAndSelf().Reverse().Select(GetElementName).Aggregate(string.Empty, ConcatenateXPath);
        }

        private string GetElementName(XElement element)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
                return element.Name.LocalName;
            string namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return element.Name.LocalName;
            return namespacePrefix + ":" + element.Name.LocalName;
        }

        private string ConcatenateXPath(string current, string next)
        {
            return current + "/" + next;
        }
    }
}