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
            if(!base.IsIncluded(attribute))
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
            var identicalSiblings = element.ElementsBeforeSelf(element.Name).Union(element.ElementsAfterSelf(element.Name));
            return element.Attributes().Where(base.IsIncluded).FirstOrDefault(attribute => HasUniqueValue(attribute, identicalSiblings));
        }

        private bool HasUniqueValue(XAttribute attribute, IEnumerable<XElement> elements)
        {
            return !elements.Any(element => element.Attributes(attribute.Name).Any(a => a.Value == attribute.Value));
        }
    }
}