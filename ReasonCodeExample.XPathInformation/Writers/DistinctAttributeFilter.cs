using System.Collections.Generic;
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
            if (attribute == null)
                return base.IsIncluded(attribute);
            if (attribute.Parent == null)
                return base.IsIncluded(attribute);
            // attribute.Parent.AncestorsAndSelf()
            // Must be run from the top down as e.g. multiple "/employee/id" elements could 
            // exist without "id" existing twice in any single "employee"-node
            // Check if there are sibling nodes similar to the current node.
            // if not return false (we don't need to include attributes)
            // Check if the current node has any useable "distinct attributes"
            // if not run check on ancestor node
            // Check each attribute to see if a sibling has a similar attribute with the same value
            // if not return true (we can use this attribute in the distinct path)
            return false;
        }
    }
}