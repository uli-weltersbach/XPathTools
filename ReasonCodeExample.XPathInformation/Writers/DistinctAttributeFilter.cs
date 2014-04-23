using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class DistinctAttributeFilter : AttributeFilter
    {
        public DistinctAttributeFilter(IEnumerable<XPathSetting> settings)
            : base(settings)
        {
        }

        public override bool IsIncluded(XAttribute attribute)
        {
            if (!base.IsIncluded(attribute))
            {
                return false;
            }
            var candidates = FindCandidateAttributes(attribute);
            return candidates.Contains(attribute);
        }

        private IList<XAttribute> FindCandidateAttributes(XAttribute attribute)
        {
            return attribute.Parent.AncestorsAndSelf().Reverse().Select(FindCandidateAttribute).ToArray();
        }

        private XAttribute FindCandidateAttribute(XElement element)
        {
            var identicallyNamedSiblings = element.ElementsBeforeSelf(element.Name).Union(element.ElementsAfterSelf(element.Name));
            return element.Attributes().Where(base.IsIncluded).FirstOrDefault(attribute => HasUniqueValue(attribute, identicallyNamedSiblings));
        }

        private bool HasUniqueValue(XAttribute attribute, IEnumerable<XElement> elements)
        {
            var identicallyNamedAttributes = elements.SelectMany(element => element.Attributes(attribute.Name));
            return identicallyNamedAttributes.All(otherAttribute => otherAttribute.Value != attribute.Value);
        }
    }
}