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
            IEnumerable<string> elementParts = GetElementParts(xmlFragment);
            foreach (string elementPart in elementParts)
            {
                XElement element = CreateElement(elementPart);
                AddElement(elementPart, element);
            }
            return CreateXPath(_currentElement);
        }

        private IEnumerable<string> GetElementParts(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return Enumerable.Empty<string>();
            return xml.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries).Select(element => "<" + element.Trim()).ToArray();
        }

        private XElement CreateElement(string elementText)
        {
            string elementName = GetElementName(elementText);
            if (string.IsNullOrEmpty(elementName))
                return null;
            string namespaceName = GetNamespaceName(elementText);
            XName name = string.IsNullOrEmpty(namespaceName) ? XName.Get(elementName) : XName.Get(elementName, namespaceName);
            return new XElement(name);
        }

        private string GetElementName(string elementText)
        {
            Regex elementNameRegex = new Regex(@"</?(\w+:)?(?'ElementName'\w+)");
            Match elementNameMatch = elementNameRegex.Match(elementText);
            return elementNameMatch.Success ? elementNameMatch.Groups["ElementName"].Value : null;
        }

        private string GetNamespaceName(string elementText)
        {
            Regex namespaceRegex = new Regex(@"</?(?'Namespace'\w+):");
            Match namespaceMatch = namespaceRegex.Match(elementText);
            return namespaceMatch.Success ? namespaceMatch.Groups["Namespace"].Value : null;
        }

        private void AddElement(string elementPart, XElement element)
        {
            if (element == null)
                return;

            if (_currentElement == null)
            {
                _currentElement = element;
                return;
            }

            if (IsClosedTag(elementPart))
            {
                _currentElement.Add(element);
                return;
            }

            if (IsClosingTag(elementPart))
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