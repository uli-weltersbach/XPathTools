using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class XPathWriter
    {
        private const string PathSeparator = "/";
        private readonly IEnumerable<INodeFilter> _filters;
        private readonly StringBuilder _xpath = new StringBuilder();
        private int _index;
        private XObject _end;

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
            _end = node;
            var significantNodes = GetSignificantNodes(node);
            Write(significantNodes);
            return _xpath.ToString();
        }

        private List<XObject> GetSignificantNodes(XObject node)
        {
            XElement element;
            if (IsElement(node))
                element = (XElement)node;
            else if (IsAttribute(node))
                element = node.Parent;
            else
                throw new ArgumentException("Node is not an element or attribute: " + node.GetType(), "node");

            var significantNodes = new List<XObject>();
            var ancestorsAndSelf = element.AncestorsAndSelf().Reverse();
            foreach (XElement ancestor in ancestorsAndSelf)
            {
                significantNodes.Add(ancestor);
                var attributes = ancestor.Attributes().Where(MatchesAnyFilter);
                significantNodes.AddRange(attributes);
            }
            if (significantNodes.Last() != node)
                significantNodes.Add(node);
            return significantNodes;
        }

        private bool MatchesAnyFilter(XObject node)
        {
            return _filters.Any(filter => filter.IsIncluded(node));
        }

        private void Write(List<XObject> significantNodes)
        {
            for (_index = 0; _index < significantNodes.Count; _index++)
            {
                var current = significantNodes[_index];
                if (IsElement(current))
                {
                    WritePathSeparator();
                    WriteElement(current as XElement);
                    WritePredicates(significantNodes);
                }
                else if (IsAttribute(current))
                {
                    WritePathSeparator();
                    WriteAttribute(current as XAttribute);
                }
            }
        }

        private bool IsElement(XObject node)
        {
            return node is XElement;
        }

        private void WritePathSeparator()
        {
            _xpath.Append(PathSeparator);
        }

        private void WriteElement(XElement element)
        {
            _xpath.Append(element.Name);
        }

        private void WritePredicates(List<XObject> significantNodes)
        {
            if (IsNextNodePredicatePart(significantNodes))
            {
                _index++;
                WritePredicateStart();
                WriteAttribute(significantNodes[_index] as XAttribute);
                WritePredicateEnd();
            }
        }

        private bool IsNextNodePredicatePart(List<XObject> significantNodes)
        {

            if (_index + 1 >= significantNodes.Count)
                return false;
            XObject nextNode = significantNodes[_index + 1];
            return IsAttribute(nextNode) && nextNode != _end;
        }

        private void WritePredicateStart()
        {
            _xpath.Append("[");
        }

        private void WritePredicateEnd()
        {
            _xpath.Append("]");
        }

        private bool IsAttribute(XObject node)
        {
            return node is XAttribute;
        }

        private void WriteAttribute(XAttribute attribute)
        {
            _xpath.Append("@").Append(attribute.Name);
        }
    }
}