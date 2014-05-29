using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class AbsoluteXPathWriter : XPathWriter
    {
        public AbsoluteXPathWriter()
        {
        }

        public AbsoluteXPathWriter(IEnumerable<IAttributeFilter> filters)
            : base(filters)
        {
        }

        protected override void WritePredicates(XPathPart pathPart)
        {
            WritePredicateStart();
            XPath.Append(GetElementIndex((XElement)pathPart.Node));
            WritePredicateEnd();
            base.WritePredicates(pathPart);
        }

        private int GetElementIndex(XElement element)
        {
            return element.ElementsBeforeSelf(element.Name).Count() + 1;
        }
    }
}