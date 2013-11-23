using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class AbsolutePathFormatter : XPathFormatter
    {
        protected override string GetElementName(XElement element)
        {
            string elementName = base.GetElementName(element);
            int? elmentIndex = GetElementIndex(element);
            return elmentIndex.HasValue ? string.Format("{0}[{1}]", elementName, elmentIndex) : elementName;
        }

        private int? GetElementIndex(XElement element)
        {
            int elementsBefore = element.ElementsBeforeSelf(element.Name).Count();
            int elementsAfter = element.ElementsAfterSelf(element.Name).Count();
            bool hasSiblingsWithSameName = elementsBefore > 0 || elementsAfter > 0;
            if (hasSiblingsWithSameName)
                return elementsBefore + 1;
            return null;
        }
    }
}