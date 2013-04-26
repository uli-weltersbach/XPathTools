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
        private IList<string> _elements;
        private int _index;

        public string Parse(string xmlFragment)
        {
            _elements = xmlFragment.Split(new[] { '<' }, StringSplitOptions.RemoveEmptyEntries);
            for (_index = 0; _index < _elements.Count; _index++)
            {
                string element = _elements[_index];
                AddOrSkipElement(element);
            }
            return _currentElement
                .AncestorsAndSelf()
                .Reverse()
                .Select(node => node.Name)
                .Aggregate(string.Empty, (current, next) => current + "/" + next)
                .Replace("{", string.Empty)
                .Replace("}", ":");
        }

        private void AddOrSkipElement(string elementText)
        {
            XName name = GetElementName(elementText);
            if (name == null)
                return;

            bool isClosedElement = elementText.EndsWith("/>");
            XElement element = new XElement(name);
            if (_currentElement == null)
            {
                _currentElement = element;
            }
            else if (isClosedElement)
            {
                _currentElement.Add(element);
            }
            else
            {
                _currentElement.Add(element);
                _currentElement = element;
            }

            bool isClosingTag = elementText.StartsWith("</");
            if (isClosingTag && _currentElement.Parent != null)
            {
                _currentElement = _currentElement.Parent;
            }
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
    }
}