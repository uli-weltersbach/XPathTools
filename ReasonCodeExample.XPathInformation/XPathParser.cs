using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathParser
    {
        private XElement _currentElement;

        public string Parse(string xmlFragment)
        {
            IEnumerable<string> elements = GetElements(xmlFragment);
            foreach (string element in elements)
            {
                CreateElement(element);
            }
            return CreateXPath(_currentElement);
        }

        private IEnumerable<string> GetElements(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return Enumerable.Empty<string>();
            return xml.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries).Select(element => "<" + element).ToArray();
        }

        private void CreateElement(string elementText)
        {
            XName name = GetElementName(elementText);
            if (name == null)
            {
                return;
            }

            XElement element = new XElement(name);
            if (_currentElement == null)
            {
                _currentElement = element;
                return;
            }

            if (IsClosedTag(elementText))
            {
                _currentElement.Add(element);
                return;
            }

            if (IsClosingTag(elementText))
            {
                if (HasParent(_currentElement))
                {
                    _currentElement = _currentElement.Parent;
                }
                return;
            }

            _currentElement.Add(element);
            _currentElement = element;
        }

        private XName GetElementName(string elementText)
        {
            Regex elementNameRegex = new Regex(@"</?(\w+:)?(?'ElementName'\w+)");
            Match elementNameMatch = elementNameRegex.Match(elementText);
            if (!elementNameMatch.Success)
                return null;
            string elementName = elementNameMatch.Groups["ElementName"].Value;
            string namespaceName = GetNamespaceName(elementText);
            return string.IsNullOrEmpty(namespaceName) ? XName.Get(elementName) : XName.Get(elementName, namespaceName);
        }

        private string GetNamespaceName(string elementText)
        {
            return GetNamespaceFromPrefix(elementText) ??
                   GetNamespaceFromAttribute(elementText) ??
                   GetNamespaceFromAncestor();
        }

        private string GetNamespaceFromPrefix(string elementText)
        {
            Regex namespaceRegex = new Regex(@"</?(?'Namespace'\w+):");
            Match namespaceMatch = namespaceRegex.Match(elementText);
            return namespaceMatch.Success ? namespaceMatch.Groups["Namespace"].Value : null;
        }

        private string GetNamespaceFromAttribute(string elementText)
        {
            Regex namespaceAttributeRegex = new Regex(@"xmlns:(?'Namespace'\w+)=");
            Match namespaceAttributeMatch = namespaceAttributeRegex.Match(elementText);
            return namespaceAttributeMatch.Success ? namespaceAttributeMatch.Groups["Namespace"].Value : null;
        }

        private string GetNamespaceFromAncestor()
        {
            if (_currentElement == null)
                return null;
            XElement current = _currentElement;
            string namespaceName = null;
            while (string.IsNullOrEmpty(namespaceName) && HasParent(current))
            {
                namespaceName = current.Name.NamespaceName;
                current = current.Parent;
            }
            return namespaceName;
        }

        private bool IsClosedTag(string elementText)
        {
            return elementText.EndsWith("/>");
        }

        private bool IsClosingTag(string elementText)
        {
            return elementText.StartsWith("</");
        }

        private bool HasParent(XElement element)
        {
            return element.Parent != null;
        }

        private string CreateXPath(XElement element)
        {
            if (element == null)
                return string.Empty;
            if (element.HasElements)
                element = element.Elements().Last();
            return element.AncestorsAndSelf()
                          .Reverse()
                          .Select(node => node.Name)
                          .Aggregate(string.Empty, (current, next) => current + "/" + next)
                          .Replace("{", string.Empty)
                          .Replace("}", ":");
        }
    }
}