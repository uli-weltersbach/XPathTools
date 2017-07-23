using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal class DistinctAttributeFilter : AttributeFilter
    {
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
            var candidates = FindCandidateAttributes(attribute);
            return candidates.Contains(attribute);
        }

        private IList<XAttribute> FindCandidateAttributes(XAttribute attribute)
        {
            var attributes = new List<XAttribute>();
            var ancestorsAndSelf = attribute.Parent.AncestorsAndSelf().Reverse();
            var requiresDistinctAttribute = false;
            foreach(var element in ancestorsAndSelf)
            {
                var hasIdenticallyNamedSiblings = GetIdenticallyNamedSiblings(element).Any();
                var candidateAttribute = FindCandidateAttribute(element);
                var hasCandidateAttribute = candidateAttribute != null;
                requiresDistinctAttribute = requiresDistinctAttribute || (hasIdenticallyNamedSiblings && !hasCandidateAttribute);
                if (requiresDistinctAttribute || hasIdenticallyNamedSiblings)
                {
                    attributes.Add(candidateAttribute);
                }
            }
            return attributes;
        }

        private XAttribute FindCandidateAttribute(XElement element)
        {
            var identicallyNamedSiblings = GetIdenticallyNamedSiblings(element);
            return GetFirstIncludedAttributeWithUniqueValue(element, identicallyNamedSiblings);
        }

        private IList<XElement> GetIdenticallyNamedSiblings(XElement element)
        {
            return element.ElementsBeforeSelf(element.Name).Union(element.ElementsAfterSelf(element.Name)).ToArray();
        }

        private XAttribute GetFirstIncludedAttributeWithUniqueValue(XElement element, IEnumerable<XElement> identicallyNamedSiblings)
        {
            return element.Attributes().Where(base.IsIncluded).FirstOrDefault(attribute => HasUniqueValue(attribute, identicallyNamedSiblings));
        }

        private bool HasUniqueValue(XAttribute attribute, IEnumerable<XElement> elements)
        {
            var identicallyNamedAttributes = elements.SelectMany(element => element.Attributes(attribute.Name));
            return identicallyNamedAttributes.All(otherAttribute => otherAttribute.Value != attribute.Value);
        }
    }
}