using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class DistinctAttributeFilter : AttributeFilter
    {
        private IList<XPathCandidate> _candidates;

        public DistinctAttributeFilter(IEnumerable<XPathSetting> settings)
            : base(settings)
        {
        }

        public override bool IsIncluded(XAttribute attribute)
        {
            if(!base.IsIncluded(attribute))
            {
                return false;
            }
            if(_candidates == null)
            {
                _candidates = FindXPathCandidates(attribute);
            }
            return _candidates.Any(candidate => candidate.Attributes.Contains(attribute));
        }

        private IList<XPathCandidate> FindXPathCandidates(XAttribute attribute)
        {
            var candidates = new List<XPathCandidate>();
            foreach(var element in attribute.Parent.AncestorsAndSelf().Reverse())
            {
                var candidate = CreateXPathCandidate(element);
                candidates.Add(candidate);
            }
            // Cache results for current document object
            return candidates;
        }

        private XPathCandidate CreateXPathCandidate(XElement element)
        {
            var identicalSiblings = element.ElementsBeforeSelf(element.Name).Union(element.ElementsAfterSelf(element.Name));
            return new XPathCandidate
                   {
                       Element = element,
                       Attributes = element.Attributes().
                                            Where(base.IsIncluded).
                                            Where(attribute => HasUniqueValue(attribute, identicalSiblings)).
                                            ToArray()
                   };
        }

        private bool HasUniqueValue(XAttribute attribute, IEnumerable<XElement> elements)
        {
            return !elements.Any(element => element.Attributes(attribute.Name).Any(a => a.Value == attribute.Value));
        }

        private class XPathCandidate
        {
            public XElement Element
            {
                get;
                set;
            }

            public IList<XAttribute> Attributes
            {
                get;
                set;
            }
        }
    }
}