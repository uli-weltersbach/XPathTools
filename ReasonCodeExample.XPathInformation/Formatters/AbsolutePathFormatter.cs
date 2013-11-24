using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class AbsolutePathFormatter : PathFormatter
    {
        protected override string GetElementName(XElement element)
        {
            string elementName = base.GetElementName(element);
            int elmentIndex = GetElementIndex(element);
            return string.Format("{0}[{1}]", elementName, elmentIndex);
        }

        private int GetElementIndex(XElement element)
        {
            return element.ElementsBeforeSelf(element.Name).Count() + 1;
        }
    }
}