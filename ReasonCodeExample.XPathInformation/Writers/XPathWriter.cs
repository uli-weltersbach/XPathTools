using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class XPathWriter : IWriter
    {
        private const string PathPartSeparator = "/";
        private const string PredicateStart = "[";
        private const string PredicateEnd = "]";
        private readonly IEnumerable<IAttributeFilter> _filters;

        public XPathWriter()
            : this(Enumerable.Empty<IAttributeFilter>())
        {
        }

        public XPathWriter(IEnumerable<IAttributeFilter> filters)
        {
            if(filters == null)
            {
                throw new ArgumentNullException("filters");
            }
            _filters = filters;
        }

        protected StringBuilder XPath
        {
            get;
            private set;
        }

        public string Write(XObject node)
        {
            if(node == null)
            {
                return string.Empty;
            }
            XPath = new StringBuilder();
            var pathParts = GetPathParts(node);
            Write(pathParts);
            return XPath.ToString();
        }

        private IEnumerable<XPathPart> GetPathParts(XObject node)
        {
            XElement selectedElement;
            if(IsElement(node))
            {
                selectedElement = (XElement)node;
            }
            else if(IsAttribute(node))
            {
                selectedElement = node.Parent;
            }
            else
            {
                throw new ArgumentException("Node is not an element or attribute: " + node.GetType(), "node");
            }

            var parts = new List<XPathPart>();
            var ancestorsAndSelf = selectedElement.AncestorsAndSelf().Reverse();
            foreach(var ancestor in ancestorsAndSelf)
            {
                var part = new XPathPart();
                part.Node = ancestor;
                part.Predicates = ancestor.Attributes().Where(MatchesAnyFilter).ToArray();
                parts.Add(part);
            }
            if(IsAttribute(node))
            {
                parts.Add(new XPathPart {Node = node});
            }
            return parts;
        }

        private bool IsElement(XObject node)
        {
            return node is XElement;
        }

        private bool IsAttribute(XObject node)
        {
            return node is XAttribute;
        }

        private bool MatchesAnyFilter(XAttribute attribute)
        {
            return _filters.Any(filter => filter.IsIncluded(attribute));
        }

        private void Write(IEnumerable<XPathPart> pathParts)
        {
            foreach(var pathPart in pathParts)
            {
                if(IsElement(pathPart))
                {
                    WritePathPartSeparator();
                    WriteElementName(pathPart.Node as XElement);
                    WritePredicates(pathPart);
                }
                else if(IsAttribute(pathPart))
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

        private void WritePathPartSeparator()
        {
            XPath.Append(PathPartSeparator);
        }

        private void WriteElementName(XElement element)
        {
            if(string.IsNullOrEmpty(element.Name.NamespaceName))
            {
                XPath.Append(element.Name.LocalName);
                return;
            }
            var namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if(string.IsNullOrEmpty(namespacePrefix))
            {
                WriteElementNameWithoutPrefix(element.Name);
            }
            else
            {
                WriteElementNameWithPrefix(namespacePrefix, element.Name.LocalName);
            }
        }

        protected virtual void WriteElementNameWithoutPrefix(XName name)
        {
            XPath.AppendFormat("*[local-name()='{0}'][namespace-uri()='{1}']", name.LocalName, name.NamespaceName);
        }

        protected virtual void WriteElementNameWithPrefix(string namespacePrefix, string localName)
        {
            XPath.AppendFormat("{0}:{1}", namespacePrefix, localName);
        }

        protected virtual void WritePredicates(XPathPart pathPart)
        {
            if(pathPart.Predicates == null)
            {
                return;
            }
            if(pathPart.Predicates.Count == 0)
            {
                return;
            }
            foreach(var predicate in pathPart.Predicates)
            {
                WritePredicateStart();
                WriteAttributeName(predicate);
                WriteAttributeValue(predicate);
                WritePredicateEnd();
            }
        }

        protected void WritePredicateStart()
        {
            XPath.Append(PredicateStart);
        }

        private void WriteAttributeName(XAttribute attribute)
        {
            XPath.Append("@");
            if(string.IsNullOrEmpty(attribute.Name.NamespaceName))
            {
                XPath.Append(attribute.Name.LocalName);
                return;
            }

            if(attribute.Parent == null)
            {
                throw new XmlException(string.Format("Unable to determine namespace prefix for attribute \"{0}\". Parent is null.", attribute.Name));
            }

            var namespacePrefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace);
            if(string.IsNullOrEmpty(namespacePrefix))
            {
                WriteAttributeNameWithoutPrefix(attribute.Name);
            }
            else
            {
                WriteAttributeNameWithPrefix(namespacePrefix, attribute.Name.LocalName);
            }
        }

        protected virtual void WriteAttributeNameWithoutPrefix(XName name)
        {
            XPath.Append(name.LocalName);
        }

        protected virtual void WriteAttributeNameWithPrefix(string namespacePrefix, string localName)
        {
            XPath.AppendFormat("{0}:{1}", namespacePrefix, localName);
        }

        private void WriteAttributeValue(XAttribute attribute)
        {
            XPath.AppendFormat("='{0}'", attribute.Value);
        }

        protected void WritePredicateEnd()
        {
            XPath.Append(PredicateEnd);
        }

        private bool IsAttribute(XPathPart pathPart)
        {
            return pathPart.Node is XAttribute;
        }

        protected class XPathPart
        {
            public XObject Node
            {
                get;
                set;
            }

            public IList<XAttribute> Predicates
            {
                get;
                set;
            }
        }
    }
}