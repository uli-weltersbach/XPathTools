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
            IEnumerable<string> elementParts = SplitElementParts(xmlFragment);
            foreach (string elementPart in elementParts)
            {
                CreateElement(elementPart);
            }
            if (_currentElement.HasElements)
            {
                _currentElement = _currentElement.Elements().Last();
            }
            return CreateXPath(_currentElement);
        }

        private IEnumerable<string> SplitElementParts(string xml)
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
            Regex elementRegex = new Regex(@"(?'NamespaceName'\w+:)?(?'ElementName'\w+)");
            Match elementMatch = elementRegex.Match(elementText);
            if (!elementMatch.Success)
                return null;

            Group namespaceNameGroup = elementMatch.Groups["NamespaceName"];
            Group elementNameGroup = elementMatch.Groups["ElementName"];
            if (namespaceNameGroup.Success && elementNameGroup.Success)
            {
                return XName.Get(elementNameGroup.Value, namespaceNameGroup.Value.Replace(":", string.Empty));
            }
            if (elementNameGroup.Success)
            {
                return XName.Get(elementNameGroup.Value);
            }
            return null;
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
            return element.AncestorsAndSelf()
                          .Reverse()
                          .Select(node => node.Name)
                          .Aggregate(string.Empty, (current, next) => current + "/" + next)
                          .Replace("{", string.Empty)
                          .Replace("}", ":");
        }
    }
}