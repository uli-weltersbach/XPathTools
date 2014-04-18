using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class XPathWriter
    {
        private const string PathPartSeparator = "/";
        private const string PredicateStart = "[";
        private const string PredicateEnd = "]";
        private const string And = " and ";
        private readonly IEnumerable<INodeFilter> _filters;
        private readonly StringBuilder _xpath = new StringBuilder();

        public XPathWriter()
            : this(Enumerable.Empty<INodeFilter>())
        {
        }

        public XPathWriter(IEnumerable<INodeFilter> filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");
            _filters = filters;
        }

        public string Write(XObject node)
        {
            var pathParts = GetPathParts(node);
            Write(pathParts);
            return _xpath.ToString();
        }

        private IEnumerable<XPathPart> GetPathParts(XObject node)
        {
            XElement selectedElement;
            if (IsElement(node))
                selectedElement = (XElement)node;
            else if (IsAttribute(node))
                selectedElement = node.Parent;
            else
                throw new ArgumentException("Node is not an element or attribute: " + node.GetType(), "node");

            var parts = new List<XPathPart>();
            var ancestorsAndSelf = selectedElement.AncestorsAndSelf().Reverse();
            foreach (XElement ancestor in ancestorsAndSelf)
            {
                var part = new XPathPart();
                part.Node = ancestor;
                part.Predicates = ancestor.Attributes().Where(MatchesAnyFilter).ToArray();
                parts.Add(part);
            }
            if (IsAttribute(node))
                parts.Add(new XPathPart { Node = node });
            return parts;
        }

        private bool MatchesAnyFilter(XObject node)
        {
            return _filters.Any(filter => filter.IsIncluded(node));
        }

        private void Write(IEnumerable<XPathPart> pathParts)
        {
            foreach (XPathPart pathPart in pathParts)
            {
                if (IsElement(pathPart))
                {
                    WritePathPartSeparator();
                    WriteElementName(pathPart.Node as XElement);
                    WritePredicates(pathPart);
                }
                else if (IsAttribute(pathPart))
                {
                    WritePathPartSeparator();
                    WriteAttributeName(pathPart.Node as XAttribute);
                }
            }
        }

        private bool IsElement(XPathPart pathPart)
        {
            return IsElement(pathPart.Node);
        }

        private bool IsElement(XObject node)
        {
            return node is XElement;
        }

        private void WritePathPartSeparator()
        {
            _xpath.Append(PathPartSeparator);
        }

        private void WriteElementName(XElement element)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
            {
                _xpath.Append(element.Name.LocalName);
                return;
            }
            string namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                _xpath.AppendFormat("*[local-name()='{0}' and namespace-uri()='{1}']", element.Name.LocalName, element.Name.NamespaceName);
            else
                _xpath.AppendFormat("{0}:{1}", namespacePrefix, element.Name.LocalName);
        }

        private void WritePredicates(XPathPart pathPart)
        {
            if (pathPart.Predicates == null)
                return;
            if (pathPart.Predicates.Count == 0)
                return;
            WritePredicateStart();
            foreach (var predicate in pathPart.Predicates)
            {
                WriteAttributeName(predicate);
                WriteAttributeValue(predicate);
                _xpath.Append(And);
            }
            _xpath.Remove(_xpath.Length - And.Length, And.Length);
            WritePredicateEnd();
        }

        private void WritePredicateStart()
        {
            bool isPredicateEnd = _xpath[_xpath.Length - PredicateEnd.Length].ToString() == PredicateEnd;
            if (isPredicateEnd)
            { 
                // "Open up" the predicate again.
                _xpath.Remove(_xpath.Length - PredicateEnd.Length, PredicateEnd.Length);
                _xpath.Append(And);
            }
            else
            { 
                // Start a new predicate.
                _xpath.Append(PredicateStart);
            }
        }

        private void WritePredicateEnd()
        {
            _xpath.Append(PredicateEnd);
        }

        private bool IsAttribute(XPathPart pathPart)
        {
            return pathPart.Node is XAttribute;
        }

        private bool IsAttribute(XObject node)
        {
            return node is XAttribute;
        }

        private void WriteAttributeName(XAttribute attribute)
        {
            _xpath.Append("@");
            if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
            {
                _xpath.Append(attribute.Name.LocalName);
                return;
            }

            if (attribute.Parent == null)
                throw new XmlException(string.Format("Unable to determine namespace prefix for attribute \"{0}\". Parent is null.", attribute.Name));

            string namespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                _xpath.Append(attribute.Name.LocalName);
            else
                _xpath.AppendFormat("{0}:{1}", namespacePrefix, attribute.Name.LocalName);
        }

        private void WriteAttributeValue(XAttribute attribute)
        {
            _xpath.AppendFormat("='{0}'", attribute.Value);
        }

        private class XPathPart
        {
            public XObject Node { get; set; }
            public IList<XAttribute> Predicates { get; set; }
        }
    }
}