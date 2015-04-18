using System.Collections.Generic;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class SimplifiedXPathWriter : XPathWriter
    {
        public SimplifiedXPathWriter()
        {
        }

        public SimplifiedXPathWriter(IEnumerable<IAttributeFilter> filters)
            : base(filters)
        {
        }

        protected override void WriteElementNameWithoutPrefix(XName element)
        {
            XPath.Append(element.LocalName);
        }

        protected override void WriteElementNameWithPrefix(string namespacePrefix, string localName)
        {
            XPath.Append(localName);
        }

        protected override void WriteAttributeNameWithoutPrefix(XName name)
        {
            XPath.Append(name.LocalName);
        }

        protected override void WriteAttributeNameWithPrefix(string namespacePrefix, string localName)
        {
            XPath.Append(localName);
        }
    }
}